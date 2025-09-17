using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Helper;
using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Models;
using System.Net.Http;
using static Netflix_BackendAPI.Services.VideoService;

namespace Netflix_BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IMusicService _musicService;
        private readonly IVideoService _videoService;
        private readonly IPlaylistService _playlistService;
        private readonly IFileService _fileService;
        private readonly ILogger<FeaturesController> _logger;
        private readonly HttpClient _httpClient;

        public FeaturesController(
            IMusicService musicService,
            IVideoService videoService,
            IPlaylistService playlistService,
            IFileService fileService,
            ILogger<FeaturesController> logger, IHttpClientFactory httpClientFactory)
        {
            _musicService = musicService;
            _videoService = videoService;
            _playlistService = playlistService;
            _fileService = fileService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        #region 🎧 Music Streaming

        /// <summary>
        /// Gets the stream info for a music track.
        /// </summary>
        [HttpGet("music/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        [ProducesResponseType((int)ResponseCode.NotFound)]
        public async Task<IActionResult> GetStreamInfo([FromRoute] int id)
        {
            if (id <= 0)
            {
                var badRequest = new BaseResponse<PlayInfo> { Success = false, Message = "Invalid music ID", Code = ResponseCode.BadRequest };
                return ResponseMapper.Map(this, badRequest);
            }

            var result = await _musicService.GetStreamInfoAsync(id, Url);
            return ResponseMapper.Map(this, result);
        }

        /// <summary>
        /// Streams a local audio file by ID.
        /// </summary>
        [HttpGet("music/stream/{id}")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileStreamResult), (int)ResponseCode.Success)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.UnsupportedMediaType)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.ServerError)]
        public async Task<IActionResult> StreamAudio([FromRoute] int id)
        {
            if (id <= 0)
            {
                var badRequest = new BaseResponse<string> { Success = false, Message = "Invalid music ID.", Code = ResponseCode.BadRequest };
                return ResponseMapper.Map(this, badRequest);
            }

            var (stream, mimeType, error) = await _musicService.StreamAudioAsync(id);

            if (error != null)
            {
                return ResponseMapper.Map(this, error);
            }

            var safeMimeType = mimeType ?? "application/octet-stream";
            if (stream == null)
            {
                // Defensive: If stream is null, return error response
                var notFound = new BaseResponse<string> { Success = false, Message = "Audio stream not found.", Code = ResponseCode.NotFound };
                return ResponseMapper.Map(this, notFound);
            }

            return new FileStreamResult(stream, safeMimeType)
            {
                EnableRangeProcessing = true
            };
        }

        #endregion

        #region 📺 Video Streaming

        /// <summary>
        /// Gets playback information for a video (URL, YouTube ID, or stream path).
        /// </summary>
        [HttpGet("video/{id}/play-info")]
        [ProducesResponseType(typeof(BaseResponse<PlayInfo>), (int)ResponseCode.Success)]
        [ProducesResponseType(typeof(BaseResponse<PlayInfo>), (int)ResponseCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<PlayInfo>), (int)ResponseCode.NotFound)]
        public async Task<IActionResult> GetVideoPlaybackInfo([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new BaseResponse<PlayInfo>
                {
                    Success = false,
                    Message = "Invalid video ID.",
                    Code = ResponseCode.BadRequest
                });
            }

            _logger.LogInformation("Requesting playback info for video ID {VideoId}", id);
            var response = await _videoService.GetVideoPlaybackInfoAsync(id, Url);
            return new ObjectResult(response) { StatusCode = (int)response.Code };
        }


        /// <summary>
        /// Streams a local video file directly. This action is now corrected to properly handle the service response.
        /// </summary>
        [HttpGet("video/streamNew/{id}")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StreamVideo([FromRoute] int id)
        {
            if (id <= 0)
            {
                return ResponseMapper.Map(this, new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid video ID.",
                    Code = ResponseCode.BadRequest
                });
            }

            var (stream, mimeType, localPath, response) = await _videoService.StreamVideoAsync(id);

            if (response != null)
            {
                return ResponseMapper.Map(this, response);
            }

            if (stream == null || mimeType == null || localPath == null)
            {
                return ResponseMapper.Map(this, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An internal error occurred: stream not available.",
                    Code = ResponseCode.ServerError
                });
            }

            var fileName = Path.GetFileName(localPath);
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");

            _logger.LogInformation("Streaming video ID {VideoId} with MIME type {MimeType}", id, mimeType);

            return new FileStreamResult(stream, mimeType)
            {
                EnableRangeProcessing = true
            };
        }



        /// <summary>
        /// Streams a local video file directly.
        /// </summary>
        [HttpGet("video/streamOld/{id}")]
        [Produces("application/octet-stream")]
        //[ProducesResponseType(typeof(FileStreamResult), (int)ResponseCode.Success)]
        //[ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.BadRequest)]
        //[ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.NotFound)]
        //[ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        public async Task<IActionResult> StreamOldVideo([FromRoute] int id)
        {
            if (id <= 0)
            {
                return ResponseMapper.Map(this, new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid video ID.",
                    Code = ResponseCode.BadRequest
                });
            }

            var (stream, mimeType, localPath, response) = await _videoService.StreamVideoAsync(id);

            if (response != null)
            {
                return ResponseMapper.Map(this, response);
            }

            if (stream == null || mimeType == null || localPath == null)
            {
                return ResponseMapper.Map(this, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An internal error occurred: stream not available.",
                    Code = ResponseCode.ServerError
                });
            }

            _logger.LogInformation("Streaming video ID {VideoId} with MIME type {MimeType}", id, mimeType);

            // ✅ Do not force download, let browser stream it
            return new FileStreamResult(stream, mimeType)
            {
                EnableRangeProcessing = true
            };
        }

        /// <summary>
        /// Returns the HLS playlist (.m3u8) for a video.
        /// </summary>
        [HttpGet("video/Hlsstream/{id}")]
        public async Task<IActionResult> StreamHlsVideo([FromRoute] int id)
        {
            try
            {
                var video = await _videoService.GetOrCreateHlsPlaylistAsync(id);

                if (!System.IO.File.Exists(video.PlaylistPath))
                    return NotFound(new { message = "HLS playlist not found." });

                // ✅ Return .m3u8 with correct MIME type
                return PhysicalFile(video.PlaylistPath, "application/vnd.apple.mpegurl");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// Returns a TS segment from HLS.
        /// </summary>
        [HttpGet("video/HlsSegmentstream/{id}/{segment}")]
        public IActionResult GetHlsSegment([FromRoute] int id, [FromRoute] string segment)
        {
            try
            {
                var hlsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hls", id.ToString());
                var segmentPath = Path.Combine(hlsDir, segment);

                if (!System.IO.File.Exists(segmentPath))
                    return NotFound(new { message = $"Segment {segment} not found." });

                return PhysicalFile(segmentPath, "video/MP2T");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Segment load failed: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }


        #endregion

        #region 📁 File Upload

        /// <summary>
        /// Uploads a music or video file.
        /// </summary>   [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.UnsupportedMediaType)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)ResponseCode.ServerError)]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return StatusCode((int)ResponseCode.BadRequest, new BaseResponse<string>
                {
                    Success = false,
                    Message = "File is required.",
                    Code = ResponseCode.BadRequest,
                    Errors = new List<string> { "No file provided." }
                });
            }

            _logger.LogInformation("Uploading file: {FileName}", file.FileName);
            var response = await _fileService.UploadFileAsync(file);
            return ResponseMapper.Map(this, response);
        }



        #endregion

        #region 🎶 Playlist Management

        /// <summary>
        /// Creates a new playlist.
        /// </summary>
        [HttpPost("playlists")]
        [ProducesResponseType(typeof(BaseResponse<PlaylistDto>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistDto dto)
        {
            if (dto == null)
                return StatusCode((int)ResponseCode.BadRequest, "Playlist data is required.");

            _logger.LogInformation("Creating playlist: {PlaylistName}", dto.Name);
            var response = await _playlistService.CreatePlaylistAsync(dto);
            return ResponseMapper.Map(this, response);
        }

        /// <summary>
        /// Adds a music track to a playlist.
        /// </summary>
        [HttpPost("playlists/{id}/music")]
        [ProducesResponseType(typeof(BaseResponse<bool>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> AddMusicToPlaylist([FromRoute] int id, [FromBody] int musicId)
        {
            if (id <= 0 || musicId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid playlist or music ID.");

            _logger.LogInformation("Adding music {MusicId} to playlist {PlaylistId}", musicId, id);
            var response = await _playlistService.AddMusicToPlaylistAsync(id, musicId);
            return ResponseMapper.Map(this, response);
        }

        /// <summary>
        /// Adds a video to a playlist.
        /// </summary>
        [HttpPost("playlists/{id}/video")]
        [ProducesResponseType(typeof(BaseResponse<bool>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> AddVideoToPlaylist([FromRoute] int id, [FromBody] int videoId)
        {
            if (id <= 0 || videoId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid playlist or video ID.");

            _logger.LogInformation("Adding video {VideoId} to playlist {PlaylistId}", videoId, id);
            var response = await _playlistService.AddVideoToPlaylistAsync(id, videoId);
            return ResponseMapper.Map(this, response);
        }


        #endregion

        #region 📂 Retrieval Endpoints
        [HttpGet("playlists")]
        [ProducesResponseType(typeof(BaseResponse<List<Playlist>>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]

        public async Task<IActionResult> GetAllPlaylistsWithMedia()
        {
            var response = await _playlistService.GetAllPlaylistsWithMediaAsync();
            return ResponseMapper.Map(this, response); 
        }
        /// <summary>
        /// Gets all videos in a playlist.
        /// </summary>
        [HttpGet("playlists/{playlistId}/videos")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Video>>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> GetVideosByPlaylistId([FromRoute] int playlistId)
        {
            if (playlistId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid playlist ID.");

            var response = await _videoService.GetVideosByPlaylistIdAsync(playlistId);
            return ResponseMapper.Map(this, response);
        }

        /// <summary>
        /// Gets all music tracks in an album.
        /// </summary>
        [HttpGet("albums/{albumId}/music")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<MusicResponseDto>>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> GetByAlbum([FromRoute] int albumId)
        {
            if (albumId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid album ID.");

            _logger.LogInformation("Fetching music tracks by Album ID: {AlbumId}", albumId);
            var response = await _musicService.GetByAlbumAsync(albumId);
            return ResponseMapper.Map(this, response);
        }

        [HttpGet("streamTeraBox")]
        public async Task<IActionResult> Stream([FromQuery] string? id, [FromQuery] string? url)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(url))
                return BadRequest("Provide either 'id' or 'url'.");

            // If a full URL is given, use it. Otherwise, build from id.
            string shareUrl = !string.IsNullOrEmpty(url)
                ? url
                : $"https://1024terabox.com/s/{id}";

            var response = await _httpClient.GetAsync(shareUrl, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to resolve Terabox file.");

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var stream = await response.Content.ReadAsStreamAsync();

            return File(stream, contentType);
        }
    

        #endregion
    }
}
