using System.Diagnostics;

namespace Netflix_BackendAPI.Helper
{
    public static class ThumbnailHelper
    {
        public static async Task<bool> GenerateThumbnailAsync(string videoPath, string thumbnailPath)
        {
            var ffmpegPath = "C:\\Users\\kr470\\OneDrive\\Desktop\\Netflix app\\KApp_NewDevelopment\\Thumnail Image\\ffmpeg-7.1.1\\ffmpeg-2025-07-31-git-119d127d05-essentials_build\\ffmpeg-2025-07-31-git-119d127d05-essentials_build\\bin\\ffmpeg.exe";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-ss 00:00:05 -i \"{videoPath}\" -frames:v 1 -q:v 2 \"{thumbnailPath}\" -y",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            await process.WaitForExitAsync();
            return File.Exists(thumbnailPath);
        }

    }

}
