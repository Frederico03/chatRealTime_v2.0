using ChatRealTime.Data;
using ChatRealTime.DTOs;
using ChatRealTime.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BCrypt.Net;
using System.Text.Json;

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
                    return Ok(users);
                }
                else
                {
                    return NotFound("A coleção de usuários não foi inicializada.");
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
                var usernameCheck = _users.Find(filterName).FirstOrDefault();
                if (usernameCheck != null)
                {
                    return BadRequest(new { message = "Usuário já está sendo utilizado" });
                }

                var filterEmail = Builders<User>.Filter.Eq(user => user.Email, userRegisterDto.Email);
                var emailCheck = _users.Find(filterEmail).FirstOrDefault();
                if (emailCheck != null)
                {
                    return BadRequest(new { message = "Email já está sendo utilizado" });
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


    }
}