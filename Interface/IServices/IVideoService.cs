using Microsoft.AspNetCore.Mvc;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IServices
{
    public interface IVideoService
    {
        Task<BaseResponse<IEnumerable<VideoResponseDto>>> GetAllVideosAsync(int page, int pageSize);
        Task<BaseResponse<Video>> GetVideoByIdAsync(int id);
        Task<BaseResponse<IEnumerable<Video>>> GetVideosByPlaylistIdAsync(int playlistId);
        Task<BaseResponse<IEnumerable<VideoResponseDto>>> GetVideosByCategoryAsync(string categoryName);
        Task<BaseResponse<VideoResponseDto>> AddVideoAsync(VideoDto dto);
        Task<BaseResponse<VideoResponseDto>> AddVideoWithManualIdAsync(int id, VideoDto dto);
        Task<BaseResponse<VideoResponseDto>> UpdateVideoAsync(VideoResponseDto dto);
        Task<BaseResponse<VideoResponseDto>> UpdateVideoByIdAsync(int id, VideoDto dto);
        Task<BaseResponse<bool>> DeleteVideoAsync(int id);
        Task<BaseResponse<bool>> VideoExistsAsync(int id);
        //Task<BaseResponse<FileStreamResult>> StreamVideoAsync(int id);
        // Task<(FileStreamResult? Stream, BaseResponse<string>? Error)> StreamVideoAsync(int id);
        Task<BaseResponse<PlayInfo>> GetVideoPlaybackInfoAsync(int id, IUrlHelper urlHelper);
        Task<BaseResponse<object>> StreamVideoAsyncOld(int id);
        Task<(Stream? stream, string? mimeType, string? localPath, BaseResponse<object>? response)> StreamVideoAsync(int id);
        Task<(string PlaylistPath, string DirectoryPath)> GetOrCreateHlsPlaylistAsync(int id);
    }
}
