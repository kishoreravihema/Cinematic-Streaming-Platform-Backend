using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IServices
{
    public interface IPlaylistService
    {
        Task<BaseResponse<Playlist>> CreatePlaylistAsync(PlaylistDto dto);
        Task<BaseResponse<string>> AddMusicToPlaylistAsync(int playlistId, int musicId);
        Task<BaseResponse<string>> AddVideoToPlaylistAsync(int playlistId, int videoId);
        Task<BaseResponse<List<PlaylistDto>>> GetAllPlaylistsWithMediaAsync();
    }
}
