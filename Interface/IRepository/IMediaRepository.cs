using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IRepository
{
    public interface IMediaRepository
    {
        Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync();
        Task<IEnumerable<ArtistDto>> GetAllArtistsAsync();
        Task<IEnumerable<GenreDto>> GetAllGenresAsync();
        Task<IEnumerable<PlaylistMusicDto>> GetAllPlaylistMusicAsync();
        Task<IEnumerable<PlaylistVideoDto>> GetAllPlaylistVideosAsync();
    }

}
