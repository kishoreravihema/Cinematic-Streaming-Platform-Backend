namespace Netflix_BackendAPI.Interface.IServices
{
    public interface IFfmpegService
    {
        Task<string> GenerateHlsAsync(string inputFile, string outputFile);
    }
}
