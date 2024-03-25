using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YTR.Settings;

namespace YTR.Classes
{   
    public static class SystemCodecMaps
    {
        public static FFmpegVideoCodec AV1 = new FFmpegVideoCodec(SystemVideoCodec.AV1, "libaom-av1");
        public static FFmpegVideoCodec MPEG4 = new FFmpegVideoCodec(SystemVideoCodec.MPEG4, "mpeg4");
        public static FFmpegVideoCodec H264 = new FFmpegVideoCodec(SystemVideoCodec.H264, "libx264");
        public static FFmpegVideoCodec H265 = new FFmpegVideoCodec(SystemVideoCodec.H265, "libx265");
        public static FFmpegVideoCodec VP9 = new FFmpegVideoCodec(SystemVideoCodec.VP9, "libvpx-vp9 -tag:v hvc1");
        public static FFmpegVideoCodec OGG = new FFmpegVideoCodec(SystemVideoCodec.THEORA, "libtheora -qscale:v 3");
        public static FFmpegAudioCodec AAC = new FFmpegAudioCodec(AudioFormat.AAC, "libfdk_aac");
        public static FFmpegAudioCodec FLAC = new FFmpegAudioCodec(AudioFormat.FLAC, "flac");
        public static FFmpegAudioCodec OPUS = new FFmpegAudioCodec(AudioFormat.OPUS, "libopus -qscale:a 3");
        public static FFmpegAudioCodec MP3 = new FFmpegAudioCodec(AudioFormat.MP3, "libmp3lame");
        public static FFmpegAudioCodec VORBIS = new FFmpegAudioCodec(AudioFormat.VORBIS, "libvorbis -qscale:a 3");
        public static FFmpegAudioCodec WAV = new FFmpegAudioCodec(AudioFormat.WAV, "pcm_s32le");

        public static VideoCodecMap FLV = new VideoCodecMap(VideoFormat.FLV, 
            new List<FFmpegVideoCodec>(){ 
                MPEG4,
                H264
            }, 
            new List<FFmpegAudioCodec>()
            {
                MP3
            }
        );

        public static VideoCodecMap MP4 = new VideoCodecMap(VideoFormat.MP4, 
            new List<FFmpegVideoCodec>()
            {
                AV1,
                MPEG4,
                H264,
                H265,
                VP9,
            },
            new List<FFmpegAudioCodec>()
            {
                OPUS,
                VORBIS,
                MP3,
                AAC
            }
        );

        public static VideoCodecMap MKV = new VideoCodecMap(VideoFormat.MKV, 
            new List<FFmpegVideoCodec>()
            {
                MPEG4,
                H264, 
                H265,
                VP9,
            },
            new List<FFmpegAudioCodec>()
            {
                OPUS,
                VORBIS,
                MP3,
                AAC
            }
        );

        public static VideoCodecMap WEBM = new VideoCodecMap(VideoFormat.WEBM, 
            new List<FFmpegVideoCodec>()
            {
                VP9
            },
            new List<FFmpegAudioCodec>()
            {
                VORBIS,
                OPUS
            }
        );

        public static VideoCodecMap OGGVideo = new VideoCodecMap(VideoFormat.OGG, 
            new List<FFmpegVideoCodec>()
            {
                OGG
            },
            new List<FFmpegAudioCodec>()
            {
                VORBIS,
                OPUS
            }
        );

        public static VideoCodecMap GetMappedCodecs(string format)
        {
            switch (format.ToUpper())
            {
                case "FLV":
                    return FLV;
                case "MP4":
                    return MP4;
                case "MKV":
                    return MKV;
                case "WEBM":
                    return WEBM;
                case "OGG":
                    return OGGVideo;
                default:
                    return null;
            }
        }

