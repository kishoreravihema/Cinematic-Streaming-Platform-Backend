using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IRepository
{
    public interface IPlaylistRepository
    {
        Task<List<PlaylistDto>> GetAllPlaylistsWithMediaAsync();
        Task<bool> PlaylistExistsForUserAsync(int userId);
        Task<Playlist?> GetByIdAsync(int playlistId);
        Task<bool> MusicExistsInPlaylistAsync(int playlistId, int musicId);
        Task<bool> VideoExistsInPlaylistAsync(int playlistId, int videoId);
        Task AddMusicAsync(PlaylistMusic entry);
        Task AddVideoAsync(PlaylistVideo entry);
        Task AddAsync(Playlist playlist);
    }
}
