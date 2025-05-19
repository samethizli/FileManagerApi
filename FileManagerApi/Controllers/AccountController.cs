using Microsoft.AspNetCore.Mvc;
using FileManagerApi.Models;

namespace FileManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private static readonly List<User> _users = new();

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_users.Any(u => u.Username == user.Username))
                return BadRequest("Bu kullanıcı adı zaten kayıtlı.");

            _users.Add(user);
            return Ok("Kayıt işlemi başarılı.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var matchedUser = _users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (matchedUser == null)
                return Unauthorized("Kullanıcı adı veya şifre hatalı.");

            HttpContext.Session.SetString("username", matchedUser.Username);
            return Ok("Giriş başarılı.");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return Ok("Oturum sonlandırıldı.");
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var username = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(username))
                return Unauthorized("Giriş yapılmamış.");

            return Ok(new { username });
        }

    }
}
