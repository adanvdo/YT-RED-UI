using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YT_RED.Logging;
using YT_RED.Settings;

namespace YT_RED.Utils
{
    public static class VideoUtil
    {
        private static YoutubeDL ytdl;

        public static IProgress<DownloadProgress> ytProgress;
        public static IProgress<string> ytOutput;
        private static CancellationTokenSource cancellationTokenSource;
        private static YoutubeDLSharp.Helpers.ProcessRunner runner;
        private static Regex rgxFile = new Regex(@"\[download\] Destination: [a-zA-Z]:\\\S+\.\S{3,}", RegexOptions.Compiled);

        public static void Init()
        {
            ytdl = new YoutubeDL();
            ytdl.FFmpegPath = @".\Resources\App\ffmpeg.exe";
            string ytdlVer = Program.x64 ? "yt-dlp.exe" : "yt-dlp_x86.exe";
            ytdl.YoutubeDLPath = $@".\Resources\App\{ytdlVer}";
            ytdl.OutputFolder = AppSettings.Default.General.VideoDownloadPath;
            ytdl.OutputFileTemplate = "%(title)s_%(id)s.%(ext)s";
            ytdl.RestrictFilenames = true;
            ytdl.OverwriteFiles = false;
            runner = new YoutubeDLSharp.Helpers.ProcessRunner(4);
        }

        public static string GenerateUniqueYtdlFileName(Classes.StreamType streamType)
        {
            string dir = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
            return Path.Combine(dir, $"{DateTime.Now.ToString("MMddyyyyhhmmss")}.%(ext)s");
        }

        public static string GenerateUniqueFFmpegFileName()
        {
            return $"{DateTime.Now.ToString("MMddyyyyhhmmss")}";
        }
        
        public static int[] ConvertCrop(int[] crops, int videoWidth, int videoHeight)
        {
            if (crops.Length > 0 && videoWidth > 0 && videoHeight > 0)
            {
                
                int top = crops[0];
                int bottom = crops[1];
                int left = crops[2];
                int right = crops[3];

                int X = left;
                int Y = top;
                int width = videoWidth - X - right;
                int height = videoHeight - Y - bottom;

                if (X > videoWidth || Y > videoHeight || width > videoWidth || height > videoHeight)
                    throw new Exception("Crop Coordinates Exceed Video Dimensions");

                return new int[] { X, Y, width, height };
            }

            return null;
        }

