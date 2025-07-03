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



        [HttpGet("handyman/threads")]
        [Authorize(AuthenticationSchemes = "myschema")]
        public IActionResult GetHandymanThreads()
        {
            var userIdClaim = User.FindFirst("NameIdentifier");
            if (userIdClaim == null) return Unauthorized();

            int handymanId = int.Parse(userIdClaim.Value);
            var threads = _chatService.GetHandymanThreads(handymanId);
            return Ok(threads);
        }





    }
}
