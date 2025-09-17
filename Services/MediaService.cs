using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Interface.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _repo;

    public MediaService(IMediaRepository repo)
    {
        _repo = repo;
    }

    public async Task<BaseResponse<IEnumerable<AlbumDto>>> GetAlbumsAsync()
    {
        try
        {
            var albums = await _repo.GetAllAlbumsAsync();
            return new BaseResponse<IEnumerable<AlbumDto>>
            {
                Success = true,
                Data = albums,
                Message = "Albums fetched successfully.",
                Code = ResponseCode.Success
            };
        }
        catch (Exception ex)
        {
            LogExceptionHandler.LogException(ex, nameof(GetAlbumsAsync));
            return new BaseResponse<IEnumerable<AlbumDto>>
            {
                Success = false,
                Message = "An error occurred while fetching albums.",
                Errors = new List<string> { ex.Message },
                Code = ResponseCode.ServerError
            };
        }
    }

    public async Task<BaseResponse<IEnumerable<ArtistDto>>> GetArtistsAsync()
    {
        try
        {
            var artists = await _repo.GetAllArtistsAsync();
            return new BaseResponse<IEnumerable<ArtistDto>>
            {
                Success = true,
                Data = artists,
                Message = "Artists fetched successfully.",
                Code = ResponseCode.Success
            };
        }
        catch (Exception ex)
        {
            LogExceptionHandler.LogException(ex, nameof(GetArtistsAsync));
            return new BaseResponse<IEnumerable<ArtistDto>>
            {
                Success = false,
                Message = "An error occurred while fetching artists.",
                Errors = new List<string> { ex.Message },
                Code = ResponseCode.ServerError
            };
        }
    }

    public async Task<BaseResponse<IEnumerable<GenreDto>>> GetGenresAsync()
    {
        try
        {
            var genres = await _repo.GetAllGenresAsync();
            return new BaseResponse<IEnumerable<GenreDto>>
            {
                Success = true,
                Data = genres,
                Message = "Genres fetched successfully.",
                Code = ResponseCode.Success
            };
        }
        catch (Exception ex)
        {
            LogExceptionHandler.LogException(ex, nameof(GetGenresAsync));
            return new BaseResponse<IEnumerable<GenreDto>>
            {
                Success = false,
                Message = "An error occurred while fetching genres.",
                Errors = new List<string> { ex.Message },
                Code = ResponseCode.ServerError
            };
        }
    }

    public async Task<BaseResponse<IEnumerable<PlaylistMusicDto>>> GetPlaylistMusicAsync()
    {
        try
        {
            var playlistMusic = await _repo.GetAllPlaylistMusicAsync();
            return new BaseResponse<IEnumerable<PlaylistMusicDto>>
            {
                Success = true,
                Data = playlistMusic,
                Message = "Playlist music fetched successfully.",
                Code = ResponseCode.Success
            };
        }
        catch (Exception ex)
        {
            LogExceptionHandler.LogException(ex, nameof(GetPlaylistMusicAsync));
            return new BaseResponse<IEnumerable<PlaylistMusicDto>>
            {
                Success = false,
                Message = "An error occurred while fetching playlist music.",
                Errors = new List<string> { ex.Message },
                Code = ResponseCode.ServerError
            };
        }
    }

    public async Task<BaseResponse<IEnumerable<PlaylistVideoDto>>> GetPlaylistVideosAsync()
    {
        try
        {
            var playlistVideos = await _repo.GetAllPlaylistVideosAsync();
            return new BaseResponse<IEnumerable<PlaylistVideoDto>>
            {
                Success = true,
                Data = playlistVideos,
                Message = "Playlist videos fetched successfully.",
                Code = ResponseCode.Success
            };
        }
        catch (Exception ex)
        {
            LogExceptionHandler.LogException(ex, nameof(GetPlaylistVideosAsync));
            return new BaseResponse<IEnumerable<PlaylistVideoDto>>
            {
                Success = false,
                Message = "An error occurred while fetching playlist videos.",
                Errors = new List<string> { ex.Message },
                Code = ResponseCode.ServerError
            };
        }
    }
}
