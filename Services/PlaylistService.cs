using Microsoft.EntityFrameworkCore;
using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IRepository;

using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Models;
using Netflix_BackendAPI.Repository;


namespace Netflix_BackendAPI.Service
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepo;
        private readonly IMusicRepository _musicRepo;
        private readonly IVideoRepository _videoRepo;
       
        private readonly IUnitOfWork _unitOfWork;

        public PlaylistService(
            IPlaylistRepository playlistRepo,
            IMusicRepository musicRepo,
            IVideoRepository videoRepo,
        
            IUnitOfWork unitOfWork)
        {
            _playlistRepo = playlistRepo;
            _musicRepo = musicRepo;
            _videoRepo = videoRepo;
           
            _unitOfWork = unitOfWork;
        }

        // 🎶 Create Playlist
        public async Task<BaseResponse<Playlist>> CreatePlaylistAsync(PlaylistDto dto)
        {
            try
            {
                var userExists = await _playlistRepo.PlaylistExistsForUserAsync(dto.UserId);
                if (!userExists)
                {
                    return new BaseResponse<Playlist>
                    {
                        Success = false,
                        Message = $"User with ID {dto.UserId} does not exist.",
                        Code = ResponseCode.BadRequest,
                        Errors = new List<string> { "Invalid UserId" }
                    };
                }

                var playlist = new Playlist
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    UserId = dto.UserId,
                    IsPublic = true,
                    CoverImageUrl = dto.CoverImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Tags = dto.Tags ?? new List<string>(),
                };

                await _playlistRepo.AddAsync(playlist);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse<Playlist>
                {
                    Success = true,
                    Message = "Playlist created successfully.",
                    Code = ResponseCode.Created,
                    Data = playlist
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(CreatePlaylistAsync));
                return new BaseResponse<Playlist>
                {
                    Success = false,
                    Message = "An error occurred while creating playlist.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // ➕ Add Music to Playlist
        public async Task<BaseResponse<string>> AddMusicToPlaylistAsync(int playlistId, int musicId)
        {
            try
            {
                var playlist = await _playlistRepo.GetByIdAsync(playlistId);
                var music = await _musicRepo.GetByIdAsync(musicId);

                if (playlist == null || music == null)
                {
                    return new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Playlist or Music not found.",
                        Code = ResponseCode.NotFound,
                        Errors = new List<string> { "Invalid PlaylistId or MusicId" }
                    };
                }

                var exists = await _playlistRepo.MusicExistsInPlaylistAsync(playlistId, musicId);
                if (exists)
                {
                    return new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Music already exists in playlist.",
                        Code = ResponseCode.Conflict
                    };
                }

                var entry = new PlaylistMusic
                {
                    PlaylistId = playlistId,
                    MusicId = musicId
                };

                await _playlistRepo.AddMusicAsync(entry);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse<string>
                {
                    Success = true,
                    Message = "Music added to playlist.",
                    Code = ResponseCode.Created,
                    Data = "Success"
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(AddMusicToPlaylistAsync));
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while adding music to playlist.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // ➕ Add Video to Playlist
        public async Task<BaseResponse<string>> AddVideoToPlaylistAsync(int playlistId, int videoId)
        {
            try
            {
                var playlist = await _playlistRepo.GetByIdAsync(playlistId);
                var video = await _videoRepo.GetVideoByIdAsync(videoId);

                if (playlist == null || video == null)
                {
                    return new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Playlist or Video not found.",
                        Code = ResponseCode.NotFound,
                        Errors = new List<string> { "Invalid PlaylistId or VideoId" }
                    };
                }

                var exists = await _playlistRepo.VideoExistsInPlaylistAsync(playlistId, videoId);
                if (exists)
                {
                    return new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Video already exists in playlist.",
                        Code = ResponseCode.Conflict
                    };
                }

                var entry = new PlaylistVideo
                {
                    PlaylistId = playlistId,
                    VideoId = videoId
                };

                await _playlistRepo.AddVideoAsync(entry);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse<string>
                {
                    Success = true,
                    Message = "Video added to playlist.",
                    Code = ResponseCode.Created,
                    Data = "Success"
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(AddVideoToPlaylistAsync));
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while adding video to playlist.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<List<PlaylistDto>>> GetAllPlaylistsWithMediaAsync()
        {
            try
            {
                var playlists = await _playlistRepo.GetAllPlaylistsWithMediaAsync(); // returns List<PlaylistDto>

                return new BaseResponse<List<PlaylistDto>>
                {
                    Success = true,
                    Data = playlists,
                    Code = ResponseCode.Success,
                    Message = "Playlists with media fetched successfully"
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetAllPlaylistsWithMediaAsync));
                return new BaseResponse<List<PlaylistDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching playlists.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

    }
}
