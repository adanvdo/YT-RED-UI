using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.Office;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Events;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YT_RED.Classes;
using YT_RED.Logging;
using YT_RED.Settings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace YT_RED.Utils
{
    public static class VideoUtil
    {
        private static YoutubeDL ytdl;
        public static bool Running = false;
        public static IProgress<DownloadProgress> ytProgress;
        public static IProgress<string> ytOutput;
        public static event ConversionProgressEventHandler OnFFMpegProgress;
        public static CancellationTokenSource CancellationTokenSource;
        private static YoutubeDLSharp.Helpers.ProcessRunner runner;
        private static Regex rgxFile = new Regex(@"\[download\] Destination: [a-zA-Z]:\\\S+\.\S{3,}", RegexOptions.Compiled);
        public static Dictionary<string, string> TemporaryFiles;

        public static void Init()
        {
            string ytdlVer = Program.x64 ? "yt-dlp.exe" : "yt-dlp_x86.exe";
            string workingDir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            ytdl = new YoutubeDL(4, $@"{workingDir}\Resources\App\{ytdlVer}", $@"{workingDir}\Resources\App")
            {
                OutputFolder = AppSettings.Default.General.VideoDownloadPath,
                OutputFileTemplate = "%(title)s_%(id)s.%(ext)s",
                RestrictFilenames = true,
                OverwriteFiles = false
            };
            runner = new YoutubeDLSharp.Helpers.ProcessRunner(4);
            TemporaryFiles = new Dictionary<string, string>();
        }

        public static List<string> ResolutionList { 
            get
            {
                List<string> resolutions = new List<string>();
                resolutions.AddRange(Enum.GetNames(typeof(Classes.Resolution)).Cast<string>());
                return resolutions;
            } 
        }

        public static string GenerateUniqueYtdlFileName(Classes.StreamType streamType, string playlist = "")
        {
            string dir = "";
            if (string.IsNullOrEmpty(playlist))
                dir = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
            else
                dir = streamType == Classes.StreamType.Audio ? $@"{AppSettings.Default.General.AudioDownloadPath}\{playlist}" : $@"{AppSettings.Default.General.VideoDownloadPath}\{playlist}";
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

        public static async Task<IConversion> PrepareYoutubeConversion(string url, Classes.YTDLFormatPair formatPair, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false, 
            int[] crops = null, VideoFormat convertVideo = VideoFormat.UNSPECIFIED, AudioFormat convertAudio = AudioFormat.UNSPECIFIED, string prependPath = "", int prependDuration = 0, MediaDuration? prependType = null,
            string audioPath = "", TimeSpan? audioStart = null, TimeSpan? audioDuration = null, string imagePath = "")
        {
            List<string> getUrls = new List<string>();
            List<string> vUrls = new List<string>();
            List<string> aUrls = new List<string>();
            if (formatPair.RedditAudioFormat != null)
            {
                if (formatPair.VideoFormat != null)
                {
                    vUrls = await GetFormatUrls(url, formatPair.VideoFormat.FormatId == null ? formatPair.VideoFormat.Format.Split(' ')[0] : formatPair.VideoFormat.FormatId.Split('+')[0], ytProgress, ytOutput);
                    if (vUrls != null)
                        getUrls.Add(vUrls[0]);
                }
                aUrls = await GetFormatUrls(url, formatPair.RedditAudioFormat.FormatId, ytProgress, ytOutput);
                if (aUrls != null)
                    getUrls.Add(aUrls[0]);
            }
            else
            {
                if (formatPair.VideoFormat != null)
                {
                    vUrls = await GetFormatUrls(url, formatPair.VideoFormat.FormatId == null ? formatPair.VideoFormat.Format.Split(' ')[0] : formatPair.VideoFormat.FormatId.Split('+')[0], ytProgress, ytOutput);
                    if (vUrls != null)
                        getUrls.Add(vUrls[0]);
                }
                if (formatPair.AudioFormat != null)
                {
                    aUrls = await GetFormatUrls(url, formatPair.AudioFormat.FormatId == null ? formatPair.AudioFormat.Format.Split(' ')[0] : formatPair.AudioFormat.FormatId.Split('+')[0], ytProgress, ytOutput);
                    if (aUrls != null)
                        getUrls.Add(aUrls[0]);
                }
            }

            if (getUrls == null || getUrls.Count < 1)
                return null;

            string videoUrl = string.Empty;
            string audioUrl = string.Empty;
            int outWidth = -1;
            int outHeight = -1;
            int x = -1;
            int y = -1;

            if (crops != null && crops.Length == 4 && (formatPair.VideoFormat != null && formatPair.VideoFormat.Width != null && formatPair.VideoFormat.Height != null))
            {
                int[] ffmpegCrop = ConvertCrop(crops, (int)formatPair.VideoFormat.Width, (int)formatPair.VideoFormat.Height);
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
            
            if (convertAudio != AudioFormat.UNSPECIFIED)
            {
                aFormat = convertAudio;
                aCodec = Classes.SystemCodecMaps.GetAudioCodec(convertAudio);
            }            

            if (!string.IsNullOrEmpty(formatPair.VideoCodec) && formatPair.VideoCodec != "none")
            {
                videoUrl = getUrls[0];
                if (!string.IsNullOrEmpty(formatPair.AudioCodec) && getUrls.Count > 1)
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
            else if (!string.IsNullOrEmpty(formatPair.AudioCodec) && formatPair.AudioCodec != "none")
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
                if (vCodec == SystemCodecMaps.RGB24)
                    parameters.Add(new Classes.FFmpegParam(Classes.ParamType.VideoOutFormat, $"-pix_fmt {vCodec.Encoder}"));
                else
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

        public static async Task<IConversion> PrepareBestYtdlConversion(string url, string format, TimeSpan? start = null, TimeSpan? duration = null, bool usePreferences = false, int[] crops = null,
            VideoFormat convertVideo = VideoFormat.UNSPECIFIED, AudioFormat convertAudio = AudioFormat.UNSPECIFIED, bool embedThumbnail = false, Action<string> showOutput = null,
            string prependPath = "", int prependDuration = 0, MediaDuration? prependType = null, string audioPath = "", TimeSpan? audioStart = null, TimeSpan? audioDuration = null, string imagePath = "")
        {
            List<Classes.FFmpegParam> parameters = new List<Classes.FFmpegParam>();
            showOutput("Evaluating Formats..");
            var getUrls = await GetFormatUrls(url, format);
            if (getUrls == null || getUrls.Count < 1)
                return null;

            Uri vUrl = new Uri(getUrls[0]);
            string baseUrl = vUrl.GetLeftPart(UriPartial.Path);

            IMediaInfo videoInfo = null;
            IMediaInfo audioInfo = null;
            showOutput("Fetching Media Info..");
            CancellationTokenSource = new CancellationTokenSource();

            if (format.StartsWith("bestvideo") || format.Contains("+bestaudio/best"))
            {
                videoInfo = await FFmpeg.GetMediaInfo(getUrls[0], CancellationTokenSource.Token);
                if (!string.IsNullOrEmpty(audioPath))
                {
                    audioInfo = await FFmpeg.GetMediaInfo(audioPath, CancellationTokenSource.Token);
                    showOutput("Creating Temporary Audio");
                    if (audioStart != null && audioStart != TimeSpan.Zero) parameters.Add(new FFmpegParam(ParamType.StartTime, $"-ss {((TimeSpan)audioStart)}"));
                    if (audioDuration != null && audioDuration != TimeSpan.Zero && audioDuration <= videoInfo.Duration)
                        parameters.Add(new FFmpegParam(ParamType.Duration, $"-t {((TimeSpan)audioDuration)}"));
                    else
                        parameters.Add(new FFmpegParam(ParamType.Duration, $"-t {videoInfo.Duration}"));
                    parameters.Add(new FFmpegParam(ParamType.AudioOutFormat, "-c:a copy"));
                    IConversion audioConversion = await PrepareStreamConversion(null, audioPath, parameters.ToArray());
                    string destination = audioConversion.OutputFilePath;
                    CancellationTokenSource = new System.Threading.CancellationTokenSource();
                    audioConversion.OnProgress += OnFFMpegProgress;
                    await audioConversion.Start(VideoUtil.CancellationTokenSource.Token);
                    if (File.Exists(destination))
                    {
                        TemporaryFiles.Add(vUrl.OriginalString, destination);
                        if (getUrls.Count > 1)
                        {
                            getUrls[1] = destination;
                        }
                        else if (getUrls.Count == 1)
                        {
                            getUrls.Add(destination);
                        }
                    }
                    parameters.Clear();
                }
            }
            else if (format.StartsWith("bestaudio"))
            {
                audioInfo = await FFmpeg.GetMediaInfo(getUrls[0], CancellationTokenSource.Token);
            }

            CancellationTokenSource = new CancellationTokenSource();
            if (videoInfo != null && format.Contains("+bestaudio/best") && getUrls.Count > 1)
            {
                audioInfo = await FFmpeg.GetMediaInfo(getUrls[1], CancellationTokenSource.Token);
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

            showOutput("Preparing Post-Processing Args..");

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

                if (vCodec == null)
                {

                    if (usePreferences)
                        vFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                    else
                    {
                        var tryGetMap = Classes.SystemCodecMaps.GetMappedCodecs(videoStream.Codec);
                        vFormat = tryGetMap != null ? tryGetMap.Format : AppSettings.Default.Advanced.PreferredVideoFormat;
                    }

                    vMap = Classes.SystemCodecMaps.GetMappedCodecs(vFormat);
                    if(vFormat == VideoFormat.GIF)
                        vCodec = SystemCodecMaps.RGB24;
                    else
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

            if(!string.IsNullOrEmpty(prependPath) && videoStream != null)
            {
                var fr = videoStream.Framerate;
                var pf = videoStream.PixelFormat;
                var w = videoStream.Width;
                var h = videoStream.Height;
                var d = videoStream.Duration;

                if (fr >= 1 && w > 0 && h > 0 && d > TimeSpan.Zero)
                {
                    TimeSpan imgDur = TimeSpan.Zero;
                    if(prependType == MediaDuration.Frames)
                    {   
                        double second = prependDuration / fr;
                        double ms = 1000 * second;
                        imgDur = TimeSpan.FromMilliseconds(ms);                        
                    }
                    else
                    {
                        imgDur = TimeSpan.FromSeconds(prependDuration);
                    }

                    //parameters.Add(new FFmpegParam(ParamType.ANULLSRC, $"-f lavfi -i anullsrc"));
                    parameters.Add(new FFmpegParam(ParamType.Loop, "-loop 1"));
                    parameters.Add(new FFmpegParam(ParamType.Framerate, $"-framerate {fr}"));
                    parameters.Add(new FFmpegParam(ParamType.Input, $"-i {prependPath}"));
                    parameters.Add(new FFmpegParam(ParamType.ANULLSRC, $"-f lavfi -i anullsrc"));
                    parameters.Add(new FFmpegParam(ParamType.VideoCodec, $"-c:v {vCodec.Encoder}"));
                    parameters.Add(new FFmpegParam(ParamType.Duration, $"-t {((TimeSpan)imgDur)}"));
                    parameters.Add(new FFmpegParam(ParamType.PixelFormat, $"-pix_fmt {pf}"));
                    parameters.Add(new FFmpegParam(ParamType.VF, $"-vf \"scale={w}:{h}:force_original_aspect_ratio=decrease,pad={w}:{h}:(ow-iw)/2:(oh-ih)/2\""));
                    parameters.Add(new FFmpegParam(ParamType.Map, $"-map 0:v -map 0:a? -map 1:a -shortest"));

                    IConversion imageConversion = await PrepareStreamConversion(prependPath, null, parameters.ToArray(), vFormat);
                    string destination = imageConversion.OutputFilePath;
                    CancellationTokenSource = new System.Threading.CancellationTokenSource();
                    imageConversion.OnProgress += OnFFMpegProgress;
                    await imageConversion.Start(VideoUtil.CancellationTokenSource.Token);
                    if (File.Exists(destination))
                    {
                        TemporaryFiles.Add(url, destination);
                    }
                    parameters.Clear();
                }
            }
            
            if (start != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.StartTime, $"-ss {((TimeSpan)start)}"));
            }
            if (duration != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Duration, $"-t {((TimeSpan)duration)}"));
            }
            if (vCodec != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.VideoOutFormat, vFormat == VideoFormat.GIF ? $"-pix_fmt {vCodec.Encoder}" : $"-c:v {vCodec.Encoder}"));
            }
            if (aCodec != null)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.AudioOutFormat, $"-c:a {aCodec.Encoder}"));
            }

            if (outWidth >= 0 && outHeight >= 0 && x >= 0 && y >= 0)
            {
                parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Crop, $"-filter:v \"crop={outWidth}:{outHeight}:{x}:{y}\""));
            }

            showOutput("Preparing Conversion..");
            IConversion createConversion;
            if ((videoStream != null && videoStream.Path == videoUrl) || audioStream.Path == audioUrl)
                createConversion = await PrepareStreamConversionFromStreams(videoStream, audioStream, parameters.ToArray(), vFormat, aFormat);
            else 
                createConversion = await PrepareStreamConversion(videoUrl, audioUrl, parameters.ToArray(), vFormat, aFormat);
            showOutput("Starting Conversion..");
            return createConversion;
        }

        public static async Task<IConversion> PrepareVideoConcatenationConversion(string first, string second)
        {
            string fileName = GenerateUniqueFFmpegFileName();
            FileInfo fi = new FileInfo(second);
            if (fi != null)
            {
                string extension = !fi.Extension.StartsWith(".") ? $".{fi.Extension}" : fi.Extension;
                var conversion = await FFmpeg.Conversions.FromSnippet.Concatenate(Path.Combine(AppSettings.Default.General.VideoDownloadPath, $"{fileName}{extension}"), first, second);
                return conversion;
            }
            return null;
        }

        public static async Task<IConversion> PrepareStreamConversionFromStreams(IVideoStream videoStream = null, IAudioStream audioStream = null, FFmpegParam[] parameters = null, VideoFormat format = VideoFormat.UNSPECIFIED, AudioFormat aformat = AudioFormat.UNSPECIFIED, string prependPath = "")
        {
            return await PrepareStreamConversion(null, null, parameters, format, aformat, null, null, videoStream, audioStream);
        }

        public static async Task<IConversion> PrepareStreamConversion(string videoUrl = "", string audioUrl = "", Classes.FFmpegParam[] parameters = null, VideoFormat format = VideoFormat.UNSPECIFIED, AudioFormat aformat = AudioFormat.UNSPECIFIED, 
            IMediaInfo videoInfo = null, IMediaInfo audioInfo = null, IVideoStream videoStream = null, IAudioStream audioStream = null)
        {
            try
            {
                if ((videoInfo == null && audioInfo == null) && (videoStream == null && audioStream == null) && (string.IsNullOrEmpty(videoUrl) && string.IsNullOrEmpty(audioUrl)))
                    throw new ArgumentNullException("Invalid Parameters");

                string outputDir = "";
                if (string.IsNullOrEmpty(videoUrl) && videoInfo == null && videoStream == null)
                       outputDir = AppSettings.Default.General.AudioDownloadPath;
                else
                    outputDir = AppSettings.Default.General.VideoDownloadPath;

                string fileName = GenerateUniqueFFmpegFileName();
                string extension = "";                

                IMediaInfo mediaInfo = null;
                IVideoStream v = null;
                IAudioStream a = null;

                if (videoInfo == null && videoStream == null)
                {
                    if (!string.IsNullOrEmpty(videoUrl))
                    {
                        CancellationTokenSource = new CancellationTokenSource();
                        mediaInfo = await FFmpeg.GetMediaInfo(videoUrl, CancellationTokenSource.Token);
                        v = mediaInfo.VideoStreams.FirstOrDefault();
                    }
                } 
                else if(videoStream != null)
                {
                    v = videoStream;
                }
                else
                {
                    v = videoInfo.VideoStreams.FirstOrDefault();
                }

                if (audioInfo == null && audioStream == null)
                {               
                    if (!string.IsNullOrEmpty(audioUrl))
                    {
                        CancellationTokenSource = new CancellationTokenSource();
                        audioInfo = await FFmpeg.GetMediaInfo(audioUrl, CancellationTokenSource.Token);
                        a = audioInfo.AudioStreams.FirstOrDefault();
                    }
                    else if (mediaInfo != null && mediaInfo.AudioStreams.ToList().Count > 0)
                    {
                        a = mediaInfo.AudioStreams.FirstOrDefault();
                    }
                }
                var convert = FFmpeg.Conversions.New();
                var segStartParam = parameters == null || parameters.Length < 1 ? null : parameters.FirstOrDefault(p => p.Type == Classes.ParamType.StartTime);
                var segDurParam = parameters == null || parameters.Length < 1 ? null : parameters.FirstOrDefault(p => p.Type == Classes.ParamType.Duration);


                if (string.IsNullOrEmpty(videoUrl) || (!videoUrl.ToLower().EndsWith(".jpg") && !videoUrl.ToLower().EndsWith(".png")))
                {
                    if (v != null)
                    {
                        if (segStartParam != null && segDurParam != null)
                        {
                            bool startParse = TimeSpan.TryParse(segStartParam.Value.Replace("-ss ", ""), out TimeSpan startSpan);
                            bool durParse = TimeSpan.TryParse(segDurParam.Value.Replace("-t ", ""), out TimeSpan durSpan);
                            if (startParse && durParse)
                                v.SetSeek(startSpan);
                        }
                        convert.AddStream<Xabe.FFmpeg.IVideoStream>(v);
                    }
                    if (a != null)
                    {
                        if (segStartParam != null && segDurParam != null)
                        {
                            bool startParse = TimeSpan.TryParse(segStartParam.Value.Replace("-ss ", ""), out TimeSpan startSpan);
                            bool durParse = TimeSpan.TryParse(segDurParam.Value.Replace("-t ", ""), out TimeSpan durSpan);
                            if (startParse && durParse)
                                a.SetSeek(startSpan);
                        }
                        convert.AddStream<Xabe.FFmpeg.IAudioStream>(a);
                    }
                }

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        if (param.Type != Classes.ParamType.StartTime)
                        {
                            if (param.Type != ParamType.Loop && param.Type != ParamType.Framerate)
                                convert.AddParameter(param.Value, ParameterPosition.PostInput);
                            else
                                convert.AddParameter(param.Value, ParameterPosition.PreInput);
                        }
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
                else
                {
                    extension = $".{format}";
                }
                fileName += extension;
                string outputFile = Path.Combine(outputDir, fileName);

                convert.SetOutput(outputFile);
                var test = convert.Build();

                return convert;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, videoUrl, audioUrl, true);
            }
            return null;
        }   

        public static async Task<YoutubeDLSharp.Metadata.VideoData> GetPlaylistData(string url)
        {
            CancellationTokenSource = new CancellationTokenSource();
            OptionSet plOptions = new OptionSet();
            plOptions.FlatPlaylist = true;
            plOptions.DumpSingleJson = true;
            RunResult<VideoData> res = await ytdl.RunVideoDataFetch(url, CancellationTokenSource.Token, true, plOptions, ytProgress, ytOutput, false, AppSettings.Default.Advanced.VerboseOutput);
            if (res.Success)
            {
                return res.Data;
            }
            else
            {
                throw new Exception(String.Join("\n", res.ErrorOutput));
            }
        }

        public static async Task<YoutubeDLSharp.Metadata.VideoData> GetVideoData(string url, bool useFfmpegMetaDataFallback = false)
        {
            CancellationTokenSource = new CancellationTokenSource();            
            RunResult<VideoData> res = await ytdl.RunVideoDataFetch(url, CancellationTokenSource.Token, true, null, ytProgress, ytOutput, useFfmpegMetaDataFallback, AppSettings.Default.Advanced.VerboseOutput);
            if(res.Success)
            {
                return res.Data;
            }
            else
            {
                throw new Exception(String.Join("\n", res.ErrorOutput));
            }
        }

        public static async Task<List<string>> GetFormatUrls(string url, string format, IProgress<DownloadProgress> progress = null, IProgress<string> output = null)
        {
            CancellationTokenSource = new CancellationTokenSource();
            var options = new YoutubeDLSharp.Options.OptionSet()
            {                
                GetUrl = true,
                Format = format
            };
            try
            {
                var result = await ytdl.RunWithOptions(new[] { url }, options, CancellationTokenSource.Token, progress, output, AppSettings.Default.Advanced.VerboseOutput);
                if (result != null && result.Data.Length > 0)
                {
                    return result.Data.ToList();
                }
                return new List<string>();
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex, url);
            }
            return null;
        }

        public static async Task<RunResult<string>> DownloadBestYtdl(string url, Classes.StreamType streamType, bool embedThumbnail = false, string playlist = "")
        {
            bool cancelled = false;            
            try
            {
                if (string.IsNullOrEmpty(playlist))
                    ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
                else
                    ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? $@"{AppSettings.Default.General.AudioDownloadPath}\{playlist}" : $@"{AppSettings.Default.General.VideoDownloadPath}\{playlist}";

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
                    options.AddCustomOption<string>("-o", GenerateUniqueYtdlFileName(streamType, playlist));
                }
                AudioConversionFormat audioConversionFormat = AudioConversionFormat.Best;
                if (streamType == Classes.StreamType.Audio)
                {
                    if(AppSettings.Default.General.MaxFilesizeBest > 0)
                    {
                        options.MaxFilesize = $"{AppSettings.Default.General.MaxFilesizeBest}M]";
                    }
                    if (embedThumbnail)
                    {
                        audioConversionFormat = AudioConversionFormat.Mp3;
                        options.EmbedThumbnail = true;
                        options.AddMetadata = true;
                        options.AddCustomOption<string>("--postprocessor-args", "-write_id3v1 1 -id3v2_version 3");
                        options.AddCustomOption<string>("--convert-thumbnails", "jpg");
                    }
                }

                string mainFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
                string finalFormatString = string.Empty;

                finalFormatString = String.Format(mainFormatString,
                    AppSettings.Default.General.MaxResolutionValue > 0 ? $"[height<={AppSettings.Default.General.MaxResolutionValue}]" : "",
                    AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxFilesizeBest}M]" : "");

                CancellationTokenSource = new CancellationTokenSource();

                if(streamType == Classes.StreamType.Audio)
                {
                    return await ytdl.RunAudioDownload(url, audioConversionFormat, CancellationTokenSource.Token, ytProgress, ytOutput, options, AppSettings.Default.Advanced.VerboseOutput);
                }
                
                return await ytdl.RunVideoDownload(url, finalFormatString, YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, 
                        YoutubeDLSharp.Options.VideoRecodeFormat.None, CancellationTokenSource.Token, ytProgress, ytOutput, options, AppSettings.Default.Advanced.VerboseOutput);             
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex, url);
                else cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static async Task<RunResult<string>> DownloadPreferredYtdl(string url, Classes.StreamType streamType, string playlist = "")
        {
            bool cancelled = false;
            try
            {
                if (string.IsNullOrEmpty(playlist))
                    ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
                else
                    ytdl.OutputFolder = streamType == Classes.StreamType.Audio ? $@"{AppSettings.Default.General.AudioDownloadPath}\{playlist}" : $@"{AppSettings.Default.General.VideoDownloadPath}\{playlist}";
                var options = YoutubeDLSharp.Options.OptionSet.Default;
                if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
                {
                    options.DeleteCustomOption("-o");
                }
                if (!AppSettings.Default.General.UseTitleAsFileName)
                {
                    options.AddCustomOption<string>("-o", GenerateUniqueYtdlFileName(streamType, playlist));
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

                string mainFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
                string finalFormatString = string.Empty;

                finalFormatString = String.Format(mainFormatString,
                    AppSettings.Default.General.MaxResolutionValue > 0 ? $"[height<={AppSettings.Default.General.MaxResolutionValue}]" : "",
                    AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxResolutionBest}M]" : "");

                CancellationTokenSource = new CancellationTokenSource();

                if(streamType == Classes.StreamType.Audio)
                {
                    return await DownloadAudioYTDL(url, AppSettings.Default.Advanced.PreferredAudioFormat, true, AppSettings.Default.General.MaxFilesizeBest);
                }

                return await ytdl.RunVideoDownload(url, finalFormatString, mergeFormat, videoRecodeFormat, CancellationTokenSource.Token, ytProgress, ytOutput, options, AppSettings.Default.Advanced.VerboseOutput);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex, url);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static async Task<RunResult<string>> DownloadAudioYTDL(string url, AudioFormat audioFormat, bool embedThumbnail = false, decimal maxFilesize = 0)
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
                if(maxFilesize > 0)
                {
                    options.MaxFilesize = $"{maxFilesize}M";
                }
                CancellationTokenSource = new CancellationTokenSource();
                return await ytdl.RunAudioDownload(url, audioConversion, CancellationTokenSource.Token, 
                    ytProgress, ytOutput, options, AppSettings.Default.Advanced.VerboseOutput);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex, null, url);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static async Task<RunResult<string>> DownloadYTDLFormat(string videoUrl, Classes.YTDLFormatPair formatPair, bool embedThumbnail = false)
        {
            string outputFile = string.Empty;
            if (string.IsNullOrEmpty(videoUrl))
            {
                throw new ArgumentException("Invalid Video Url");
            }
            if(formatPair == null || !formatPair.IsValid())
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

            if (formatPair.VideoCodec == "none")
            {
                options.Format = formatPair.AudioFormat.FormatId;
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

            return await downloadYTDLVideo(videoUrl, formatPair.FormatId);
        }

        private static string convertToArgs(string[] urls, OptionSet options)
            => (urls != null ? String.Join(" ", urls) : String.Empty) + options.ToString();

        public static async Task<RunResult<string>> DownloadYTDLGif(string url, OptionSet options)
        {
            bool cancelled = false;
            if (options == null) options = new OptionSet();
            ytdl.OutputFolder = AppSettings.Default.General.VideoDownloadPath;
            try
            {
                CancellationTokenSource = new CancellationTokenSource();
                string outFile = string.Empty;
                var process = new YoutubeDLProcess(ytdl.YoutubeDLPath);
                if (AppSettings.Default.Advanced.VerboseOutput)
                    ytOutput?.Report($"Arguments: {convertToArgs(new[] { url }, options)}\n");
                else
                    ytOutput?.Report($"Starting Downlaod: {url}");
                process.OutputReceived += (o, e) =>
                {
                    var match = rgxFile.Match(e.Data);
                    if (match.Success)
                    {
                        outFile = match.Groups[0].ToString().Replace("[download] Destination:", "").Replace(" ", "");
                        ytProgress.Report(new DownloadProgress(DownloadState.Success, data: outFile));
                    }
                    ytOutput?.Report(e.Data);
                };
                (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, options, CancellationTokenSource.Token, ytProgress, ytOutput);
                return new RunResult<string>(code == 0, errors, outFile);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex, null, url);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        private static async Task<RunResult<string>> downloadYTDLAudio(string url, OptionSet options)
        {
            bool cancelled = false;
            if (options == null) throw new ArgumentNullException("options");
            ytdl.OutputFolder = AppSettings.Default.General.AudioDownloadPath;
            try
            {
                CancellationTokenSource = new CancellationTokenSource();
                string outFile = string.Empty;
                var process = new YoutubeDLProcess(ytdl.YoutubeDLPath);

                if (AppSettings.Default.Advanced.VerboseOutput)
                    ytOutput?.Report($"Arguments: {convertToArgs(new[] { url }, options)}\n");
                else
                    ytOutput?.Report($"Starting Download: {url}");
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
                (int code, string[] errors) = await runner.RunThrottled(process, new[] { url }, options, CancellationTokenSource.Token, ytProgress, ytOutput);
                return new RunResult<string>(code == 0, errors, outFile);
            }
            catch(Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex, null, url);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        private static async Task<RunResult<string>> downloadYTDLVideo(string url, string format)
        {
            bool cancelled = false;
            try
            {
                CancellationTokenSource = new CancellationTokenSource();
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
                    options.Format = format;
                    return await DownloadYTDLGif(url, options);
                }
                return await ytdl.RunVideoDownload(url, format, YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified, YoutubeDLSharp.Options.VideoRecodeFormat.None, 
                    CancellationTokenSource.Token, ytProgress, ytOutput, options, AppSettings.Default.Advanced.VerboseOutput);
            }
            catch(Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex, null, url);
                else
                    cancelled = true;
            }
            if (cancelled)
                return new RunResult<string>(false, new string[] { "a task was canceled." }, "canceled");
            else
                return null;
        }

        public static Classes.YoutubeLink ConvertToYouTubeLink(string linkOrID)
        {
            YoutubeLinkType type = YoutubeLinkType.Invalid;
            if (HtmlUtil.CheckUrl(linkOrID) == DownloadType.YouTube)
            {
                if (linkOrID.Length == 11)
                {
                    linkOrID = @"https://www.youtube.com/watch?v=" + linkOrID;
                    type = YoutubeLinkType.Video;
                }
                else if (linkOrID.Length == 34)
                {
                    linkOrID = @"https://www.youtube.com/playlist?list=" + linkOrID;
                    type = YoutubeLinkType.Playlist;
                }
                else
                {
                    Uri uri = new Uri(linkOrID);
                    if (uri.Segments.Select(s => s.ToLower()).ToArray().Contains("shorts") && linkOrID.Remove(0, linkOrID.ToLower().IndexOf("/shorts/") + 8).Length == 11)
                    {
                        string id = linkOrID.Substring(linkOrID.IndexOf("/shorts/") + 8, 11);
                        linkOrID = $"https://www.youtube.com/watch?v={id}";
                        type = YoutubeLinkType.Short;
                    }
                    else if (linkOrID.StartsWith(@"https://youtu.be/") && linkOrID.Remove(0, 17).Length == 11)
                    {
                        string id = linkOrID.Substring(17, 11);
                        linkOrID = $"https://www.youtube.com/watch?v={id}";
                        type = YoutubeLinkType.Video;
                    }
                    else if (linkOrID.StartsWith(@"https://www.youtube.com/") || linkOrID.StartsWith(@"https://youtube.com/"))
                    {
                        string id = string.Empty;
                        if (uri.Segments.Select(s => s.ToLower()).ToArray().Contains("playlist"))
                        {
                            id = uri.Query.Substring(uri.Query.IndexOf("?"), uri.Query.Length - uri.Query.IndexOf("?")).Split('?').Where(p => p.ToLower().StartsWith("list")).FirstOrDefault().Split('=')[1];
                            linkOrID = $"https://www.youtube.com/playlist?list={id}";
                            type = YoutubeLinkType.Playlist;
                        }
                        else if(linkOrID.Remove(0, linkOrID.IndexOf("v=") + 2).Length == 11)
                        {
                            id = linkOrID.Substring(linkOrID.IndexOf("v=") + 2, 11);
                            linkOrID = $"https://www.youtube.com/watch?v={id}";
                            type = YoutubeLinkType.Video;
                        }
                    }
                }
                return new YoutubeLink(type, linkOrID);
            }

            return new YoutubeLink(Classes.YoutubeLinkType.Invalid, linkOrID);
        }

        public static async Task DeleteTemporaryFiles()
        {
            foreach(string v in TemporaryFiles.Values)
            {
                try
                {
                    FileInfo f = new FileInfo(v);
                    if (f.Exists)
                    {
                        await Task.Run(() => f.Delete());
                    }
                }
                catch(Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                }
            }
            TemporaryFiles.Clear();
        }
    }    
}
