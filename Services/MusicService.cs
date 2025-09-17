using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Helper;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Models;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Netflix_BackendAPI.Services
{
    public class MusicService : IMusicService
    {
        private readonly IMusicRepository _repo;

        private readonly ILogger<MusicService> _logger;

        private readonly string? _safeRootDirectory;

        private readonly IHttpClientFactory _httpClientFactory;


        public MusicService(IMusicRepository repo, ILogger<MusicService> logger, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _logger = logger;
            _safeRootDirectory = config["FileStreaming:MusicRootDirectory"] ?? string.Empty;
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        public async Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetAllAsync(int page, int pageSize)
        {
            try
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 10;

                int skip = (page - 1) * pageSize;
                var data = await _repo.GetPagedAsync(skip, pageSize);
                var dto = data.Select(MapToDto).ToList();

                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = true,
                    Data = dto,
                    Message = $"Fetched page {page} with {dto.Count} music tracks.",
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetAllAsync));
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching music.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<MusicResponseDto?>> GetByIdAsync(int id)
        {
            try
            {
                var data = await _repo.GetByIdAsync(id);
                if (data == null)
                {
                    return new BaseResponse<MusicResponseDto?>
                    {
                        Success = false,
                        Message = "Music not found.",
                        Code = ResponseCode.NotFound
                    };
                }
                var dto = MapToDto(data);
                return new BaseResponse<MusicResponseDto?>
                {
                    Success = true,
                    Data = dto,
                    Message = "Music retrieved successfully.",
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetByIdAsync));
                return new BaseResponse<MusicResponseDto?>
                {
                    Success = false,
                    Message = "An error occurred while fetching music.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetByAlbumAsync(int albumId)
        {
            try
            {
                var data = await _repo.GetByAlbumAsync(albumId);
                var dto = data.Select(MapToDto).ToList();
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = true,
                    Data = dto,
                    Message = "Fetched Music by Album successfully.",
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetByAlbumAsync));
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching music by album.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetByGenreAsync(string genre)
        {
            try
            {
                var data = await _repo.GetByGenreAsync(genre);
                var dto = data.Select(MapToDto).ToList();
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = true,
                    Data = dto,
                    Message = "Fetched Music by Genre successfully.",
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetByGenreAsync));
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching music by genre.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<MusicResponseDto>>> GetByArtistAsync(int artistId)
        {
            try
            {
                var data = await _repo.GetByArtistAsync(artistId);
                var dto = data.Select(MapToDto).ToList();
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = true,
                    Data = dto,
                    Message = "Fetched Music by Artist successfully.",
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetByArtistAsync));
                return new BaseResponse<IEnumerable<MusicResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching music by artist.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<MusicResponseDto>> AddAsync(MusicDto dto)
        {
            try
            {
                if (!await _repo.GenreExistsAsync(dto.GenreId))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"Genre with Id {dto.GenreId} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }
                if (!await _repo.AlbumExistsAsync(dto.AlbumId))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"Album with Id {dto.AlbumId} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }
                if (dto.UserId.HasValue && !await _repo.UserExistsAsync(dto.UserId.Value))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"User with Id {dto.UserId.Value} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }


                    var artists = await ResolveArtistsAsync(dto);

                var music = new Music
                {
                    Title = string.IsNullOrWhiteSpace(dto.Title) ? "Untitled" : dto.Title,
                    Url = dto.Url ?? string.Empty,
                    Description = dto.Description ?? string.Empty,
                    Lyrics = dto.Lyrics ?? string.Empty,
                    ThumbnailUrl = dto.ThumbnailUrl ?? string.Empty,
                    Language = dto.Language ?? string.Empty,
                    DurationInSeconds = dto.DurationInSeconds ?? 0,
                    IsExplicit = dto.IsExplicit,
                    IsPremium = dto.IsPremium,
                    PlayCount = dto.PlayCount == 0 ? 0 : dto.PlayCount,
                    ReleaseDate = dto.ReleaseDate == default ? DateTime.UtcNow : dto.ReleaseDate,
                    CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,

                    AlbumId = dto.AlbumId,
                    GenreId = dto.GenreId,
                    UserId = dto.UserId,
                    Artists = artists
                };


                var created = await _repo.AddAsync(music);
                if (created == null)
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = "Failed to add song",
                        Code = ResponseCode.BadRequest
                    };
                }

                return new BaseResponse<MusicResponseDto>
                {
                    Success = true,
                    Data = MapToDto(created),
                    Message = "Song added",
                    Code = ResponseCode.Created
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(AddAsync));
                return new BaseResponse<MusicResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while adding music.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<MusicResponseDto>> AddManuallyAsync(MusicResponseDto dto)
        {
            try
            {
                if (!await _repo.GenreExistsAsync(dto.GenreId))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"Genre with Id {dto.GenreId} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }

                var artists = await ResolveArtistsAsync(dto);

                var music = new Music
                {
                    //Id = dto.Id,
                    Title = dto.Title,
                    Url = dto.Url,
                    Description = dto.Description,
                    AlbumId = dto.AlbumId,
                    GenreId = dto.GenreId,
                    Language = dto.Language,
                    IsPremium = dto.IsPremium,
                    ReleaseDate = dto.ReleaseDate,
                    CreatedAt = dto.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    Artists = artists
                };

                var created = await _repo.AddWithManualIdAsync(music);
                if (created == null)
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = "Failed to add song with manual ID.",
                        Code = ResponseCode.BadRequest
                    };
                }

                return new BaseResponse<MusicResponseDto>
                {
                    Success = true,
                    Data = MapToDto(created),
                    Message = "Song added successfully.",
                    Code = ResponseCode.Created
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(AddManuallyAsync));
                return new BaseResponse<MusicResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while adding music.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<MusicResponseDto>> UpdateAsync(int id, MusicDto dto)
        {
            try
            {
                if (!await _repo.GenreExistsAsync(dto.GenreId))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"Genre with Id {dto.GenreId} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }

                if (!await _repo.AlbumExistsAsync(dto.AlbumId))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"Album with Id {dto.AlbumId} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }
                if (dto.UserId.HasValue && !await _repo.UserExistsAsync(dto.UserId.Value))
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = $"User with Id {dto.UserId.Value} does not exist.",
                        Code = ResponseCode.BadRequest
                    };
                }


                var artists = await ResolveArtistsAsync(dto) ?? new List<Artist>();

                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = "Music track not found.",
                        Code = ResponseCode.NotFound
                    };
                }

                existing.Title = dto.Title;
                existing.Url = dto.Url;
                existing.Description = dto.Description ?? string.Empty;
                existing.Lyrics = dto.Lyrics ?? string.Empty;
                existing.ThumbnailUrl = dto.ThumbnailUrl ?? string.Empty;
                existing.Language = dto.Language ?? string.Empty;
                existing.DurationInSeconds = dto.DurationInSeconds ?? 0;
                existing.IsExplicit = dto.IsExplicit;
                existing.IsPremium = dto.IsPremium;
                existing.PlayCount = dto.PlayCount;
                existing.ReleaseDate = dto.ReleaseDate == default ? DateTime.UtcNow : dto.ReleaseDate;
                existing.UpdatedAt = DateTime.UtcNow;

                existing.AlbumId = dto.AlbumId;
                existing.GenreId = dto.GenreId;
                existing.UserId = dto.UserId;
                existing.Artists = artists;
                //var musicToUpdate = new Music
                //{
                //    Id = id,
                //    Title = dto.Title,
                //    Url = dto.Url,
                //    Description = dto.Description,
                //    Lyrics = dto.Lyrics,
                //    ThumbnailUrl = dto.ThumbnailUrl,
                //    Language = dto.Language,
                //    DurationInSeconds = dto.DurationInSeconds ?? 0,
                //    IsExplicit = dto.IsExplicit,
                //    IsPremium = dto.IsPremium,
                //    PlayCount = dto.PlayCount,
                //    ReleaseDate = dto.ReleaseDate,
                //    UpdatedAt = DateTime.UtcNow,

                //    // Foreign Keys
                //    AlbumId = dto.AlbumId,
                //    GenreId = dto.GenreId,
                //    UserId = dto.UserId,

                //    Artists = artists
                //};

                var updated = await _repo.UpdateAsync(existing);
                if (updated == null)
                {
                    return new BaseResponse<MusicResponseDto>
                    {
                        Success = false,
                        Message = "Music track not found.",
                        Code = ResponseCode.NotFound
                    };
                }

                return new BaseResponse<MusicResponseDto>
                {
                    Success = true,
                    Message = "Music track updated successfully.",
                    Data = MapToDto(updated),
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(UpdateAsync));
                return new BaseResponse<MusicResponseDto>
                {
                    Success = false,
                    Message = "Failed to update music track.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var exists = await _repo.ExistsAsync(id);
                if (!exists)
                {
                    return new BaseResponse<bool>
                    {
                        Success = false,
                        Message = "Music track not found.",
                        Data = false,
                        Code = ResponseCode.NotFound
                    };
                }

                var deleted = await _repo.DeleteAsync(id);
                return new BaseResponse<bool>
                {
                    Success = deleted,
                    Message = deleted ? "Music track deleted." : "Delete failed.",
                    Data = deleted,
                    Code = deleted ? ResponseCode.NoContent : ResponseCode.ServerError
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(DeleteAsync));
                return new BaseResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting music track.",
                    Errors = new List<string> { ex.Message },
                    Data = false,
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await _repo.ExistsAsync(id);
                return new BaseResponse<bool>
                {
                    Success = true,
                    Message = "Existence check completed.",
                    Data = exists,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(ExistsAsync));
                return new BaseResponse<bool>
                {
                    Success = false,
                    Message = "Error checking existence.",
                    Errors = new List<string> { ex.Message },
                    Data = false,
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<PlayInfo>> GetStreamInfoAsync(int id, IUrlHelper urlHelper)
        {
            try
            {
                var music = await _repo.GetByIdAsync(id);
                if (music == null || string.IsNullOrWhiteSpace(music.Url))
                {
                    return new BaseResponse<PlayInfo>
                    {
                        Success = false,
                        Message = "Music not found.",
                        Code = ResponseCode.NotFound
                    };
                }

                if (MediaHelper.IsYouTubeUrl(music.Url))
                {
                    var videoId = MediaHelper.ExtractYouTubeId(music.Url);
                    if (string.IsNullOrWhiteSpace(videoId))
                    {
                        return new BaseResponse<PlayInfo>
                        {
                            Success = false,
                            Message = "Invalid YouTube URL format.",
                            Code = ResponseCode.BadRequest
                        };
                    }

                    return new BaseResponse<PlayInfo>
                    {
                        Success = true,
                        Message = "YouTube video ID for embedding",
                        Data = new PlayInfo
                        {
                            PlaybackType = "YOUTUBE",
                            Url = videoId // For YouTube, this is the video ID
                        }
                    };
                }

                if (Uri.TryCreate(music.Url, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    var mimeType = await GetRemoteMimeTypeAsync(music.Url);
                    if (mimeType != null && MediaHelper.IsSupportedAudioMimeType(mimeType))
                    {
                        return new BaseResponse<PlayInfo>
                        {
                            Success = true,
                            Message = "Remote audio URL",
                            Data = new PlayInfo
                            {
                                PlaybackType = "REMOTE_URL",
                                Url = music.Url
                            }
                        };
                    }

                    return new BaseResponse<PlayInfo>
                    {
                        Success = false,
                        Message = "Remote URL is not a supported audio format.",
                        Code = ResponseCode.UnsupportedMediaType
                    };
                }

              
                var streamEndpoint = urlHelper.Action(
                    action: "StreamAudio",
                    controller: "Features", // Make sure this matches your actual controller name
                    values: new { id = id }
                );

                return new BaseResponse<PlayInfo>
                {
                    Success = true,
                    Message = "Local file available",
                    Data = new PlayInfo
                    {
                        PlaybackType = "LOCAL_STREAM",
                        Url = streamEndpoint // 🔥 CORRECTED LOGIC IS HERE
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stream info for music ID {Id}", id);
                LogExceptionHandler.LogException(ex, nameof(GetStreamInfoAsync));
                return new BaseResponse<PlayInfo>
                {
                    Success = false,
                    Message = "An internal server error occurred.",
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<(FileStream? Stream, string? MimeType, BaseResponse<string>? Error)> StreamAudioAsync(int id)
        {
            try
            {
                var music = await _repo.GetByIdAsync(id);
                if (music == null || string.IsNullOrWhiteSpace(music.Url) || music.Url.StartsWith("http"))
                {
                    var error = new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Music not found or is not a local file.",
                        Code = ResponseCode.NotFound
                    };
                    return (null, null, error);
                }

                if (string.IsNullOrWhiteSpace(_safeRootDirectory))
                {
                    var error = new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Safe root directory is not configured.",
                        Code = ResponseCode.ServerError
                    };
                    return (null, null, error);
                }

                var localPath = Path.Combine(_safeRootDirectory, music.Url.TrimStart('\\', '/'));

                if (!System.IO.File.Exists(localPath))
                {
                    _logger.LogWarning("Audio file not found at physical path: {Path}", localPath);
                    var error = new BaseResponse<string>
                    {
                        Success = false,
                        Message = "File not found on server.",
                        Code = ResponseCode.NotFound
                    };
                    return (null, null, error);
                }

                var mime = MediaHelper.GetMimeType(localPath);
                if (!MediaHelper.IsSupportedAudioMimeType(mime))
                {
                    var error = new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Unsupported audio format.",
                        Code = ResponseCode.UnsupportedMediaType
                    };
                    return (null, null, error);
                }

                var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                return (stream, mime, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error streaming audio for music ID {Id}", id);
                LogExceptionHandler.LogException(ex, nameof(StreamAudioAsync));
                var error = new BaseResponse<string>
                {
                    Success = false,
                    Message = "An internal error occurred while streaming.",
                    Code = ResponseCode.ServerError
                };
                return (null, null, error);
            }
        }

        // ==============================================================================
        // == HELPER METHODS (Used by the active implementation) ========================
        // ==============================================================================

        private async Task<string?> GetRemoteMimeTypeAsync(string url)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MusicStreamBot/1.0)");
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                return response.IsSuccessStatusCode ? response.Content.Headers.ContentType?.MediaType : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to detect remote MIME type for URL: {Url}", url);
                return null;
            }
        }

        private bool IsSafePath(string path)
        {
            if (string.IsNullOrWhiteSpace(_safeRootDirectory)) return true;
            var fullPath = Path.GetFullPath(path);
            var rootPath = Path.GetFullPath(_safeRootDirectory);
            return fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
        }
        private MusicResponseDto MapToDto(Music music)
        {
            return new MusicResponseDto
            {
                Id = music.Id,

                // Core Info
                Title = music.Title ?? string.Empty,
                Url = music.Url ?? string.Empty,
                Description = music.Description,
                Lyrics = music.Lyrics,
                ThumbnailUrl = music.ThumbnailUrl,
                Language = music.Language,

                // Media Info
                DurationInSeconds = music.DurationInSeconds,
                IsExplicit = music.IsExplicit,
                IsPremium = music.IsPremium,
                PlayCount = music.PlayCount,

                // Dates
                ReleaseDate = music.ReleaseDate,
                CreatedAt = music.CreatedAt,
                UpdatedAt = music.UpdatedAt,

                // Foreign Keys
                AlbumId = music.AlbumId,
                GenreId = music.GenreId,
                UserId = music.UserId,

                // Navigation
                Artists = music.Artists?
                    .Select(a => new ArtistResponseDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    })
                    .ToList() ?? new List<ArtistResponseDto>()
            };
        }

        //private MusicResponseDto MapToDto(Music music)
        //{
        //    return new MusicResponseDto
        //    {
        //        Id = music.Id,
        //        Title = music.Title ?? string.Empty,
        //        Url = music.Url ?? string.Empty,
        //        Description = music.Description,
        //        AlbumId = music.AlbumId,
        //        GenreId = music.GenreId,
        //        Language = music.Language,
        //        IsPremium = music.IsPremium,
        //        ReleaseDate = music.ReleaseDate,
        //        CreatedAt = music.CreatedAt,
        //        Artists = music.Artists?
        //            .Select(a => new ArtistResponseDto
        //            {
        //                Id = a.Id,
        //                Name = a.Name
        //            })
        //            .ToList() ?? new List<ArtistResponseDto>()
        //    };
        //}

        private async Task<List<Artist>> ResolveArtistsAsync(MusicDto dto)
        {
            var artistDtos = dto.Artists?.Select(a => new ArtistResponseDto
            {
                Id = a.Id,
                Name = a.Name
            }).ToList() ?? new();

            return await _repo.GetOrCreateArtistsAsync(artistDtos);
        }

        private async Task<List<Artist>> ResolveArtistsAsync(MusicResponseDto dto)
        {
            var artistDtos = dto.Artists?.ToList() ?? new List<ArtistResponseDto>();
            return await _repo.GetOrCreateArtistsAsync(artistDtos);
        }

        private IActionResult MapError(ResponseCode code, string message, string? errorDetail = null)
        {
            var response = new BaseResponse<string>
            {
                Success = false,
                Message = message,
                Code = code,
                Errors = errorDetail != null ? new List<string> { errorDetail } : null
            };

            return ResponseMapper.Map(new DummyController(), response);
        }

        // Dummy controller to satisfy ResponseMapper signature
        private class DummyController : ControllerBase { }

    }
}
    
    
    //public async Task<IActionResult> GetStreamInfoAsync(int id)
    //{
    //    try
    //    {
    //        var music = await _repo.GetByIdAsync(id);
    //        if (music == null || string.IsNullOrWhiteSpace(music.Url))
    //        {
    //            _logger.LogWarning("Music ID {Id} not found or URL missing.", id);
    //            return MapError(ResponseCode.NotFound, "Music not found or URL missing.");
    //        }

    //        // ✅ Handle YouTube URLs via IFrame embedding
    //        if (IsYouTubeUrl(music.Url))
    //        {
    //            var videoId = ExtractYouTubeVideoId(music.Url);
    //            if (string.IsNullOrWhiteSpace(videoId))
    //            {
    //                _logger.LogWarning("Failed to extract YouTube video ID from URL: {Url}", music.Url);
    //                return MapError(ResponseCode.BadRequest, "Invalid YouTube URL format.");
    //            }

    //            _logger.LogInformation("Returning YouTube video ID for embedding: {VideoId}", videoId);
    //            return new OkObjectResult(new BaseResponse<string>
    //            {
    //                Success = true,
    //                Message = "YouTube video ID for embedding",
    //                Code = ResponseCode.Success,
    //                Data = videoId
    //            });
    //        }

    //        // ✅ Handle remote audio URLs
    //        if (Uri.TryCreate(music.Url, UriKind.Absolute, out var uri) &&
    //            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
    //        {
    //            var mimeType = await GetRemoteMimeTypeAsync(uri.ToString());
    //            if (mimeType != null && IsSupportedAudioMimeType(mimeType))
    //            {
    //                _logger.LogInformation("Returning remote audio URL: {Url}", music.Url);
    //                return new OkObjectResult(new BaseResponse<string>
    //                {
    //                    Success = true,
    //                    Message = "Remote audio URL",
    //                    Code = ResponseCode.Success,
    //                    Data = music.Url
    //                });
    //            }

    //            _logger.LogWarning("Remote URL is not a supported audio format: {Url}", music.Url);
    //            return MapError(ResponseCode.UnsupportedMediaType, "Remote URL is not a supported audio format.");
    //        }

    //        // ✅ Handle local file path
    //        var localPath = music.Url.StartsWith("file:///", StringComparison.OrdinalIgnoreCase)
    //            ? new Uri(music.Url).LocalPath
    //            : music.Url;

    //        if (!string.IsNullOrWhiteSpace(_safeRootDirectory) &&
    //            !Path.GetFullPath(localPath).StartsWith(Path.GetFullPath(_safeRootDirectory), StringComparison.OrdinalIgnoreCase))
    //        {
    //            _logger.LogWarning("Access denied outside safe directory: {Path}", localPath);
    //            return MapError(ResponseCode.Forbidden, "Access to this file is not allowed.");
    //        }

    //        if (!System.IO.File.Exists(localPath))
    //        {
    //            _logger.LogWarning("Local file not found: {Path}", localPath);
    //            return MapError(ResponseCode.NotFound, "Local music file not found.");
    //        }

    //        var mime = GetMimeType(localPath);
    //        if (!IsSupportedAudioMimeType(mime))
    //        {
    //            _logger.LogWarning("Unsupported MIME type: {MimeType} for file {Path}", mime, localPath);
    //            return MapError(ResponseCode.UnsupportedMediaType, "Unsupported audio format.");
    //        }

    //        return new OkObjectResult(new BaseResponse<string>
    //        {
    //            Success = true,
    //            Message = "Local file available",
    //            Code = ResponseCode.Success,
    //            Data = $"/api/stream/audio/{id}"
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting stream info for music ID {Id}", id);
    //        return MapError(ResponseCode.ServerError, "An error occurred while retrieving stream info.", ex.Message);
    //    }
    //}

    //public async Task<IActionResult> StreamAudioAsync(int id)
    //{
    //    try
    //    {
    //        var music = await _repo.GetByIdAsync(id);
    //        if (music == null || string.IsNullOrWhiteSpace(music.Url))
    //        {
    //            _logger.LogWarning("Music ID {Id} not found or URL missing.", id);
    //            return MapError(ResponseCode.NotFound, "Music not found or URL missing.");
    //        }

    //        var localPath = music.Url.StartsWith("file:///", StringComparison.OrdinalIgnoreCase)
    //            ? new Uri(music.Url).LocalPath
    //            : music.Url;

    //        if (!string.IsNullOrWhiteSpace(_safeRootDirectory) &&
    //            !Path.GetFullPath(localPath).StartsWith(Path.GetFullPath(_safeRootDirectory), StringComparison.OrdinalIgnoreCase))
    //        {
    //            _logger.LogWarning("Access denied outside safe directory: {Path}", localPath);
    //            return MapError(ResponseCode.Forbidden, "Access to this file is not allowed.");
    //        }

    //        if (!System.IO.File.Exists(localPath))
    //        {
    //            _logger.LogWarning("Local file not found: {Path}", localPath);
    //            return MapError(ResponseCode.NotFound, "Local music file not found.");
    //        }

    //        var mime = GetMimeType(localPath);
    //        if (!IsSupportedAudioMimeType(mime))
    //        {
    //            _logger.LogWarning("Unsupported MIME type: {MimeType} for file {Path}", mime, localPath);
    //            return MapError(ResponseCode.UnsupportedMediaType, "Unsupported audio format.");
    //        }

    //        var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
    //        return new FileStreamResult(stream, mime)
    //        {
    //          // FileDownloadName = Path.GetFileName(localPath),
    //            EnableRangeProcessing = true
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error streaming audio for music ID {Id}", id);
    //        return MapError(ResponseCode.ServerError, "An error occurred while streaming music.", ex.Message);
    //    }
    //}

    ////helper methods
    //private async Task<string?> GetRemoteMimeTypeAsync(string url)
    //{
    //    try
    //    {
    //        using var handler = new HttpClientHandler
    //        {
    //            AllowAutoRedirect = true
    //        };

    //        using var client = new HttpClient(handler)
    //        {
    //            Timeout = TimeSpan.FromSeconds(5)
    //        };

    //        using var request = new HttpRequestMessage(HttpMethod.Head, url);
    //        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MusicStreamBot/1.0)");

    //        using var response = await client.SendAsync(request);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            var contentType = response.Content.Headers.ContentType?.MediaType;
    //            _logger.LogInformation("Remote MIME type for {Url}: {MimeType}", url, contentType);
    //            return contentType;
    //        }
    //        else
    //        {
    //            _logger.LogWarning("HEAD request failed for {Url} with status {StatusCode}", url, response.StatusCode);
    //        }
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        _logger.LogWarning("HEAD request timed out for {Url}", url);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogWarning(ex, "Failed to detect remote MIME type for URL: {Url}", url);
    //    }

    //    return null;
    //}
    //        private bool IsYouTubeUrl(string url)
    //        {
    //            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
    //                   (uri.Host.Contains("youtube.com") || uri.Host.Contains("youtu.be"));
    //        }

    //        private string? ExtractYouTubeVideoId(string url)
    //        {
    //            var match = Regex.Match(url, @"(?:youtube\.com/watch\?v=|youtu\.be/)([^\&\?\/]+)");
    //            return match.Success ? match.Groups[1].Value : null;
    //        }


    //        private string GetMimeType(string filePath)
    //        {
    //            var extension = Path.GetExtension(filePath)?.ToLowerInvariant();
    //            return extension switch
    //            {
    //                ".mp3" => "audio/mpeg",
    //                ".wav" => "audio/wav",
    //                ".ogg" => "audio/ogg",
    //                ".flac" => "audio/flac",
    //                ".aac" => "audio/aac",
    //                ".mp4" => "audio/mp4",
    //                ".m4a" => "audio/mp4", 
    //                ".webm" => "audio/webm",
    //                ".wma" => "audio/x-ms-wma",
    //                ".aiff" => "audio/x-aiff",
    //                ".mka" => "audio/x-matroska",
    //                _ => "application/octet-stream"
    //            };
    //        }

    //        private static readonly HashSet<string> _supportedAudioTypes = new()
    //{
    //    "audio/mpeg",
    //    "audio/wav",
    //    "audio/ogg",
    //    "audio/flac",
    //    "audio/aac",
    //    "audio/mp4",       
    //    "audio/webm",
    //    "audio/x-ms-wma",
    //    "audio/x-aiff",
    //    "audio/x-matroska"
    //};


    //        private bool IsSupportedAudioMimeType(string mimeType) =>
    //            _supportedAudioTypes.Contains(mimeType);

    // ✅ Summary of Enhancements Feature

    //helper
    //private async Task<string?> GetRemoteMimeTypeAsync(string url)
    //{
    //    try
    //    {
    //        using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true })
    //        {
    //            Timeout = TimeSpan.FromSeconds(5)
    //        };

    //        using var request = new HttpRequestMessage(HttpMethod.Head, url);
    //        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MusicStreamBot/1.0)");

    //        var response = await client.SendAsync(request);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            var contentType = response.Content.Headers.ContentType?.MediaType;
    //            _logger.LogInformation("Detected MIME type for remote URL {Url}: {MimeType}", url, contentType);
    //            return contentType;
    //        }

    //        _logger.LogWarning("HEAD request failed for URL {Url} with status {StatusCode}", url, response.StatusCode);
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        _logger.LogWarning("HEAD request timed out for {Url}", url);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogWarning(ex, "Failed to get remote MIME type for URL: {Url}", url);
    //    }

    //    return null;
    //}

