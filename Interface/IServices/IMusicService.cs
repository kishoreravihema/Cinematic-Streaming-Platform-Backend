using Microsoft.AspNetCore.Mvc;
using Netflix_BackendAPI.DTO;

namespace Netflix_BackendAPI.Interface.IServices
{
    public interface IMusicService
    {
        Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetAllAsync(int page, int pageSize);
        Task<BaseResponse<MusicResponseDto>> GetByIdAsync(int id);
        Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetByAlbumAsync(int albumId);
        Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetByGenreAsync(string genre);
        Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetByArtistAsync(int artistId);
        Task<BaseResponse<MusicResponseDto>> AddAsync(MusicDto dto);
        Task<BaseResponse<MusicResponseDto>> AddManuallyAsync(MusicResponseDto dto);
        Task<BaseResponse<MusicResponseDto>> UpdateAsync(int id, MusicDto dto);
        Task<BaseResponse<bool>> DeleteAsync(int id);
        Task<BaseResponse<bool>> ExistsAsync(int id);
        //Task<BaseResponse<FileStreamResult>> StreamMusicAsync(int id);
        //Task<BaseResponse<IActionResult>> StreamMusicAsync(int id);
        Task<BaseResponse<PlayInfo>> GetStreamInfoAsync(int id, IUrlHelper urlHelper);
        Task<(FileStream? Stream, string? MimeType, BaseResponse<string>? Error)> StreamAudioAsync(int id);



    }
}
