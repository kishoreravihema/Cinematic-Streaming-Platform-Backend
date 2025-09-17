using Netflix_BackendAPI.Interface.IServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class FfmpegService : IFfmpegService
{
    private readonly string _ffmpegPath;

    public FfmpegService()
    {
        // Change this path to your local ffmpeg.exe
        _ffmpegPath = @"C:\Users\kr470\OneDrive\Desktop\Netflix app\KApp_NewDevelopment\ffmpeg-8.0-essentials_build\bin\ffmpeg.exe";
    }

    private string GetSafeAbsolutePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is empty.");

        string fullPath = Path.GetFullPath(path);

        if (!Path.IsPathRooted(fullPath))
            throw new UnauthorizedAccessException("Relative paths are not allowed.");

        return fullPath;
    }

    public async Task<string> GenerateHlsAsync(string inputFile, string outputFile)
    {
        string inputPath = GetSafeAbsolutePath(inputFile);
        string outputPath = GetSafeAbsolutePath(outputFile);

        var args = $"-i \"{inputPath}\" " +
                   "-map 0:v -map 0:a:0 -c:v copy -c:a aac -f hls -var_stream_map \"v:0,a:0\" " +
                   $"\"{outputPath}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            Arguments = args,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new Exception($"FFmpeg failed: {error}");

        return outputPath;
    }
}
