using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YT_RED.Logging;
using YT_RED.Settings;

namespace YT_RED.Utils
{
    public static class TagUtil
    {
        /// <summary>
        /// The Temporary Directory to store downloaded album art.
        /// </summary>
        private static DirectoryInfo scratchDirectory
        {
            get
            {
                try
                {
                    FileInfo exeinfo = new FileInfo(Assembly.GetEntryAssembly().Location);
                    string scratchPath = Path.Combine(exeinfo.DirectoryName, "Temp");
                    DirectoryInfo scratch = new DirectoryInfo(scratchPath);
                    if (!scratch.Exists) scratch.Create();
                    return scratch;
                }
                catch(Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                }
                return null;
            }
        }

        /// <summary>
        /// Adds Specific ID3v2 Tags to Mp3 Files
        /// </summary>
        /// <param name="mp3Path"></param>
        /// <param name="artPath"></param>
        /// <param name="title"></param>
        /// <param name="album"></param>
        /// <param name="track"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> AddMp3Tags(string mp3Path, string artPath, string title = "", string album = "", int track = -1, int year = -1)
        {
            if (!File.Exists(mp3Path))
                throw new Exception($"The file {mp3Path} does not exist");
            try
            {
                TagLib.Id3v2.Tag.DefaultVersion = 3;
                TagLib.Id3v2.Tag.ForceDefaultVersion = true;
                TagLib.File file = TagLib.File.Create(mp3Path);

                string albumArt = await setID3v2AlbumArt(artPath, file);
                if (string.IsNullOrEmpty(albumArt)) return false;

                await Task.Run(() => File.Delete(albumArt));

                if (!string.IsNullOrEmpty(title))
                    file.Tag.Title = title;
                if (!string.IsNullOrEmpty(album))
                    file.Tag.Album = album;
                if (track > -1)
                    file.Tag.Track = (uint)track;
                if (year > -1)
                    file.Tag.Year = (uint)year;

                file.RemoveTags(file.TagTypes & ~file.TagTypesOnDisk);
                await Task.Run(() => file.Save());
                return true;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        public static async Task<bool> AddAlbumCover(string audioPath, string artPath)
        {
            if (!File.Exists(audioPath))
                throw new Exception($"The file \"{audioPath}\" does not exist");
            try
            {
                TagLib.Id3v2.Tag.DefaultVersion = 3;
                TagLib.Id3v2.Tag.ForceDefaultVersion = true;
                TagLib.File file = TagLib.File.Create(audioPath);

                string albumArt = await setGeneralAlbumArt(artPath, file);
                if (string.IsNullOrEmpty(albumArt)) return false;

                await Task.Run(() => File.Delete(albumArt));                

                file.RemoveTags(file.TagTypes & ~file.TagTypesOnDisk);
                await Task.Run(() => file.Save());
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        private static async Task<string> setGeneralAlbumArt(string imageUrl, TagLib.File file)
        {
            try
            {
                string path = Path.Combine(scratchDirectory.FullName, $"{Guid.NewGuid()}.jpg");                
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(imageUrl, path);
                }

                var cover = new TagLib.Picture(path);

                file.Tag.Pictures = new TagLib.IPicture[] { cover };
                return path;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        /// <summary>
        /// Sets the Album Art for the TagLib File
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static async Task<string> setID3v2AlbumArt(string imageUrl, TagLib.File file)
        {
            try
            {
                string path = Path.Combine(scratchDirectory.FullName, $"{Guid.NewGuid()}.jpg");
                byte[] imageBytes;
                using (WebClient client = new WebClient())
                {
                    imageBytes = await client.DownloadDataTaskAsync(imageUrl);
                }
                if (imageBytes == null || imageBytes.Length == 0) return null;

                TagLib.Id3v2.AttachmentFrame cover = new TagLib.Id3v2.AttachmentFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Cover",
                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                    Data = imageBytes,
                    TextEncoding = TagLib.StringType.UTF16
                };

                file.Tag.Pictures = new TagLib.IPicture[] { cover };
                return path;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }
    }
}
