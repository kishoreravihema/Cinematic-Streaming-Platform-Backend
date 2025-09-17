using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IServices;

namespace Netflix_BackendAPI.Controllers
{
    [ApiController]
    [Route("api/music")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class MusicController : ControllerBase
    {
        private readonly IMusicService _musicService;
        private readonly ILogger<MusicController> _logger;

        public MusicController(IMusicService musicService, ILogger<MusicController> logger)
        {
            _musicService = musicService;
            _logger = logger;
        }

        #region 🔍 Retrieval Endpoints

        // GET: api/music?page=1&pageSize=10
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MusicResponseDto>), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid pagination parameters.");

            _logger.LogInformation("Fetching music tracks. Page: {Page}, PageSize: {PageSize}", page, pageSize);
            var response = await _musicService.GetAllAsync(page, pageSize);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/music/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MusicResponseDto), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid music ID.");

            _logger.LogInformation("Fetching music track by ID: {Id}", id);
            var response = await _musicService.GetByIdAsync(id);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/music/albums/3
        [HttpGet("albums/{albumId:int}")]
        [ProducesResponseType(typeof(IEnumerable<MusicResponseDto>), (int)ResponseCode.Success)]
        public async Task<IActionResult> GetByAlbum([FromRoute] int albumId)
        {
            if (albumId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid album ID.");

            _logger.LogInformation("Fetching music tracks by Album ID: {AlbumId}", albumId);
            var response = await _musicService.GetByAlbumAsync(albumId);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/music/genres/Rock
        [HttpGet("genres/{genre}")]
        [ProducesResponseType(typeof(IEnumerable<MusicResponseDto>), (int)ResponseCode.Success)]
        public async Task<IActionResult> GetByGenre([FromRoute] string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return StatusCode((int)ResponseCode.BadRequest, "Genre is required.");

            _logger.LogInformation("Fetching music tracks by Genre: {Genre}", genre);
            var response = await _musicService.GetByGenreAsync(genre);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/music/artists/7
        [HttpGet("artists/{artistId:int}")]
        [ProducesResponseType(typeof(IEnumerable<MusicResponseDto>), (int)ResponseCode.Success)]
        public async Task<IActionResult> GetByArtist([FromRoute] int artistId)
        {
            if (artistId <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid artist ID.");

            _logger.LogInformation("Fetching music tracks by Artist ID: {ArtistId}", artistId);
            var response = await _musicService.GetByArtistAsync(artistId);
            return ResponseMapper.Map(this, response);
        }

        // GET: api/music/5/exists
        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(typeof(bool), (int)ResponseCode.Success)]
        public async Task<IActionResult> Exists([FromRoute] int id)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid music ID.");

            _logger.LogInformation("Checking existence of music track ID: {Id}", id);
            var response = await _musicService.ExistsAsync(id);
            return ResponseMapper.Map(this, response);
        }

        #endregion

        #region ➕ Creation & Update

        // POST: api/music
        [HttpPost]
        [ProducesResponseType(typeof(MusicResponseDto), (int)ResponseCode.Created)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> Add([FromBody] MusicDto dto)
        {
            if (dto == null)
                return StatusCode((int)ResponseCode.BadRequest, "Music data is required.");

            _logger.LogInformation("Adding new music track.");
            var response = await _musicService.AddAsync(dto);
            return ResponseMapper.Map(this, response);
        }

        // POST: api/music/manual
        [HttpPost("manual")]
        [ProducesResponseType(typeof(MusicResponseDto), (int)ResponseCode.Created)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> AddById([FromBody] MusicResponseDto dto)
        {
            if (dto == null || dto.Id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Valid music ID and data are required.");

            _logger.LogInformation("Adding music track with manual ID: {Id}", dto.Id);
            var response = await _musicService.AddManuallyAsync(dto);
            return ResponseMapper.Map(this, response);
        }

        // PUT: api/music/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(MusicResponseDto), (int)ResponseCode.Success)]
        [ProducesResponseType((int)ResponseCode.BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MusicDto dto)
        {
            if (id <= 0 || dto == null)
                return StatusCode((int)ResponseCode.BadRequest, "Valid music ID and update data are required.");

            _logger.LogInformation("Updating music track ID: {Id}", id);
            var response = await _musicService.UpdateAsync(id, dto);
            return ResponseMapper.Map(this, response);
        }

        #endregion

        #region ❌ Deletion

        // DELETE: api/music/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(bool), (int)ResponseCode.NoContent)]
        [ProducesResponseType((int)ResponseCode.NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0)
                return StatusCode((int)ResponseCode.BadRequest, "Invalid music ID.");

            _logger.LogInformation("Deleting music track ID: {Id}", id);
            var response = await _musicService.DeleteAsync(id);
            return ResponseMapper.Map(this, response);
        }

        #endregion
    }
}
