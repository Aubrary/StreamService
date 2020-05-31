using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamService.Helpers;

namespace StreamService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class StreamingController : Controller
    {
        private AppSettings _settings;

        public StreamingController(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
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

            var path = GetFilePath(model.SongId);
            
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

            var songId = id; // Should be generic type.
            
            var path = GetFilePath(songId.Value);
            
            // System.Console.WriteLine(path);
            
            var memory = new MemoryStream();
            await using (var stream = new FileStream(path, FileMode.Open))  
            {  
                await stream.CopyToAsync(memory);  
            }  
            memory.Position = 0;

            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetFilePath(Guid songId)
        {
            return Path.Combine(_settings.StorageRootPath, "Files", songId.ToString(), ".mp3");
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