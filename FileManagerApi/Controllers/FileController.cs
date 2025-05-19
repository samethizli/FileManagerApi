using Microsoft.AspNetCore.Mvc;

namespace FileManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly string _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public FileController()
        {
            if (!Directory.Exists(_uploadRoot))
                Directory.CreateDirectory(_uploadRoot);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized("Giriş yapılmamış.");

            if (file == null || file.Length == 0)
                return BadRequest("Yüklenecek dosya bulunamadı.");

            var userPath = Path.Combine(_uploadRoot, username);
            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            var path = Path.Combine(userPath, file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("Dosya başarıyla yüklendi.");
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized("Giriş yapılmamış.");

            var userPath = Path.Combine(_uploadRoot, username);
            if (!Directory.Exists(userPath))
                return Ok(new List<string>());

            var files = Directory.GetFiles(userPath)
                                 .Select(Path.GetFileName)
                                 .ToList();

            return Ok(files);
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string filename)
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized("Giriş yapılmamış.");

            if (string.IsNullOrWhiteSpace(filename))
                return BadRequest("Dosya adı geçersiz.");

            var path = Path.Combine(_uploadRoot, username, filename);
            if (!System.IO.File.Exists(path))
                return NotFound("Dosya bulunamadı.");

            System.IO.File.Delete(path);
            return Ok("Dosya silindi.");
        }
    }
}
