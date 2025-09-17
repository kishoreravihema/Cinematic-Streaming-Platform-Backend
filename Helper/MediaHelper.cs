using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

namespace Netflix_BackendAPI.Helper
{
    public static class MediaHelper
    {
        //A single dicttionary for all MIME types
        private static readonly Dictionary<string, string> MimeTypes = new()
        {
            // Audio
            { ".mp3", "audio/mpeg" }, { ".wav", "audio/wav" }, { ".ogg", "audio/ogg" },
            { ".flac", "audio/flac" }, { ".aac", "audio/aac" }, { ".m4a", "audio/mp4" },
            { ".webm", "audio/webm" }, { ".wma", "audio/x-ms-wma" }, { ".aiff", "audio/x-aiff" },
            { ".mka", "audio/x-matroska" },
            // Video
            { ".mp4", "video/mp4" }, /* .webm is already present */ { ".mkv", "video/x-matroska" },
            { ".ogv", "video/ogg" }, { ".avi", "video/x-msvideo" }, { ".wmv", "video/x-ms-wmv" },
            { ".mov", "video/quicktime" }, { ".flv", "video/x-flv" }, { ".3gp", "video/3gpp" }
        };
        private static readonly HashSet<string> SupportedAudioTypes = new()
        {
            "audio/mpeg", "audio/wav", "audio/ogg", "audio/flac", "audio/aac",
            "audio/mp4", "audio/webm", "audio/x-ms-wma", "audio/x-aiff", "audio/x-matroska"
        };

        private static readonly HashSet<string> SupportedVideoTypes = new()
        {
            "video/mp4", "video/webm", "video/x-matroska", "video/ogg", "video/x-msvideo",
            "video/x-ms-wmv", "video/quicktime", "video/x-flv", "video/3gpp"
        };
        public static string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath)?.ToLowerInvariant();
            return MimeTypes.TryGetValue(extension ?? "", out var mimeType) ? mimeType : "application/octet-stream";
        }
        public static bool IsYouTubeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   (uri.Host.Contains("youtube.com") || uri.Host.Contains("youtu.be"));
        }

        public static string? ExtractYouTubeId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            // Check for video URL
            var videoMatch = Regex.Match(url, @"^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]{11}).*");
            return (videoMatch.Success && videoMatch.Groups[2].Length == 11) ? videoMatch.Groups[2].Value : null;
        }
        public static string? ExtractYouTubePlaylistId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            { 
                return null;
            }

            var match = Regex.Match(url, @"[?&]list=([^#&]+)");
            return match.Success ? match.Groups[1].Value : null;
        }
        public static bool IsYouTubeChannelPlaylistPage(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Regex.IsMatch(url, @"^https:\/\/www\.youtube\.com\/@[^\/]+\/playlists$");
        }



        public static bool IsSupportedAudioMimeType(string mimeType) =>
            !string.IsNullOrWhiteSpace(mimeType) && SupportedAudioTypes.Contains(mimeType);

        public static bool IsSupportedVideoMimeType(string mimeType) =>
            !string.IsNullOrWhiteSpace(mimeType) && SupportedVideoTypes.Contains(mimeType);

        public static FileExtensionContentTypeProvider GetCustomMimeProvider()
        {
            var provider = new FileExtensionContentTypeProvider();

            // Audio formats
            provider.Mappings[".m4a"] = "audio/mp4";
            provider.Mappings[".mp3"] = "audio/mpeg";
            provider.Mappings[".wav"] = "audio/wav";
            provider.Mappings[".ogg"] = "audio/ogg";
            provider.Mappings[".flac"] = "audio/flac";
            provider.Mappings[".aac"] = "audio/aac";
            provider.Mappings[".webm"] = "audio/webm";

            // Video formats
            provider.Mappings[".mp4"] = "video/mp4";
            provider.Mappings[".mkv"] = "video/x-matroska";
            provider.Mappings[".webm"] = "video/webm";
            provider.Mappings[".ogv"] = "video/ogg";
            provider.Mappings[".avi"] = "video/x-msvideo";
            provider.Mappings[".wmv"] = "video/x-ms-wmv";
            provider.Mappings[".mov"] = "video/quicktime";
            provider.Mappings[".flv"] = "video/x-flv";
            provider.Mappings[".3gp"] = "video/3gpp";

            return provider;
        }
    }
}

