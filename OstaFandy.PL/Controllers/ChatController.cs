using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.Hubs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;

        }

        // 1. Ensure a chat session exists for a given booking
        [HttpPost("ensure/{bookingId}")]
        public IActionResult EnsureChat(int bookingId)
        {
            var chatId = _chatService.EnsureChatExists(bookingId);
            return Ok(new { chatId });
        }


        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> Send([FromBody] MessageDTO dto)
        {
            var userIdClaim = User.FindFirst("NameIdentifier");
            if (userIdClaim == null) return Unauthorized();

            dto.SenderId = int.Parse(userIdClaim.Value);
            _chatService.SendMessage(dto); // Save message

            // ✅ Fetch latest from DB (to include SentAt, SenderName, etc.)
            var fullMessage = _chatService.GetMessages(dto.ChatId)
                                          .OrderByDescending(m => m.SentAt)
                                          .FirstOrDefault();

            if (fullMessage == null)
                return StatusCode(500, "Failed to retrieve saved message");

            // ✅ Broadcast full message
            await _hubContext.Clients.Group($"chat-{dto.ChatId}")
                .SendAsync("ReceiveMessage", fullMessage);

            return Ok(new { message = "Message sent", success = true });
        }




        // 3. Get full message history for a chat
        [HttpGet("history/{chatId}")]
        public IActionResult GetHistory(int chatId)
        {
            var history = _chatService.GetMessages(chatId);
            return Ok(history);
        }



  
        [HttpGet("client/threads")]
        [Authorize(AuthenticationSchemes = "myschema")]
        public IActionResult GetClientThreads([FromQuery] ChatThreadFilterDTO filters)
        {
            var userId = User.FindFirst("NameIdentifier")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = _chatService.GetClientThreads(int.Parse(userId), filters);
            return Ok(result);
        }

        [HttpGet("handyman/threads")]
        [Authorize(AuthenticationSchemes = "myschema")]
        public IActionResult GetHandymanThreads([FromQuery] ChatThreadFilterDTO filters)
        {
            var userId = User.FindFirst("NameIdentifier")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = _chatService.GetHandymanThreads(int.Parse(userId), filters);
            return Ok(result);
        }



        [HttpGet("threads")]
        [Authorize(AuthenticationSchemes = "myschema")]
        public IActionResult GetClientThreads([FromQuery] string name = "", [FromQuery] string sort = "newest", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userIdClaim = User.FindFirst("NameIdentifier");
            if (userIdClaim == null) return Unauthorized();

            int clientId = int.Parse(userIdClaim.Value);

            var filters = new ChatThreadFilterDTO
            {
                Name = name,
                Sort = sort,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var threads = _chatService.GetClientThreads(clientId, filters);
            return Ok(threads);
        }


        [HttpGet("threads/handyman")]
        [Authorize(AuthenticationSchemes = "myschema")]
        public IActionResult GetFilteredHandymanThreads(
    [FromQuery] string name = "",
    [FromQuery] string sort = "newest",
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 5)
        {
            var userIdClaim = User.FindFirst("NameIdentifier");
            if (userIdClaim == null) return Unauthorized();

            int handymanId = int.Parse(userIdClaim.Value);

            var filters = new ChatThreadFilterDTO
            {
                Name = name,
                Sort = sort,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var threads = _chatService.GetHandymanThreads(handymanId, filters);
            return Ok(threads);
        }

    }
}
