using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StreamService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamingController : Controller
    {
        public StreamingController()
        {
            
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] SongModel model)
        {
            if (Path.GetExtension(model.File.FileName) != ".mp3")
            {
                return BadRequest();
            }

            if (model.File.ContentType != "audio/mpeg")
            {
                return BadRequest();
            }

            var path = $"/Users/benjaminmlynek/Dropbox/Dokumenter/Skole/Software Engineering (SDU)/Bachelor/00 - Code/StreamService/Files/{model.SongId}.mp3";
            
            await using (var fileStream = new FileStream(path, FileMode.Create)) {
                await model.File.CopyToAsync(fileStream);
            }
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? id)
        {
            
            if (id == null)
            {
                return BadRequest();
            }

            var songId = id.ToString() + ".mp3"; // Should be generic type.
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", songId);
            
            // System.Console.WriteLine(path);
            
            var memory = new MemoryStream();
            await using (var stream = new FileStream(path, FileMode.Open))  
            {  
                await stream.CopyToAsync(memory);  
            }  
            memory.Position = 0;

            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        
        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".mp3", "audio/mpeg"}
            };
        }

        public class SongModel
        {
            public Guid SongId { get; set; }
            public IFormFile File { get; set; }
        }
    }
}