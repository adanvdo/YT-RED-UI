using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static void Init()
        {
            ytdl = new YoutubeDL();
            ytdl.FFmpegPath = @".\Resources\App\ffmpeg.exe";
            string ytdlVer = Program.x64 ? "yt-dlp.exe" : "yt-dlp_x86.exe";
            ytdl.YoutubeDLPath = $@".\Resources\App\{ytdlVer}";
            ytdl.OutputFolder = AppSettings.Default.General.VideoDownloadPath;
        }

        public static string GetRedditVideoID(string m3u8URL)
        {
            if (m3u8URL == null)
                throw new ArgumentNullException("m3U8 URL was null");
            string id = m3u8URL.Replace(@"https://v.redd.it/", "");
            int endex = id.IndexOf('/');
            id = id.Substring(0, endex);
            return id;
        }

        public static async Task<IMediaInfo> ParseM3U8(string url)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(url);
            return mediaInfo;
        }

        public static async Task<IConversion> PrepareDashConversion(string videoUrl)
        {
            return await PrepareStreamConversion(videoUrl, string.Empty);
        }

        public static async Task<IConversion> PrepareYoutubeAudioConversion(string url, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false)
        {
            var getUrls = await GetFormatUrls(url, "best");
            if (getUrls == null || getUrls.Count < 1)
                return null;

            var getUrl = getUrls[0];
            List<Classes.FFmpegParam> parameters = new List<Classes.FFmpegParam>();
            if (start != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.StartTime, $"-ss {((TimeSpan)start)}"));
            }
            if (duration != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Duration, $"-t {((TimeSpan)duration)}"));
            }

            string ac = usePreferences ? AppSettings.Default.Advanced.PreferredAudioFormat.ToFriendlyString(true) : "mp3";
            parameters.Add(new Classes.FFmpegParam(Classes.ParamType.AudioOutFormat, $@" -acodec {ac}"));


            return await PrepareStreamConversion("", getUrl, parameters.ToArray(), VideoFormat.UNSPECIFIED, usePreferences ? AppSettings.Default.Advanced.PreferredAudioFormat : AudioFormat.MP3);
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


        public static async Task<IConversion> PrepareYoutubeConversion(string url, FormatData formatData, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false, int[] crops = null, VideoFormat convertVideo = VideoFormat.UNSPECIFIED, AudioFormat convertAudio = AudioFormat.UNSPECIFIED)
        {
            var getUrls = await GetFormatUrls(url, formatData.FormatId);
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
                    audioUrl = getUrls[1];

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

        public static async Task<IConversion> PrepareBestYtdlConversion(string url, string format, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false, int[] crops = null, VideoFormat convertVideo = VideoFormat.UNSPECIFIED, AudioFormat convertAudio = AudioFormat.UNSPECIFIED)
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
                audioInfo = await FFmpeg.GetMediaInfo(getUrls[1]);
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

        public static async Task<IConversion> PrepareStreamConversion(string videoUrl = "", string audioUrl = "", Classes.FFmpegParam[] parameters = null, VideoFormat format = VideoFormat.MP4, AudioFormat aformat = AudioFormat.MP3)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl) && string.IsNullOrEmpty(audioUrl))
                    throw new ArgumentNullException("URL Invalid");
                string outputDir = !string.IsNullOrEmpty(videoUrl) ? AppSettings.Default.General.VideoDownloadPath : AppSettings.Default.General.AudioDownloadPath;
                string fileName = DateTime.Now.ToString("MMddyyyyhhmmss");
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

        public static IConversion PrepareConversion(Classes.ResultStream stream, Classes.FFmpegParam[] parameters = null)
        {
            string outputDir = stream.StreamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
            string fileName = DateTime.Now.ToString("MMddyyyyhhmmss")+ (stream.StreamType == Classes.StreamType.Audio ? ".m4a" : ".mp4");
            string outputFile = Path.Combine(outputDir, fileName);

            if(stream.StreamType == Classes.StreamType.AudioAndVideo)
            {
                var convert = FFmpeg.Conversions.New()
                    .AddStream(stream.AudioStream, stream.VideoStream);
                if(parameters != null)
                {
                    foreach(var p in parameters)
                    {
                        if (p.Type == Classes.ParamType.StartTime)
                            convert.AddParameter(p.Value, ParameterPosition.PreInput);
                        else
                            convert.AddParameter(p.Value, ParameterPosition.PostInput);
                    }
                }
                convert.SetOutput(outputFile);
                return convert;
            }
            else if (stream.StreamType == Classes.StreamType.Video)
            {
                var convert = FFmpeg.Conversions.New()
                    .AddStream(stream.VideoStream)
                    .SetOutput(outputFile);
                return convert;
            }
            else {
                var convert = FFmpeg.Conversions.New()
                    .AddStream(stream.AudioStream)
                    .SetOutput(outputFile);
                return convert;
            }
        }

        public static async Task<List<Classes.ResultStream>> ConsolidateStreams(List<Classes.MediaStream> streams)
        {
            return await Task.Run(() =>
            {
                List<Classes.ResultStream> consolidated = new List<Classes.ResultStream>();
                List<Classes.MediaStream> video = streams.Where(s => s.StreamType == Classes.StreamType.Video).ToList();
                List<Classes.MediaStream> audio = streams.Where(s => s.StreamType == Classes.StreamType.Audio).ToList();

                int row = 1;
                foreach (Classes.MediaStream vs in video)
                {
                    consolidated.Add(new Classes.ResultStream(vs) { Row = row });
                    row++;
                }

                foreach (Classes.MediaStream s in audio)
                {
                    foreach (Classes.MediaStream vs in video)
                    {
                        consolidated.Add(new Classes.ResultStream(vs, s) { Row = row });
                        row++;
                    }
                }

                foreach (Classes.MediaStream vs in audio)
                {
                    consolidated.Add(new Classes.ResultStream(vs) { Row = row });
                    row++;
                }

                return consolidated;
            });
        }

        public static string RedditAudioUrl(string id)
        {
            return $@"{YT_RED.Settings.AppSettings.Default.General.RedditMediaURLPrefix}{id}{YT_RED.Settings.AppSettings.Default.General.RedditDefaultAudioSuffix}";
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

        public static async Task<RunResult<string>> DownloadBestYtdl(string url, Classes.StreamType streamType, IProgress<DownloadProgress> dlProgress = null, IProgress<string> progressText = null, System.Threading.CancellationToken? cancellationToken = null)
        {
            bool cancelled = false;
            try
            {
                ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                options.RestrictFilenames = true;
                if (streamType == Classes.StreamType.AudioAndVideo)
                    return await ytdl.RunVideoDownload(url, "bestvideo+bestaudio", YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
                if (streamType == Classes.StreamType.Video)
                    return await ytdl.RunVideoDownload(url, "bestvideo", YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
                return await ytdl.RunAudioDownload(url, AudioConversionFormat.Best, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, dlProgress == null ? ytProgress : dlProgress, progressText, options);
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

        public static async Task<RunResult<string>> DownloadPreferredYtdl(string url, Classes.StreamType streamType, IProgress<DownloadProgress> dlProgress = null, IProgress<string> progressText = null, System.Threading.CancellationToken? cancellationToken = null)
        {
            bool cancelled = false;
            try
            {
                ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                options.RestrictFilenames = true;
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
                    return await DownloadAudioYT(url, AppSettings.Default.Advanced.PreferredAudioFormat, cancellationToken);

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

        public static async Task<RunResult<string>> DownloadAudioYT(string url, AudioFormat audioFormat, System.Threading.CancellationToken? cancellationToken = null)
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

                ytdl.OutputFolder = AppSettings.Default.General.AudioDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                options.RestrictFilenames = true;
                return await ytdl.RunAudioDownload(url, audioConversion, cancellationToken != null ? (System.Threading.CancellationToken)cancellationToken : default, ytProgress, null, options);
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

        public static async Task<RunResult<string>> DownloadYTFormat(string videoUrl, FormatData formatData)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                throw new ArgumentException("Invalid Video Url");
            }
            if(formatData == null)
            {
                throw new ArgumentNullException("FormatData is null");
            }
            if (formatData.VideoCodec == "none")
            {
                return await DownloadAudioYT(videoUrl, AppSettings.Default.Advanced.PreferredAudioFormat);
            }
            return await downloadYTVideo(videoUrl, formatData.FormatId);
        }

        private static async Task<RunResult<string>> downloadYTVideo(string url, string format)
        {
            try
            {
                ytdl.OutputFolder = AppSettings.Default.General.VideoDownloadPath;
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                options.RestrictFilenames = true;
                return await ytdl.RunVideoDownload(url, format, YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, default, ytProgress, null, options);
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
