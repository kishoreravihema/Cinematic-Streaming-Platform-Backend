using Netflix_BackendAPI.DTO;

namespace Netflix_BackendAPI.Interface.IServices
{
    public interface IFileService
    {
        Task<BaseResponse<string>> UploadFileAsync(IFormFile file);
    }
}
