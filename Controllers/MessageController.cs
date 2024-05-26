using ChatRealTime.Data;
using ChatRealTime.DTOs;
using ChatRealTime.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ChatRealTime.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMongoCollection<Messages>? _message;

        public MessagesController(MongoDbService mongoDbService)
        {
            _message = mongoDbService.Database?.GetCollection<Messages>("messages");
        }

        [HttpPost("addmsg/")]
        public async Task<IActionResult> AddMessage([FromBody] AddMessageDto req)
        {
            if (req == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var message = new Messages
                {
                    Content = new MessageText { Text = req.Message },
                    Users = [req.From, req.To],
                    Sender = req.From,
                    Timestamp = DateTime.Now
                };

                await _message.InsertOneAsync(message);

                return Ok(new { msg = "Mensagem adicionada com sucesso." });
            }
            catch (System.Exception)
            {
                return StatusCode(500, new { msg = "Erro em adicionar mensagem no banco de dados." });
            }
        }

        [HttpPost("getmsg/")]
        public async Task<IActionResult> GetAllMessage([FromBody] GetAllMessageDto req)
        {
            if (req == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var filter = Builders<Messages>.Filter.All("users", new[] { req.From, req.To });
                var sort = Builders<Messages>.Sort.Ascending("updatedAt");

                var messages = await _message
                                    .Find(filter)
                                    .Sort(sort)
                                    .ToListAsync();

                var projectedMessages = messages.Select(msg => new
                {
                    fromSelf = msg.Sender == req.From,
                    message = msg.Content.Text
                });

                return Ok(projectedMessages);
            }
            catch (System.Exception)
            {
                return StatusCode(500, new { msg = "Erro em adicionar mensagem no banco de dados." });
            }
        }
    }
}