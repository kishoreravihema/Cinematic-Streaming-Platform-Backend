using Microsoft.AspNetCore.Mvc;
using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix_BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediaService mediaService, ILogger<MediaController> logger)
        {
            _mediaService = mediaService;
            _logger = logger;
        }

        // 🔹 Generic wrapper to reduce repetition
        private async Task<IActionResult> ExecuteAsync<T>(Func<Task<BaseResponse<T>>> action, string actionName)
        {
            try
            {
                _logger.LogInformation("Executing {ActionName}...", actionName);
                var result = await action();
                return ResponseMapper.Map(this, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {ActionName}.", actionName);
                LogExceptionHandler.LogException(ex, actionName);

                var errorResponse = new BaseResponse<T>
                {
                    Success = false,
                    Message = $"An error occurred while executing {actionName}.",
                    Errors = new List<string> { ex.Message },
                    Code = ResponseCode.ServerError
                };

                return ResponseMapper.Map(this, errorResponse);
            }
        }

        /// <summary>Get all albums</summary>
        [HttpGet("albums")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Album>>), 200)]
        public Task<IActionResult> GetAlbums() =>
            ExecuteAsync(() => _mediaService.GetAlbumsAsync(), nameof(GetAlbums));

        /// <summary>Get all artists</summary>
        [HttpGet("artists")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Artist>>), 200)]
        public Task<IActionResult> GetArtists() =>
            ExecuteAsync(() => _mediaService.GetArtistsAsync(), nameof(GetArtists));

        /// <summary>Get all genres</summary>
        [HttpGet("genres")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Genre>>), 200)]
        public Task<IActionResult> GetGenres() =>
            ExecuteAsync(() => _mediaService.GetGenresAsync(), nameof(GetGenres));

        /// <summary>Get all playlist music</summary>
        [HttpGet("playlist-music")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlaylistMusic>>), 200)]
        public Task<IActionResult> GetPlaylistMusic() =>
            ExecuteAsync(() => _mediaService.GetPlaylistMusicAsync(), nameof(GetPlaylistMusic));

        /// <summary>Get all playlist videos</summary>
        [HttpGet("playlist-videos")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlaylistVideo>>), 200)]
        public Task<IActionResult> GetPlaylistVideos() =>
            ExecuteAsync(() => _mediaService.GetPlaylistVideosAsync(), nameof(GetPlaylistVideos));
    }
}
