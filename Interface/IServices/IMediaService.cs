using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IServices
{
    public interface IMediaService
    {
        Task<BaseResponse<IEnumerable<AlbumDto>>> GetAlbumsAsync();
        Task<BaseResponse<IEnumerable<ArtistDto>>> GetArtistsAsync();
        Task<BaseResponse<IEnumerable<GenreDto>>> GetGenresAsync();
        Task<BaseResponse<IEnumerable<PlaylistMusicDto>>> GetPlaylistMusicAsync();
        Task<BaseResponse<IEnumerable<PlaylistVideoDto>>> GetPlaylistVideosAsync();


    }
}
