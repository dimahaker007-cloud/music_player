using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class muzicControls : ControllerBase
{
   private readonly IMusicService _musicService;

   public muzicControls(IMusicService musicService)
   {
      _musicService = musicService;
   }

   // GET: api/muziccontrols
   [HttpGet]
   public async Task<ActionResult<IEnumerable<MusicDto>>> GetAllMusic()
   {
      try
      {
         var musicList = await _musicService.GetAllMusicAsync();
            
         var musicDtos = musicList.Select(m => new MusicDto
         {
            id = m.id,
            name = m.name,
            artist = m.artist,
            
         });
            
         return Ok(musicDtos);
      }
      catch (Exception ex)
      {
         return StatusCode(500, $"Internal server error: {ex.Message}");
      }
   }

   // GET: api/muziccontrols/5
   [HttpGet("{id}")]
   public async Task<ActionResult<MusicDto>> GetMusicById(int id)
   {
      try
      {
         var music = await _musicService.GetMusicByIdAsync(id);
            
         if (music == null)
            return NotFound();
                
         var musicDto = new MusicDto
         {
            id = music.id,
            name = music.name,
            artist = music.artist,
         };
            
         return Ok(musicDto);
      }
      catch (Exception ex)
      {
         return StatusCode(500, $"Internal server error: {ex.Message}");
      }
   }
}
