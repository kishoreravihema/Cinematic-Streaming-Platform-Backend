using Microsoft.AspNetCore.Mvc;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IServices;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        #region 🔍 Retrieval Endpoints

        // GET: api/videos?page=1&pageSize=10
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VideoResponseDto>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> GetAllVideos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid pagination parameters.");

            var response = await _videoService.GetAllVideosAsync(page, pageSize);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/videos/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Video), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.NotFound)]
        public async Task<IActionResult> GetVideoById([FromRoute] int id)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid video ID.");

            var response = await _videoService.GetVideoByIdAsync(id);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/videos/playlists/1
        [HttpGet("playlists/{playlistId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Video>), (int)ResponseCode.Success)]
        public async Task<IActionResult> GetVideosByPlaylistId([FromRoute] int playlistId)
        {
            if (playlistId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid playlist ID.");

            var response = await _videoService.GetVideosByPlaylistIdAsync(playlistId);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/videos/categories/Action
        [HttpGet("categories/{category}")]
        [ProducesResponseType(typeof(IEnumerable<Video>), (int)ResponseCode.Success)]
        public async Task<IActionResult> GetVideosByCategory([FromRoute] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return StatusCode((int)ResponseCode.BadRequest, "Category is required.");

            var response = await _videoService.GetVideosByCategoryAsync(category);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/videos/5/exists
        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(typeof(bool), (int)ResponseCode.Success)]
        public async Task<IActionResult> Exists([FromRoute] int id)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid video ID.");

            var response = await _videoService.VideoExistsAsync(id);
            return ResponseMapper.Map(this, response);
        }

        #endregion

        #region ➕ Creation

        // POST: api/videos
        [HttpPost]
        [ProducesResponseType(typeof(VideoResponseDto), (int)ResponseCode.Created)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> AddVideo([FromBody] VideoDto dto)
        {
            if (!ModelState.IsValid)
                return ResponseMapper.Map(this, ResponseMapper.FromModelState<VideoResponseDto>(ModelState));

            var response = await _videoService.AddVideoAsync(dto);
            return ResponseMapper.Map(this, response);
        }

        // POST: api/videos/5
        [HttpPost("{id:int}")]
        [ProducesResponseType(typeof(VideoResponseDto), (int)ResponseCode.Created)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> AddVideoWithManualIdAsync([FromRoute] int id, [FromBody] VideoDto dto)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid video ID.");

            if (!ModelState.IsValid)
                return ResponseMapper.Map(this, ResponseMapper.FromModelState<VideoResponseDto>(ModelState));

            var response = await _videoService.AddVideoWithManualIdAsync(id, dto);
            return ResponseMapper.Map(this, response);
        }

        #endregion

        #region ✏️ Update

        // PUT: api/videos
        [HttpPut]
        [ProducesResponseType(typeof(VideoResponseDto), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> UpdateVideo([FromBody] VideoResponseDto dto)
        {
            if (!ModelState.IsValid)
                return ResponseMapper.Map(this, ResponseMapper.FromModelState<VideoResponseDto>(ModelState));

            var response = await _videoService.UpdateVideoAsync(dto);
            return ResponseMapper.Map(this, response);
        }

        // PUT: api/videos/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(VideoResponseDto), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> UpdateVideoById([FromRoute] int id, [FromBody] VideoDto dto)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid video ID.");

            if (!ModelState.IsValid)
                return ResponseMapper.Map(this, ResponseMapper.FromModelState<VideoResponseDto>(ModelState));

            var response = await _videoService.UpdateVideoByIdAsync(id, dto);
            return ResponseMapper.Map(this, response);
        }

        #endregion

        #region ❌ Deletion

        // DELETE: api/videos/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(bool), (int)ResponseCode.NoContent)]
        [ProducesResponseType((int)ResponseCode.NotFound)]
        public async Task<IActionResult> DeleteVideo([FromRoute] int id)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid video ID.");

            var response = await _videoService.DeleteVideoAsync(id);
            return ResponseMapper.Map(this, response);
        }

        #endregion
    }
}