        public static async Task<IConversion> PrepareYoutubeConversion(string url, Classes.YTDLFormatData formatData, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false, int[] crops = null, VideoFormat convertVideo = VideoFormat.UNSPECIFIED, AudioFormat convertAudio = AudioFormat.UNSPECIFIED)
        {
            List<string> getUrls = null;
            if (formatData.RedditAudioFormat != null)
            {
                getUrls = new List<string>();
                List<string> vUrls = await GetFormatUrls(url, formatData.FormatId.Split('+')[0]);
                if (vUrls != null)
                    getUrls.Add(vUrls[0]);
                List<string> aUrls = await GetFormatUrls(url, formatData.RedditAudioFormat.FormatId);
                if (aUrls != null)
                    getUrls.Add(aUrls[0]);
            }
            else
            {
                getUrls = await GetFormatUrls(url, formatData.FormatId);
            }
            if (getUrls == null || getUrls.Count < 1)
                return null;

            string videoUrl = string.Empty;
            string audioUrl = string.Empty;
            int outWidth = -1;
            int outHeight = -1;
            int x = -1;
            int y = -1;

            if (crops != null && crops.Length == 4 && formatData.Width != null && formatData.Height != null)
            {
                int[] ffmpegCrop = ConvertCrop(crops, (int)formatData.Width, (int)formatData.Height);
                x = ffmpegCrop[0];
                y = ffmpegCrop[1];
                outWidth = ffmpegCrop[2];
                outHeight = ffmpegCrop[3];
            }

            VideoFormat vFormat = VideoFormat.UNSPECIFIED;
            AudioFormat aFormat = AudioFormat.UNSPECIFIED;
            Classes.VideoCodecMap vMap = null;
            Classes.FFmpegVideoCodec vCodec = null;
            Classes.FFmpegAudioCodec aCodec = null;

            if (convertVideo != VideoFormat.UNSPECIFIED)
            {
                vFormat = convertVideo;
                vMap = Classes.SystemCodecMaps.GetMappedCodecs(convertVideo);
                vCodec = vMap.BestVideo;
                aCodec = vMap.BestAudio;
            }
            else if (convertAudio != AudioFormat.UNSPECIFIED)
            {
                aFormat = convertAudio;
                aCodec = Classes.SystemCodecMaps.GetAudioCodec(convertAudio);
            }            

            if (!string.IsNullOrEmpty(formatData.VideoCodec) && formatData.VideoCodec != "none")
            {
                videoUrl = getUrls[0];
                if (!string.IsNullOrEmpty(formatData.AudioCodec) && getUrls.Count > 1)
                {
                    audioUrl = getUrls[1];
                }

                if (vCodec == null && usePreferences)
                {
                    vFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                    vMap = Classes.SystemCodecMaps.GetMappedCodecs(vFormat);
                    vCodec = vMap.BestVideo;
                    aCodec = vMap.BestAudio;
                }
            }
            else if (!string.IsNullOrEmpty(formatData.AudioCodec) && formatData.AudioCodec != "none")
            {
                audioUrl = getUrls[0];
                if (aCodec == null && usePreferences)
                {
                    aFormat = AppSettings.Default.Advanced.PreferredAudioFormat;
                    aCodec = Classes.SystemCodecMaps.GetAudioCodec(aFormat);
                }
            }

            List<Classes.FFmpegParam> parameters = new List<Classes.FFmpegParam>();
            if(start != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.StartTime, $"-ss {((TimeSpan)start)}"));
            }
            if(duration != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Duration, $"-t {((TimeSpan)duration)}"));
            }           
            if (vCodec != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.VideoOutFormat, $"-c:v {vCodec.Encoder}"));
            }
            if (aCodec != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.AudioOutFormat, $"-c:a {aCodec.Encoder}"));
            }

            if (outWidth >= 0 && outHeight >= 0 && x >= 0 && y >= 0)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Crop, $"-filter:v \"crop={outWidth}:{outHeight}:{x}:{y}\""));
            }           

            return await PrepareStreamConversion(videoUrl, audioUrl, parameters.ToArray(), vFormat, aFormat);
        }

        public static async Task<IConversion> PrepareBestYtdlConversion(string url, string format, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false, int[] crops = null, VideoFormat convertVideo = VideoFormat.UNSPECIFIED, AudioFormat convertAudio = AudioFormat.UNSPECIFIED, bool embedThumbnail = false)
        {        
            var getUrls = await GetFormatUrls(url, format);
            if (getUrls == null || getUrls.Count < 1)
                return null;

            IMediaInfo videoInfo = null;
            IMediaInfo audioInfo = null;
            if (format == "bestvideo" || format == "bestvideo+bestaudio")
                videoInfo = await FFmpeg.GetMediaInfo(getUrls[0]);
            else if (format == "bestaudio")
            {
                audioInfo = await FFmpeg.GetMediaInfo(getUrls[0]);
            }
            if (format == "bestvideo+bestaudio")
            {
                audioInfo = await FFmpeg.GetMediaInfo(getUrls[1]);
            }

            IVideoStream videoStream = null;
            IAudioStream audioStream = null;
            if (videoInfo != null) 
            {
                videoStream = videoInfo.VideoStreams.FirstOrDefault();
            }
            if (audioInfo != null)
            {
                audioStream = audioInfo.AudioStreams.FirstOrDefault();
            }

            string videoUrl = string.Empty;
            string audioUrl = string.Empty;
            int outWidth = -1;
            int outHeight = -1;
            int x = -1;
            int y = -1;

            if (crops != null && crops.Length == 4 && videoStream != null)
            {
                int[] ffmpegCrop = ConvertCrop(crops, videoStream.Width, videoStream.Height);
                x = ffmpegCrop[0];
                y = ffmpegCrop[1];
                outWidth = ffmpegCrop[2];
                outHeight = ffmpegCrop[3];
            }

            VideoFormat vFormat = VideoFormat.UNSPECIFIED;
            AudioFormat aFormat = AudioFormat.UNSPECIFIED;
            Classes.VideoCodecMap vMap = null;
            Classes.FFmpegVideoCodec vCodec = null;
            Classes.FFmpegAudioCodec aCodec = null;

            if (convertVideo != VideoFormat.UNSPECIFIED)
            {
                vFormat = convertVideo;
                vMap = Classes.SystemCodecMaps.GetMappedCodecs(convertVideo);
                vCodec = vMap.BestVideo;
                aCodec = vMap.BestAudio;
            }
            else if (convertAudio != AudioFormat.UNSPECIFIED)
            {
                aFormat = convertAudio;
                aCodec = Classes.SystemCodecMaps.GetAudioCodec(convertAudio);
            }

            if (videoStream != null && !string.IsNullOrEmpty(videoStream.Codec))
            {
                videoUrl = getUrls[0];
                if (audioStream != null && !string.IsNullOrEmpty(audioStream.Codec) && getUrls.Count > 1)
                    audioUrl = getUrls[1];

                if (vCodec == null && usePreferences)
                {
                    vFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                    vMap = Classes.SystemCodecMaps.GetMappedCodecs(vFormat);
                    vCodec = vMap.BestVideo;
                    aCodec = vMap.BestAudio;
                }
            }
            else if (audioStream != null && !string.IsNullOrEmpty(audioStream.Codec))
            {
                audioUrl = getUrls[0];

                if (aCodec == null && usePreferences)
                {
                    aFormat = AppSettings.Default.Advanced.PreferredAudioFormat;
                    aCodec = Classes.SystemCodecMaps.GetAudioCodec(aFormat);
                }
            }

            List<Classes.FFmpegParam> parameters = new List<Classes.FFmpegParam>(); 
            if (start != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.StartTime, $"-ss {((TimeSpan)start)}"));
            }
            if (duration != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Duration, $"-t {((TimeSpan)duration)}"));
            }
            if(vCodec != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.VideoOutFormat, $"-c:v {vCodec.Encoder}"));
            }
            if(aCodec != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.AudioOutFormat, $"-c:a {aCodec.Encoder}"));
            }            

            if (outWidth >= 0 && outHeight >= 0 && x >= 0 && y >= 0)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Crop, $"-filter:v \"crop={outWidth}:{outHeight}:{x}:{y}\""));
            }

            return await PrepareStreamConversion(videoUrl, audioUrl, parameters.ToArray(), vFormat, aFormat);
        }

        public static async Task<IConversion> PrepareStreamConversion(string videoUrl = "", string audioUrl = "", Classes.FFmpegParam[] parameters = null, VideoFormat format = VideoFormat.UNSPECIFIED, AudioFormat aformat = AudioFormat.UNSPECIFIED)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl) && string.IsNullOrEmpty(audioUrl))
                    throw new ArgumentNullException("URL Invalid");
                string outputDir = !string.IsNullOrEmpty(videoUrl) ? AppSettings.Default.General.VideoDownloadPath : AppSettings.Default.General.AudioDownloadPath;
                string fileName = GenerateUniqueFFmpegFileName();
                string extension = "";

                IMediaInfo mediaInfo = null;
                IStream v = null;
                if (!string.IsNullOrEmpty(videoUrl))
                {
                    mediaInfo = await FFmpeg.GetMediaInfo(videoUrl);
                    v = mediaInfo.VideoStreams.FirstOrDefault();
                }

                IStream a = null;

                IMediaInfo audioInfo = null;
                if (!string.IsNullOrEmpty(audioUrl))
                {
                    audioInfo = await FFmpeg.GetMediaInfo(audioUrl);
                    a = audioInfo.AudioStreams.FirstOrDefault();
                }
                else if (mediaInfo != null && mediaInfo.AudioStreams.ToList().Count > 0)
                {
                    a = mediaInfo.AudioStreams.FirstOrDefault();
                }

                var convert = FFmpeg.Conversions.New();
                if (v != null)
                    convert.AddStream(v);
                if (a != null)
                    convert.AddStream(a);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        if (param.Type == Classes.ParamType.StartTime)
                            convert.AddParameter(param.Value, ParameterPosition.PreInput);
                        else
                            convert.AddParameter(param.Value, ParameterPosition.PostInput);
                    }
                }

                if (format == VideoFormat.UNSPECIFIED && aformat == AudioFormat.UNSPECIFIED)
                {
                    if (v != null)
                    {
                        bool tryVF = Enum.TryParse(v.Codec.ToUpper(), out Classes.SystemVideoCodec catchCodec);
                        if (tryVF)
                        {
                            format = Classes.SystemCodecMaps.GetBestFormat(catchCodec);
                        }
                    }
                    if (a != null)
                    {
                        bool tryAF = Enum.TryParse(a.Codec.ToUpper(), out AudioFormat catchFormat);
                        if (tryAF)
                        {
                            aformat = catchFormat;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(videoUrl))
                {
                    extension = $".{format}";
                }
                else if (!string.IsNullOrEmpty(audioUrl))
                {
                    if (aformat == AudioFormat.VORBIS)
                        extension = ".OGG";
                    else
                        extension = $".{aformat}";
                }
                fileName += extension;
                string outputFile = Path.Combine(outputDir, fileName);

                convert.SetOutput(outputFile);
                return convert;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }   

        public static async Task<YoutubeDLSharp.Metadata.VideoData> GetVideoData(string url)
        {
            RunResult<VideoData> res = await ytdl.RunVideoDataFetch(url);
            if(res.Success)
            {
                return res.Data;
            }
            else
            {
                throw new Exception(String.Join("\n", res.ErrorOutput));
            }
        }

        public static async Task<List<string>> GetFormatUrls(string url, string format)
        {
            var options = new YoutubeDLSharp.Options.OptionSet()
            {
                GetUrl = true,
                Format = format
            };
            try
            {
                var result = await ytdl.RunWithOptions(new[] { url }, options, default);
                if (result != null && result.Data.Length > 0)
                {
                    return result.Data.ToList();
                }
                return new List<string>();
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public static async Task<RunResult<string>> DownloadBestYtdl(string url, Classes.StreamType streamType, IProgress<DownloadProgress> dlProgress = null, IProgress<string> progressText = null, System.Threading.CancellationToken? cancellationToken = null, bool embedThumbnail = false)
        {
            bool cancelled = false;
            if (dlProgress == null) dlProgress = ytProgress;
            if (progressText == null) progressText = ytOutput;
            try
            {                
                ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                if(options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
                {
                    options.DeleteCustomOption("-o");
                }
                if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-")).Count() > 0)
                {
                    options.DeleteCustomOption("--postprocessor-args");
                    options.DeleteCustomOption("--convert-thumbnails");
                }
                if (!AppSettings.Default.General.UseTitleAsFileName)
                {
                    options.AddCustomOption<string>("-o", GenerateUniqueYtdlFileName(streamType));
                }
                AudioConversionFormat audioConversionFormat = AudioConversionFormat.Best;
                if (streamType == Classes.StreamType.Audio && embedThumbnail)
                {
                    audioConversionFormat = AudioConversionFormat.Mp3;
                    options.EmbedThumbnail = true;
                    options.AddMetadata = true;
                    options.AddCustomOption<string>("--postprocessor-args", "-write_id3v1 1 -id3v2_version 3");
                    options.AddCustomOption<string>("--convert-thumbnails", "jpg");
                }
                
                if (streamType == Classes.StreamType.AudioAndVideo)
                    return await ytdl.RunVideoDownload(url, "bestvideo+bestaudio", YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
                if (streamType == Classes.StreamType.Video)
                    return await ytdl.RunVideoDownload(url, "bestvideo", YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
                return await ytdl.RunAudioDownload(url, audioConversionFormat, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex);
                else cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static async Task<RunResult<string>> DownloadPreferredYtdl(string url, Classes.StreamType streamType, IProgress<DownloadProgress> dlProgress = null, IProgress<string> progressText = null, System.Threading.CancellationToken? cancellationToken = null, bool embedThumbnail = false)
        {
            bool cancelled = false;
            if (dlProgress == null) dlProgress = ytProgress;
            if (progressText == null) progressText = ytOutput;
            try
            {
                ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
                {
                    options.DeleteCustomOption("-o");
                }
                if (!AppSettings.Default.General.UseTitleAsFileName)
                {
                    options.AddCustomOption<string>("-o", GenerateUniqueYtdlFileName(streamType));
                }
                DownloadMergeFormat mergeFormat = DownloadMergeFormat.Unspecified;
                VideoRecodeFormat videoRecodeFormat = VideoRecodeFormat.None;
                switch (AppSettings.Default.Advanced.PreferredVideoFormat)
                {
                    case VideoFormat.MP4:
                        videoRecodeFormat = VideoRecodeFormat.Mp4;
                        mergeFormat = DownloadMergeFormat.Mp4;
                        break;
                    case VideoFormat.MKV:
                        videoRecodeFormat = VideoRecodeFormat.Mkv;
                        mergeFormat = DownloadMergeFormat.Mkv;
                        break;
                    case VideoFormat.FLV:
                        videoRecodeFormat= VideoRecodeFormat.Flv;
                        mergeFormat = DownloadMergeFormat.Flv;
                        break;
                    case VideoFormat.WEBM:
                        videoRecodeFormat= VideoRecodeFormat.Webm;
                        mergeFormat = DownloadMergeFormat.Webm;
                        break;
                    case VideoFormat.UNSPECIFIED:
                        videoRecodeFormat = VideoRecodeFormat.None;
                        mergeFormat = DownloadMergeFormat.Unspecified;
                        break;
                    default:
                        videoRecodeFormat = VideoRecodeFormat.None;
                        mergeFormat = DownloadMergeFormat.Unspecified;
                        break;
                }                

                if (streamType == Classes.StreamType.AudioAndVideo)
                    return await ytdl.RunVideoDownload(url, "bestvideo+bestaudio", mergeFormat, videoRecodeFormat, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
                else if (streamType == Classes.StreamType.Video)
                    return await ytdl.RunVideoDownload(url, "bestvideo", mergeFormat, videoRecodeFormat, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
                else
                    return await DownloadAudioYTDL(url, AppSettings.Default.Advanced.PreferredAudioFormat, cancellationToken, embedThumbnail);

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static async Task<RunResult<string>> DownloadAudioYTDL(string url, AudioFormat audioFormat, System.Threading.CancellationToken? cancellationToken = null, bool embedThumbnail = false)
        {
            bool cancelled = false;
            try
            {
                AudioConversionFormat audioConversion = AudioConversionFormat.Best;
                switch (audioFormat)
                {
                    case AudioFormat.AAC:
                        audioConversion = AudioConversionFormat.Aac;
                        break;
                    case AudioFormat.FLAC:
                        audioConversion = AudioConversionFormat.Flac;
                        break;
                    case AudioFormat.M4A:
                        audioConversion = AudioConversionFormat.M4a;
                        break;
                    case AudioFormat.MP3:
                        audioConversion = AudioConversionFormat.Mp3;
                        break;
                    case AudioFormat.OGG:
                        audioConversion = AudioConversionFormat.Vorbis;
                        break;
                    case AudioFormat.OPUS:
                        audioConversion = AudioConversionFormat.Opus;
                        break;
                    case AudioFormat.VORBIS:
                        audioConversion = AudioConversionFormat.Vorbis;
                        break;
                    case AudioFormat.WAV:
                        audioConversion = AudioConversionFormat.Wav;
                        break;
                    case AudioFormat.UNSPECIFIED:
                        audioConversion = AudioConversionFormat.Best;
                        break;
                    default:
                        audioConversion = AudioConversionFormat.Best;
                        break;
                }

                var options = YoutubeDLSharp.Options.OptionSet.Default;
                if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
                {
                    options.DeleteCustomOption("-o");
                }
                if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-")).Count() > 0)
                {
                    options.DeleteCustomOption("--postprocessor-args");
                    options.DeleteCustomOption("--convert-thumbnails");
                }
                if (!AppSettings.Default.General.UseTitleAsFileName)
                {
                    options.AddCustomOption<string>("-o", GenerateUniqueYtdlFileName(Classes.StreamType.Audio));
                }
                if (embedThumbnail && audioConversion != AudioConversionFormat.Opus && audioConversion != AudioConversionFormat.Aac)
                {
                    options.EmbedThumbnail = true;
                    options.AddMetadata = true;
                    options.AddCustomOption<string>("--postprocessor-args", "-write_id3v1 1 -id3v2_version 3");
                    options.AddCustomOption<string>("--convert-thumbnails", "jpg");
                }
                return await ytdl.RunAudioDownload(url, audioConversion, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, ytProgress, ytOutput, options);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static async Task<RunResult<string>> DownloadYTDLFormat(string videoUrl, Classes.YTDLFormatData formatData, bool embedThumbnail = false)
        {
            string outputFile = string.Empty;
            if (string.IsNullOrEmpty(videoUrl))
            {
                throw new ArgumentException("Invalid Video Url");
            }
            if(formatData == null)
            {
                throw new ArgumentNullException("FormatData is null");
            }

            var options = YoutubeDLSharp.Options.OptionSet.Default;
            if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
            {
                options.DeleteCustomOption("-o");
            }
            if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-")).Count() > 0)
            {
                options.DeleteCustomOption("--postprocessor-args");
                options.DeleteCustomOption("--convert-thumbnails");
            }

            if (formatData.VideoCodec == "none")
            {
                options.Format = formatData.FormatId;
                outputFile = GenerateUniqueYtdlFileName(Classes.StreamType.Audio);
                options.AddCustomOption<string>("-o", outputFile);
                
                if (embedThumbnail)
                {
                    options.EmbedThumbnail = true;
                    options.AddMetadata = true;
                    options.AddCustomOption<string>("--postprocessor-args", "-write_id3v1 1 -id3v2_version 3");
                    options.AddCustomOption<string>("--convert-thumbnails", "jpg");
                }
                return await downloadYTDLAudio(videoUrl, options);
            }           

            return await downloadYTDLVideo(videoUrl, formatData.FormatId);
        }

        private static string convertToArgs(string[] urls, OptionSet options)
            => (urls != null ? String.Join(" ", urls) : String.Empty) + options.ToString();

        public static async Task<RunResult<string>> DownloadYTDLGif(string url, OptionSet options)
        {
            if (options == null) options = new OptionSet();
            ytdl.OutputFolder = AppSettings.Default.General.VideoDownloadPath;
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = cancellationTokenSource.Token;
                string outFile = string.Empty;
                var process = new YoutubeDLProcess(ytdl.YoutubeDLPath);

                ytOutput?.Report($"Arguments: {convertToArgs(new[] { url }, options)}\n");
                process.OutputReceived += (o, e) =>
                {
                    var match = rgxFile.Match(e.Data);
                    if (match.Success)
                    {
                        outFile = match.Groups[0].ToString().Replace("[download] Destination:", "").Replace(" ", "");
                        ytProgress?.Report(new DownloadProgress(DownloadState.Success, data: outFile));
                    }
                    ytOutput?.Report(e.Data);
                };
                (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, options, cancelToken, ytProgress);
                return new RunResult<string>(code == 0, errors, outFile);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        private static async Task<RunResult<string>> downloadYTDLAudio(string url, OptionSet options)
        {
            if(options == null) throw new ArgumentNullException("options");
            ytdl.OutputFolder = AppSettings.Default.General.AudioDownloadPath;
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = cancellationTokenSource.Token;
                string outFile = string.Empty;
                var process = new YoutubeDLProcess(ytdl.YoutubeDLPath);

                ytOutput?.Report($"Arguments: {convertToArgs(new[] { url }, options)}\n");
                process.OutputReceived += (o, e) =>
                {
                    var match = rgxFile.Match(e.Data);
                    if (match.Success)
                    {
                        outFile = match.Groups[0].ToString().Replace("[download] Destination:", "").Replace(" ", "");
                        ytProgress?.Report(new DownloadProgress(DownloadState.Success, data: outFile));
                    }
                    ytOutput?.Report(e.Data);
                };
                (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, options, cancelToken, ytProgress);
                return new RunResult<string>(code == 0, errors, outFile);
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        private static async Task<RunResult<string>> downloadYTDLVideo(string url, string format)
        {
            try
            {
                ytdl.OutputFolder = AppSettings.Default.General.VideoDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
                {
                    options.DeleteCustomOption("-o");
                }
                if (!AppSettings.Default.General.UseTitleAsFileName)
                {
                    options.AddCustomOption<string>("-o", GenerateUniqueYtdlFileName(Classes.StreamType.Video));
                }
                if(format == "gif")
                {
                    return await DownloadYTDLGif(url, options);
                }
                return await ytdl.RunVideoDownload(url, format, YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, default, ytProgress, ytOutput, options);
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public static string CorrectYouTubeString(string linkOrID)
        {
            if (HtmlUtil.CheckUrl(linkOrID) == DownloadType.YouTube)
            {
                if (linkOrID.Length == 11)
                    linkOrID = @"https://www.youtube.com/watch?v=" + linkOrID;
                else if (linkOrID.Contains("/shorts/"))
                {
                    string id = linkOrID.Substring(linkOrID.IndexOf("/shorts/") + 8, 11);
                    linkOrID = $"https://www.youtube.com/watch?v={id}";
                }
                else if (linkOrID.StartsWith(@"https://youtu.be/"))
                {
                    string id = linkOrID.Substring(17, 11);
                    linkOrID = $"https://www.youtube.com/watch?v={id}";
                }
                else if (linkOrID.StartsWith(@"https://www.youtube.com/") || linkOrID.StartsWith(@"https://youtube.com/"))
                {
                    string id = linkOrID.Substring(linkOrID.IndexOf("v=") + 2, 11);
                    linkOrID = $"https://www.youtube.com/watch?v={id}";
                }
            }
            return linkOrID;
        }
    }
}
