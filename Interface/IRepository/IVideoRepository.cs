using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IRepository
{
    public interface IVideoRepository
    {
         
        Task<IEnumerable<Video>> GetAllVideosAsync();
        Task<IEnumerable<Video>> GetPagedVideosAsync(int skip, int take);
        Task<Video?> GetVideoByIdAsync(int id);
        Task<IEnumerable<Video>> GetVideosByPlaylistIdAsync(int playlistId);
        Task<Video?> AddVideoAsync(Video video);
        Task<List<Actor>> GetOrCreateActorsAsync(List<ActorResponseDto> actorDtos);
        Task<Video?> AddVideoWithManualIdAsync(Video video);
        Task<Video?> UpdateVideoAsync(Video video);
        Task<bool> DeleteVideoAsync(int id);
        Task<bool> VideoExistsAsync(int id);
        Task<IEnumerable<Video>> GetVideosByCategoryAsync(string categoryName);


    }
}