        public static VideoFormat GetVideoFormatByExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case "mp4":
                    return VideoFormat.MP4;
                case "webm":
                    return VideoFormat.WEBM;
                case "flv":
                    return VideoFormat.FLV;
                case "mkv":
                    return VideoFormat.MKV;
                case "ogg":
                    return VideoFormat.OGG;
                case "gif":
                    return VideoFormat.GIF;
            }
            return VideoFormat.UNSPECIFIED;
        }

        public static AudioFormat GetAudioFormatByExtension(string extension)
        {
            switch(extension.ToLower())
            {
                case "mp3":
                    return AudioFormat.MP3;
                case "m4a":
                    return AudioFormat.M4A;
                case "aac":
                    return AudioFormat.AAC;
                case "ogg":
                    return AudioFormat.OGG;
                case "wav":
                    return AudioFormat.WAV;
                case "flac":
                    return AudioFormat.FLAC;
                case "opus":
                    return AudioFormat.OPUS;
                case "vorbis":
                    return AudioFormat.VORBIS;
            }
            return AudioFormat.UNSPECIFIED;
        }

        public static VideoFormat GetBestFormat(SystemVideoCodec codec)
        {
            switch (codec)
            {
                case SystemVideoCodec.AV1:
                    return VideoFormat.MP4;
                case SystemVideoCodec.H264:
                    return VideoFormat.MP4;
                case SystemVideoCodec.H265:
                    return VideoFormat.MP4;
                case SystemVideoCodec.MPEG4:
                    return VideoFormat.MP4;
                case SystemVideoCodec.THEORA:
                    return VideoFormat.OGG;
                case SystemVideoCodec.VP9:
                    return VideoFormat.WEBM;
                case SystemVideoCodec.RGB24:
                    return VideoFormat.GIF;
                case SystemVideoCodec.AVC1:
                    return VideoFormat.MP4;
                case SystemVideoCodec.MP4V:
                    return VideoFormat.MP4;
                default:
                    return VideoFormat.MP4;
            }
        }

        public static VideoFormat GetBestFormat(FFmpegVideoCodec codec)
        {
            switch (codec.Codec)
            {
                case SystemVideoCodec.AV1:
                    return VideoFormat.MP4;
                case SystemVideoCodec.H264:
                    return VideoFormat.MP4;
                case SystemVideoCodec.H265:
                    return VideoFormat.MP4;
                case SystemVideoCodec.MPEG4:
                    return VideoFormat.MP4;
                case SystemVideoCodec.THEORA:
                    return VideoFormat.OGG;
                case SystemVideoCodec.VP9:
                    return VideoFormat.WEBM;
                default:
                    return VideoFormat.MP4;
            }
        }

        public static VideoCodecMap GetMappedCodecs(VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.FLV:
                    return FLV;
                case VideoFormat.MKV:
                    return MKV;
                case VideoFormat.MP4:
                    return MP4;
                case VideoFormat.OGG:
                    return OGGVideo;
                case VideoFormat.WEBM:
                    return WEBM;
                default:
                    return null;
            }
        }

        public static FFmpegVideoCodec GetBestCodec(VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.FLV:
                    return H264;
                case VideoFormat.MKV:
                    return H264;
                case VideoFormat.MP4:
                    return H264;
                case VideoFormat.OGG:
                    return VP9;
                case VideoFormat.WEBM:
                    return VP9;
                default:
                    throw new Exception("Unsupported Format");
            }
        }

        public static FFmpegAudioCodec GetAudioCodec(AudioFormat audioFormat)
        {
            switch(audioFormat)
            {
                case AudioFormat.AAC:
                    return AAC;
                case AudioFormat.FLAC:
                    return FLAC;
                case AudioFormat.M4A:
                    return MP3;
                case AudioFormat.MP3:
                    return MP3;
                case AudioFormat.OGG:
                    return VORBIS;
                case AudioFormat.OPUS:
                    return OPUS;
                case AudioFormat.VORBIS:
                    return VORBIS;
                case AudioFormat.WAV:
                    return WAV;
                default:
                    return null;
            }
        }        
    }

    public class VideoCodecMap
    {
        public VideoFormat Format { get; set; }

        public List<FFmpegVideoCodec> FFmpegCodecs { get; set; }

        public List<FFmpegAudioCodec> FFmpegAudioCodecs { get; set; }

        public VideoCodecMap(VideoFormat format, List<FFmpegVideoCodec> videoCodecs, List<FFmpegAudioCodec> audioCodecs)
        {
            Format = format;
            FFmpegCodecs = videoCodecs;
            FFmpegAudioCodecs = audioCodecs;
        }

        public FFmpegVideoCodec BestVideo
        {
            get
            {
                switch (Format)
                {
                    case VideoFormat.FLV:
                        return SystemCodecMaps.H264;
                    case VideoFormat.MKV:
                        return SystemCodecMaps.H264;
                    case VideoFormat.MP4:
                        return SystemCodecMaps.H264;
                    case VideoFormat.OGG:
                        return SystemCodecMaps.VP9;
                    case VideoFormat.WEBM:
                        return SystemCodecMaps.VP9;
                    default:
                        throw new Exception("Unsupported Format");
                }
            }
        }

        public FFmpegAudioCodec BestAudio
        {
            get
            {
                switch (Format)
                {
                    case VideoFormat.FLV:
                        return SystemCodecMaps.MP3;
                    case VideoFormat.MKV:
                        return SystemCodecMaps.AAC;
                    case VideoFormat.MP4:
                        return SystemCodecMaps.MP3;
                    case VideoFormat.OGG:
                        return SystemCodecMaps.VORBIS;
                    case VideoFormat.WEBM:
                        return SystemCodecMaps.VORBIS;
                    case VideoFormat.GIF:
                        return null;
                    default:
                        throw new Exception("Unsupported Format");
                }
            }
        }
    }

    public class FFmpegVideoCodec
    {
        public SystemVideoCodec Codec { get; set; }

        public string Encoder { get; set; }

        public string Tags { get; set; }
        public string EncoderString { get { return $"{Encoder} {Tags}"; } }

        public FFmpegVideoCodec(SystemVideoCodec codec, string encoder, string tags = null)
        {
            Codec = codec;
            Encoder = encoder;
            Tags = tags;
        }
    }

    public class FFmpegAudioCodec
    {
        public AudioFormat Codec { get; set; }
        public string Encoder { get; set; }
        public string Tags { get; set; }
        public string EncoderString { get { return $"{Encoder} {Tags}"; } }

        public FFmpegAudioCodec(AudioFormat codec, string encoder, string tags = null)
        {
            Codec = codec;
            Encoder = encoder;
            Tags = tags;
        }
    }

    public enum SystemVideoCodec
    {
        AV1 = 0,
        H264 = 1,
        H265 = 3,
        VP9 = 4,
        MPEG4 = 5,
        THEORA = 6,
        RGB24 = 7,
        AVC1 = 8,
        MP4V = 9
    }
}
