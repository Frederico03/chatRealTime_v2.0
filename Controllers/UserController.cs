using System.Text.Json;
using ChatRealTime.Data;
using ChatRealTime.DTOs;
using ChatRealTime.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ChatRealTime.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User>? _users;

        public UserController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("users");
        }

        [HttpGet("allusers/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers(string id)
        {
            try
            {
                if (_users != null)
                {
                    var users = await _users.Find(user => user.Id != id)
                                            .Project(user => new { user.Email, user.Username, user.AvatarImage, user.Id })
                                            .ToListAsync();
                    return Ok(new { users });
                }
                else
                {
                    return BadRequest(new { message = "A coleção de usuários não foi inicializada.", status = false });
                }
            }
            catch (Exception ex)
            {
                // Handle the error here
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            if (userRegisterDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var filterName = Builders<User>.Filter.Eq(user => user.Username, userRegisterDto.Username);
                var userInDb = await _users.FindAsync(filterName).Result.ToListAsync();
                string? usernameCheck = userInDb[0].Username;
                if (usernameCheck != null)
                {
                    return BadRequest(new { message = "Usuário já está sendo utilizado", status = false });
                }

                var emailCheck = userInDb[0].Email;
                if (emailCheck != null)
                {
                    return BadRequest(new { message = "Email já está sendo utilizado", status = false });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password, 10);

                var user = new User
                {
                    Email = userRegisterDto.Email,
                    Username = userRegisterDto.Username,
                    Password = hashedPassword,
                };

                await _users.InsertOneAsync(user);

                return Ok(new { status = true, user });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var filterName = Builders<User>.Filter.Eq(user => user.Username, loginDto.Username);
                var userArray = await _users.FindAsync(filterName).Result.ToListAsync();
                var user = userArray[0];
                var usernameCheck = user.Username;
                if (usernameCheck == null)
                {
                    return BadRequest(new { message = "Usuário ou senha incorreto!", status = false });
                }

                string? hashPassword = user.Password;
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, hashPassword);
                if (!isPasswordValid)
                {
                    return BadRequest(new { message = "Usuário ou senha incorreto!", status = false });
                }
                return Ok(new { status = true, user });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost("setAvatar/{id}")]
        public async Task<IActionResult> setAvatar(string id, [FromBody] ImageDto imageDto)
        {
            if (imageDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var filter = Builders<User>.Filter.Eq(user => user.Id, id);
                var update = Builders<User>.Update
                    .Set(u => u.IsAvatarImageSet, true)
                    .Set(u => u.AvatarImage, imageDto.Image);

                var result = await _users?.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    return NotFound();
                }

                var user = await _users.Find(filter).FirstOrDefaultAsync();

                return Ok(new { isSet = user.IsAvatarImageSet, image = imageDto.Image });
            }
            catch (System.Exception)
            {
                throw;
            }
        }


    }
}