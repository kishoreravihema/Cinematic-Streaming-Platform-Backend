using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Helper;
using Netflix_BackendAPI.Interface.IRepository;
using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Models;
using System.Diagnostics;
using System.Linq.Expressions;
using static Netflix_BackendAPI.Services.VideoService;

namespace Netflix_BackendAPI.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ILogger<VideoService> _logger;
        private readonly string _safeRootDirectory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public VideoService(IVideoRepository videoRepository, ILogger<VideoService> logger,IWebHostEnvironment env,
        IConfiguration config,IHttpClientFactory httpClientFactory)
        {
            _videoRepository = videoRepository;
            _logger = logger;
            _safeRootDirectory = config["FileStreaming:VideoRootDirectory"] ?? string.Empty;
            _httpClientFactory = httpClientFactory;
            _env = env;

        }

        public async Task<BaseResponse<IEnumerable<VideoResponseDto>>> GetAllVideosAsync(int page, int pageSize)
        {
            try
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 10;

                int skip = (page - 1) * pageSize;
                var videos = await _videoRepository.GetPagedVideosAsync(skip, pageSize);

                var dtoList = videos.Select(MapToResponseDto).ToList();

                return new BaseResponse<IEnumerable<VideoResponseDto>>
                {
                    Success = true,
                    Message = $"Fetched page {page} with {dtoList.Count} videos.",
                    Data = dtoList,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetAllVideosAsync));
                return new BaseResponse<IEnumerable<VideoResponseDto>>
                {
                    Success = false,
                    Message = "Failed to fetch videos.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<Video>> GetVideoByIdAsync(int id)
        {
            try
            {
                var video = await _videoRepository.GetVideoByIdAsync(id);
                if (video == null)
                {
                    return new BaseResponse<Video>
                    {
                        Success = false,
                        Message = "Video not found",
                        Code = ResponseCode.NotFound
                    };
                }

                return new BaseResponse<Video>
                {
                    Success = true,
                    Message = "Video retrieved",
                    Data = video,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetVideoByIdAsync));
                return new BaseResponse<Video>
                {
                    Success = false,
                    Message = "Failed to retrieve video.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<Video>>> GetVideosByPlaylistIdAsync(int playlistId)
        {
            try
            {
                var data = await _videoRepository.GetVideosByPlaylistIdAsync(playlistId);
                return new BaseResponse<IEnumerable<Video>>
                {
                    Success = true,
                    Message = "Playlist videos fetched",
                    Data = data,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetVideosByPlaylistIdAsync));
                return new BaseResponse<IEnumerable<Video>>
                {
                    Success = false,
                    Message = "Failed to fetch playlist videos.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<VideoResponseDto>>> GetVideosByCategoryAsync(string categoryName)
        {
            try
            {
                var videos = await _videoRepository.GetVideosByCategoryAsync(categoryName);

                var dtoList = videos.Select(MapToResponseDto).ToList();

                return new BaseResponse<IEnumerable<VideoResponseDto>>
                {
                    Success = true,
                    Message = $"Fetched videos for category '{categoryName}'",
                    Data = dtoList,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(GetVideosByCategoryAsync));
                return new BaseResponse<IEnumerable<VideoResponseDto>>
                {
                    Success = false,
                    Message = "Failed to fetch category videos.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<VideoResponseDto>> AddVideoAsync(VideoDto dto)
        {
            try
            {
                var actors = await ResolveActorsAsync(dto);

                var video = new Video
                {
                    Title = dto.Title,
                    Url = dto.Url,
                    Description = dto.Description,
                    PlaylistId = dto.PlaylistId,
                    CategoryId = dto.CategoryId,
                    ThumbnailUrl = dto.ThumbnailUrl,
                    Rating = dto.Rating,
                    DurationInSeconds = dto.DurationInSeconds,
                    Language = dto.Language,
                    AgeRating = dto.AgeRating,
                    Genre = dto.Genre,
                    Director = dto.Director,
                    IsPremium = dto.IsPremium,
                    ReleaseDate = dto.ReleaseDate,
                    CreatedAt = dto.CreatedAt != default ? dto.CreatedAt : DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Actors = actors
                };

                var created = await _videoRepository.AddVideoAsync(video);
                if (created == null)
                {
                    return new BaseResponse<VideoResponseDto>
                    {
                        Success = false,
                        Message = "Failed to create video",
                        Code = ResponseCode.BadRequest
                    };
                }

                var dtoOut = MapToResponseDto(created);
                return new BaseResponse<VideoResponseDto>
                {
                    Success = true,
                    Message = "Video added successfully.",
                    Data = dtoOut,
                    Code = ResponseCode.Created
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(AddVideoAsync));
                return new BaseResponse<VideoResponseDto>
                {
                    Success = false,
                    Message = "Failed to add video.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<VideoResponseDto>> AddVideoWithManualIdAsync(int id, VideoDto dto)
        {
            try
            {
                var actors = await ResolveActorsAsync(dto);

                var video = new Video
                {
                    Id = id,
                    Title = dto.Title,
                    Url = dto.Url,
                    Description = dto.Description,
                    PlaylistId = dto.PlaylistId,
                    CategoryId = dto.CategoryId,
                    ThumbnailUrl = dto.ThumbnailUrl,
                    Rating = dto.Rating,
                    DurationInSeconds = dto.DurationInSeconds,
                    Language = dto.Language,
                    AgeRating = dto.AgeRating,
                    Genre = dto.Genre,
                    Director = dto.Director,
                    IsPremium = dto.IsPremium,
                    ReleaseDate = dto.ReleaseDate,
                    CreatedAt = dto.CreatedAt != default ? dto.CreatedAt : DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Actors = actors
                };

                var created = await _videoRepository.AddVideoWithManualIdAsync(video);
                if (created == null)
                {
                    return new BaseResponse<VideoResponseDto>
                    {
                        Success = false,
                        Message = "Failed to create video.",
                        Code = ResponseCode.BadRequest
                    };
                }

                var dtoOut = MapToResponseDto(created);
                return new BaseResponse<VideoResponseDto>
                {
                    Success = true,
                    Message = "Video added successfully.",
                    Data = dtoOut,
                    Code = ResponseCode.Created
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(AddVideoWithManualIdAsync));
                return new BaseResponse<VideoResponseDto>
                {
                    Success = false,
                    Message = "Failed to add video with manual ID.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<VideoResponseDto>> UpdateVideoAsync(VideoResponseDto dto)
        {
            try
            {
                var actors = await ResolveActorsAsync(dto);

                var existingVideo = await _videoRepository.GetVideoByIdAsync(dto.Id);
                if (existingVideo == null)
                {
                    return new BaseResponse<VideoResponseDto>
                    {
                        Success = false,
                        Message = "Video not found.",
                        Code = ResponseCode.NotFound
                    };
                }

                existingVideo.Title = dto.Title ?? string.Empty;
                existingVideo.Url = dto.VideoUrl ?? string.Empty;
                existingVideo.Description = dto.Description;
                existingVideo.PlaylistId = dto.PlaylistId;
                existingVideo.CategoryId = dto.CategoryId;
                existingVideo.ThumbnailUrl = dto.ThumbnailUrl;
                existingVideo.Rating = dto.Rating;
                existingVideo.DurationInSeconds = dto.DurationInSeconds;
                existingVideo.Language = dto.Language;
                existingVideo.AgeRating = dto.AgeRating;
                existingVideo.Genre = dto.Genre;
                existingVideo.Director = dto.Director;
                existingVideo.IsPremium = dto.IsPremium;
                existingVideo.ReleaseDate = dto.ReleaseDate;
                existingVideo.UpdatedAt = DateTime.UtcNow;
                existingVideo.Actors = actors;

                var updatedEntity = await _videoRepository.UpdateVideoAsync(existingVideo);
                var resultDto = MapToResponseDto(updatedEntity);

                return new BaseResponse<VideoResponseDto>
                {
                    Success = true,
                    Message = "Video updated successfully.",
                    Data = resultDto,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(UpdateVideoAsync));
                return new BaseResponse<VideoResponseDto>
                {
                    Success = false,
                    Message = "Failed to update video.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<VideoResponseDto>> UpdateVideoByIdAsync(int id, VideoDto dto)
        {
            try
            {
                var actors = await ResolveActorsAsync(dto);

                var existingVideo = await _videoRepository.GetVideoByIdAsync(id);
                if (existingVideo == null)
                {
                    return new BaseResponse<VideoResponseDto>
                    {
                        Success = false,
                        Message = "Video not found.",
                        Code = ResponseCode.NotFound
                    };
                }

                existingVideo.Title = dto.Title;
                existingVideo.Url = dto.Url;
                existingVideo.Description = dto.Description;
                existingVideo.PlaylistId = dto.PlaylistId;
                existingVideo.CategoryId = dto.CategoryId;
                existingVideo.ThumbnailUrl = dto.ThumbnailUrl;
                existingVideo.Rating = dto.Rating;
                existingVideo.DurationInSeconds = dto.DurationInSeconds;
                existingVideo.Language = dto.Language;
                existingVideo.AgeRating = dto.AgeRating;
                existingVideo.Genre = dto.Genre;
                existingVideo.Director = dto.Director;
                existingVideo.IsPremium = dto.IsPremium;
                existingVideo.ReleaseDate = dto.ReleaseDate;
                existingVideo.UpdatedAt = DateTime.UtcNow;
                existingVideo.Actors = actors;

                var updatedEntity = await _videoRepository.UpdateVideoAsync(existingVideo);
                var resultDto = MapToResponseDto(updatedEntity);

                return new BaseResponse<VideoResponseDto>
                {
                    Success = true,
                    Message = "Video updated successfully.",
                    Data = resultDto,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(UpdateVideoByIdAsync));
                return new BaseResponse<VideoResponseDto>
                {
                    Success = false,
                    Message = "Failed to update video.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> DeleteVideoAsync(int id)
        {
            try
            {
                var exists = await _videoRepository.VideoExistsAsync(id);
                if (!exists)
                {
                    return new BaseResponse<bool>
                    {
                        Success = false,
                        Message = "Video not found",
                        Data = false,
                        Code = ResponseCode.NotFound
                    };
                }

                var deleted = await _videoRepository.DeleteVideoAsync(id);
                return new BaseResponse<bool>
                {
                    Success = deleted,
                    Message = deleted ? "Video deleted" : "Delete failed",
                    Data = deleted,
                    Code = deleted ? ResponseCode.NoContent : ResponseCode.ServerError
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(DeleteVideoAsync));
                return new BaseResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while deleting the video.",
                    Errors = new List<string> { ex.Message },
                    Data = false,
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> VideoExistsAsync(int id)
        {
            try
            {
                var exists = await _videoRepository.VideoExistsAsync(id);
                return new BaseResponse<bool>
                {
                    Success = true,
                    Message = "Existence checked",
                    Data = exists,
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(VideoExistsAsync));
                return new BaseResponse<bool>
                {
                    Success = false,
                    Message = "Failed to check video existence.",
                    Errors = new List<string> { ex.Message },
                    Data = false,
                    Code = ResponseCode.ServerError
                };
            }
        }

        //Write this code as common for all services


        //public async Task<(FileStreamResult? Stream, BaseResponse<string>? Error)> StreamVideoAsync(int id)
        //{
        //    try
        //    {
        //        var video = await _videoRepository.GetVideoByIdAsync(id);
        //        if (video == null || string.IsNullOrWhiteSpace(video.Url))
        //        {
        //            return (null, new BaseResponse<string>
        //            {
        //                Success = false,
        //                Message = "Video not found.",
        //                Code = ResponseCode.NotFound
        //            });
        //        }

        //        if (video.Url.StartsWith("file:///"))
        //        {
        //            var localPath = new Uri(video.Url).LocalPath;
        //            return await StreamLocalFileAsync(localPath);
        //        }
        //        else if (Uri.TryCreate(video.Url, UriKind.Absolute, out var uri) &&
        //                 (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        //        {
        //            _logger.LogInformation("Returning remote video URL: {Url}", video.Url);

        //            var response = new BaseResponse<string>
        //            {
        //                Success = true,
        //                Message = "Video is hosted remotely. Please redirect or embed.",
        //                Code = ResponseCode.Found,
        //                Data = video.Url
        //            };

        //            return (null, response);
        //        }
        //        else
        //        {
        //            var localPath = Path.Combine(_safeRootDirectory, video.Url.TrimStart('\\', '/'));
        //            return await StreamLocalFileAsync(localPath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error streaming video with ID {Id}", id);
        //        LogExceptionHandler.LogException(ex, nameof(StreamVideoAsync));
        //        return (null, new BaseResponse<string>
        //        {
        //            Success = false,
        //            Message = "Internal server error while streaming video.",
        //            Code = ResponseCode.ServerError
        //        });
        //    }
        //}

        //private async Task<(FileStreamResult? Stream, BaseResponse<string>? Error)> StreamLocalFileAsync(string localPath)
        //{
        //    if (!localPath.StartsWith(_safeRootDirectory, StringComparison.OrdinalIgnoreCase))
        //    {
        //        _logger.LogWarning("Blocked access to path outside safe root: {Path}", localPath);
        //        return (null, new BaseResponse<string>
        //        {
        //            Success = false,
        //            Message = "Access to the requested file is denied.",
        //            Code = ResponseCode.Forbidden
        //        });
        //    }

        //    if (!System.IO.File.Exists(localPath))
        //    {
        //        _logger.LogWarning("File not found: {Path}", localPath);
        //        return (null, new BaseResponse<string>
        //        {
        //            Success = false,
        //            Message = "Video file not found on disk.",
        //            Code = ResponseCode.NotFound
        //        });
        //    }

        //    var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    var mimeType = GetMimeType(localPath);

        //    var result = new FileStreamResult(stream, mimeType)
        //    {
        //        FileDownloadName = Path.GetFileName(localPath)
        //    };

        //    return (result, null);
        //}

        //private string GetMimeType(string filePath)
        //{
        //    var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
        //    return ext switch
        //    {
        //        ".mp4" => "video/mp4",
        //        ".webm" => "video/webm",
        //        ".mkv" => "video/x-matroska",
        //        ".ogg" => "video/ogg",
        //        ".avi" => "video/x-msvideo",
        //        ".wmv" => "video/x-ms-wmv",
        //        ".mov" => "video/quicktime",
        //        ".flv" => "video/x-flv",
        //        ".3gp" => "video/3gpp",
        //        _ => "application/octet-stream"
        //    };
        //}
        public async Task<BaseResponse<PlayInfo>> GetVideoPlaybackInfoAsync(int id, IUrlHelper urlHelper)
        {
            try
            {
                var video = await _videoRepository.GetVideoByIdAsync(id);
                if (video == null || string.IsNullOrWhiteSpace(video.Url))
                {
                    return new BaseResponse<PlayInfo>
                    {
                        Success = false,
                        Message = "Video not found.",
                        Code = ResponseCode.NotFound
                    };
                }

                // 🎬 Case 1: YouTube URL
                if (MediaHelper.IsYouTubeUrl(video.Url))
                {
                    var videoId = MediaHelper.ExtractYouTubeId(video.Url);
                    var playlistId = MediaHelper.ExtractYouTubePlaylistId(video.Url);

                    // ✅ Prefer actual video if available
                    if (!string.IsNullOrWhiteSpace(videoId))
                    {
                        return new BaseResponse<PlayInfo>
                        {
                            Success = true,
                            Data = new PlayInfo
                            {
                                PlaybackType = "YOUTUBE",
                                Url = $"https://www.youtube.com/watch?v={videoId}",
                                ThumbnailUrl = $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg"
                            },
                            Code = ResponseCode.Success
                        };
                    }

                    // 🎵 Fallback to playlist if no videoId
                    if (!string.IsNullOrWhiteSpace(playlistId))
                    {
                        return new BaseResponse<PlayInfo>
                        {
                            Success = true,
                            Data = new PlayInfo
                            {
                                PlaybackType = "YOUTUBE",
                                Url = $"https://www.youtube.com/playlist?list={playlistId}",
                                ThumbnailUrl = null // playlists don't have a single static thumbnail
                            },
                            Code = ResponseCode.Success
                        };
                    }

                    // 📺 YouTube channel playlists page
                    if (MediaHelper.IsYouTubeChannelPlaylistPage(video.Url))
                    {
                        return new BaseResponse<PlayInfo>
                        {
                            Success = true,
                            Data = new PlayInfo
                            {
                                PlaybackType = "YOUTUBE_CHANNEL_PLAYLISTS",
                                Url = video.Url,
                                ThumbnailUrl = null
                            },
                            Code = ResponseCode.Success
                        };
                    }

                    return new BaseResponse<PlayInfo>
                    {
                        Success = false,
                        Message = "Invalid YouTube URL format.",
                        Code = ResponseCode.BadRequest
                    };
                }

                // 🌐 Case 2: Remote Video URL
                if (Uri.TryCreate(video.Url, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    return new BaseResponse<PlayInfo>
                    {
                        Success = true,
                        Data = new PlayInfo
                        {
                            PlaybackType = "REMOTE_URL",
                            Url = video.Url,
                            ThumbnailUrl = video.ThumbnailUrl // if you already store a remote thumbnail in DB
                        },
                        Code = ResponseCode.Success
                    };
                }

                // 📁 Case 3: Local File Path
                var streamEndpoint = urlHelper.Action(
                    action: "StreamVideo",
                    controller: "Videos",
                    values: new { id = id }
                );

                var localPath = video.Url;
                var thumbnailDir = Path.Combine("wwwroot", "thumbnails");
                Directory.CreateDirectory(thumbnailDir);

                var thumbnailPath = Path.Combine(thumbnailDir, $"{video.Id}.jpg");
                if (!File.Exists(thumbnailPath))
                {
                    await ThumbnailHelper.GenerateThumbnailAsync(localPath, thumbnailPath);
                }

                return new BaseResponse<PlayInfo>
                {
                    Success = true,
                    Data = new PlayInfo
                    {
                        PlaybackType = "LOCAL_STREAM",
                        Url = streamEndpoint,
                        ThumbnailUrl = $"/thumbnails/{video.Id}.jpg"
                    },
                    Code = ResponseCode.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting video playback info for ID {Id}", id);
                LogExceptionHandler.LogException(ex, nameof(GetVideoPlaybackInfoAsync));
                return new BaseResponse<PlayInfo>
                {
                    Success = false,
                    Message = "Internal server error while fetching video playback info.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };
            }
        }

        public async Task<BaseResponse<object>> StreamVideoAsyncOld(int id)
        {
            try
            {
                var video = await _videoRepository.GetVideoByIdAsync(id);
                if (video == null || string.IsNullOrWhiteSpace(video.Url) || video.Url.StartsWith("http"))
                {
                    return StreamResponseBuilder.FromError(ResponseCode.NotFound, "Video not found or invalid URL.");
                }

                string localPath;

                // Case 1: Absolute path
                if (Path.IsPathRooted(video.Url))
                {
                    localPath = Path.GetFullPath(video.Url);
                    var safeRoot = Path.GetFullPath(_safeRootDirectory);

                    if (!localPath.StartsWith(safeRoot, StringComparison.OrdinalIgnoreCase))
                        return StreamResponseBuilder.FromError(ResponseCode.Forbidden, "Access denied.");

                    if (!File.Exists(localPath))
                        return StreamResponseBuilder.FromError(ResponseCode.NotFound, "Video file not found.");

                    return StreamResponseBuilder.FromFile(
                        ResponseCode.Success,
                        localPath,
                        MediaHelper.GetMimeType(localPath),
                        Path.GetFileName(localPath)
                    );
                }

                // Case 2: Relative or partial path
                string partialName = Path.GetFileNameWithoutExtension(video.Url);

                string? resolvedFile = ResolveFileByPartialName(_safeRootDirectory, partialName);

                if (string.IsNullOrEmpty(resolvedFile) || !File.Exists(resolvedFile))
                {
                    _logger.LogWarning("Resolved video path not found for partial match: {Url}", video.Url);
                    return StreamResponseBuilder.FromError(ResponseCode.NotFound, "Video file not found.");
                }

                localPath = resolvedFile;
                _logger.LogInformation("Resolved video path: {Path}", localPath);

                var mime = MediaHelper.GetMimeType(localPath);
                if (!MediaHelper.IsSupportedVideoMimeType(mime))
                {
                    _logger.LogWarning("Unsupported video MIME type: {MimeType}", mime);
                    return StreamResponseBuilder.FromError(ResponseCode.UnsupportedMediaType, "Unsupported video format.");
                }

                _logger.LogInformation("Streaming video with MIME type: {Mime}", mime);

                var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 65536, useAsync: true);

                var streamResult = new FileStreamResult(stream, mime)
                {
                    EnableRangeProcessing = true,
                    FileDownloadName = Path.GetFileName(localPath)
                };

                return StreamResponseBuilder.FromStream(ResponseCode.Success, streamResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error streaming video with ID {Id}", id);
                LogExceptionHandler.LogException(ex, nameof(StreamVideoAsync));
                return StreamResponseBuilder.FromError(ResponseCode.ServerError, "Internal server error.");
            }
        }


        public async Task<(Stream? stream, string? mimeType, string? localPath, BaseResponse<object>? response)> StreamVideoAsync(int id)
        {
            try
            {
                var video = await _videoRepository.GetVideoByIdAsync(id);
                if (video == null)
                {
                    return (null, null, null, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "Video not found.",
                        Code = ResponseCode.NotFound
                    });
                }

                if (MediaHelper.IsYouTubeUrl(video.Url))
                {
                    _logger.LogWarning("StreamVideoAsync called for YouTube video ID {VideoId}. This should be handled via playback info.", id);
                    return (null, null, null, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "Streaming YouTube videos directly is not supported.",
                        Code = ResponseCode.BadRequest
                    });
                }

                // 📁 Resolve local file path
                // 📁 Resolve local file path
                string? localPath = null;
                var safeRoot = Path.GetFullPath(_safeRootDirectory);

                if (Path.IsPathRooted(video.Url))
                {
                    // Case 1: DB stored an absolute path (rare)
                    localPath = Path.GetFullPath(video.Url);

                    if (!localPath.StartsWith(safeRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        return (null, null, null, new BaseResponse<object>
                        {
                            Success = false,
                            Message = "Access to the requested file is forbidden.",
                            Code = ResponseCode.Forbidden
                        });
                    }
                }
                else if (!string.IsNullOrWhiteSpace(video.Url))
                {
                    // Case 2: DB stored just filename or relative path
                    localPath = Path.Combine(safeRoot, video.Url);

                    // If not found, try partial match (fallback)
                    if (!File.Exists(localPath))
                    {
                        string partialName = Path.GetFileNameWithoutExtension(video.Url);
                        localPath = ResolveFileByPartialName(safeRoot, partialName);
                    }
                }

                else
                {
                    localPath = Path.Combine(_safeRootDirectory, video.Url);
                }

                if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                {
                    _logger.LogWarning("Video file not found for ID {VideoId} at path: {Path}", id, localPath ?? "null");
                    return (null, null, null, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "Video file could not be found on the server.",
                        Code = ResponseCode.NotFound
                    });
                }

                // 🎨 Validate MIME type
                var mimeType = MediaHelper.GetMimeType(localPath);
                if (!MediaHelper.IsSupportedVideoMimeType(mimeType))
                {
                    _logger.LogWarning("Unsupported video MIME type: {MimeType} for file {Path}", mimeType, localPath);
                    return (null, null, null, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "The requested video format is not supported.",
                        Code = ResponseCode.UnsupportedMediaType
                    });
                }

                // 📸 Auto-generate thumbnail
                var thumbnailDir = Path.Combine("wwwroot", "thumbnails");
                Directory.CreateDirectory(thumbnailDir);

                var thumbnailPath = Path.Combine(thumbnailDir, $"{video.Id}.jpg");
                if (!File.Exists(thumbnailPath))
                {
                    try
                    {
                        await ThumbnailHelper.GenerateThumbnailAsync(localPath, thumbnailPath);
                        _logger.LogInformation("Thumbnail generated for video ID {VideoId} at {ThumbnailPath}", id, thumbnailPath);
                    }
                    catch (Exception thumbEx)
                    {
                        _logger.LogWarning(thumbEx, "Failed to generate thumbnail for video ID {VideoId}", id);
                        // Continue streaming even if thumbnail fails
                    }
                }

                // 🎥 Return stream
                _logger.LogInformation("Resolved video path for streaming: {Path}", localPath);
                var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 65536, useAsync: true);

                return (stream, mimeType, localPath, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing video stream for ID {Id}", id);
                LogExceptionHandler.LogException(ex, nameof(StreamVideoAsync));

                return (null, null, null, new BaseResponse<object>
                {
                    Success = false,
                    Message = "An internal server error occurred while preparing the video stream.",
                    Code = ResponseCode.ServerError
                });
            }
        }


        public async Task<(string PlaylistPath, string DirectoryPath)> GetOrCreateHlsPlaylistAsync(int id)
        {
            var video = await _videoRepository.GetVideoByIdAsync(id);
            if (video == null)
                throw new FileNotFoundException($"Video {id} not found in database.");

            // ✅ Ensure filename is safe
            var safeRoot = Path.GetFullPath(_safeRootDirectory);
            var fileName = Path.GetFileName(video.Url)?.Trim();
            if (string.IsNullOrWhiteSpace(fileName))
                throw new FileNotFoundException($"Video {id} has invalid or empty file name in DB.");

            var localPath = Path.Combine(safeRoot, fileName);
            Console.WriteLine($"[DEBUG] Checking video file at: {localPath}");

            if (!File.Exists(localPath))
                throw new FileNotFoundException($"Video file not found at {localPath}");

            // 📂 HLS output directory (absolute path inside wwwroot/hls/{id})
            var hlsDir = Path.Combine(_env.WebRootPath, "hls", id.ToString());
            var playlistPath = Path.Combine(hlsDir, "output.m3u8");

            if (!Directory.Exists(hlsDir))
                Directory.CreateDirectory(hlsDir);

            // ✅ FFmpeg executable (keep path configurable in appsettings.json ideally)
            var ffmpegPath = @"C:\Users\kr470\OneDrive\Desktop\KAPP\KApp_NewDevelopment\Netflix_BackendAPI\ffmpeg\ffmpeg-8.0-essentials_build\bin\ffmpeg.exe";
            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException($"FFmpeg not found at {ffmpegPath}");

            // ⚡ If HLS already exists, skip re-encoding
            if (!File.Exists(playlistPath))
            {
                var ffmpegArgs =
                    $"-i \"{localPath}\" -c:v h264 -crf 23 -preset veryfast -c:a aac " +
                    "-start_number 0 -hls_time 10 -hls_list_size 0 -f hls " +
                    $"\"{playlistPath}\"";

                Console.WriteLine($"[DEBUG] Running FFmpeg: {ffmpegPath} {ffmpegArgs}");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = ffmpegArgs,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string stderr = await process.StandardError.ReadToEndAsync();
                string stdout = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"[FFMPEG ERROR] {stderr}");
                    throw new Exception($"FFmpeg failed for video {id}. ExitCode={process.ExitCode}, Error={stderr}");
                }

                Console.WriteLine($"[FFMPEG SUCCESS] {stdout}");
            }

            return (playlistPath, hlsDir);
        }


        //local helper methods

        private static string? ResolveFileByPartialName(string directory, string partialName)
        {
            // This logic is preserved as requested, but be aware of its potential for mismatches.
            return Directory
                .EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f)
                    .Contains(partialName, StringComparison.OrdinalIgnoreCase));
        }

        private VideoResponseDto MapToResponseDto(Video video)
        {
            return new VideoResponseDto
            {
                Id = video.Id,
                Title = video.Title,
                VideoUrl = video.Url,
                Description = video.Description,
                PlaylistId = video.PlaylistId,
                CategoryId = video.CategoryId,
                ThumbnailUrl = video.ThumbnailUrl,
                Rating = video.Rating,
                DurationInSeconds = video.DurationInSeconds,
                Language = video.Language,
                AgeRating = video.AgeRating,
                Genre = video.Genre,
                Director = video.Director,
                IsPremium = video.IsPremium,
                ReleaseDate = video.ReleaseDate,
                CreatedAt = video.CreatedAt,
                Actors = video.Actors?
                    .Select(a => new ActorResponseDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    })
                    .ToList() ?? new List<ActorResponseDto>()
            };
        }

        private async Task<List<Actor>> ResolveActorsAsync(VideoDto dto)
        {
            var actorDtos = dto.Actors?.Select(a => new ActorResponseDto
            {
                Id = a.Id,
                Name = a.Name
            }).ToList() ?? new();

            return await _videoRepository.GetOrCreateActorsAsync(actorDtos);
        }

        private async Task<List<Actor>> ResolveActorsAsync(VideoResponseDto dto)
        {
            var actorDtos = dto.Actors?.ToList() ?? new List<ActorResponseDto>();
            return await _videoRepository.GetOrCreateActorsAsync(actorDtos);
        }



    }
}
