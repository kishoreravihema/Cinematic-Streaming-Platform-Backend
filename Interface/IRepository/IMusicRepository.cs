using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Interface.IRepository
{
    public interface IMusicRepository
    {
        Task<IEnumerable<Music>> GetAllAsync();
        Task<Music?> GetByIdAsync(int id);
        Task<IEnumerable<Music>> GetByAlbumAsync(int albumId);
        Task<IEnumerable<Music>> GetByGenreAsync(string genre);
        Task<IEnumerable<Music>> GetByArtistAsync(int artistId);
        Task<Music?> AddAsync(Music music);
        Task<Music?> AddWithManualIdAsync(Music music);
        Task<Music?> UpdateAsync(Music music);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<Artist>> GetArtistsByIdsAsync(List<int> ids);
        Task<IEnumerable<Music>> GetPagedAsync(int skip, int take);
        Task<bool> GenreExistsAsync(int genreId);
        Task<List<Artist>> GetOrCreateArtistsAsync(List<ArtistResponseDto> artistDtos);
        Task<bool> AlbumExistsAsync(int albumId);
        Task<bool> UserExistsAsync(int userId);

    }
}
