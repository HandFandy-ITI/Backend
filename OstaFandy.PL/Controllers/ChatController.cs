using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // 1. Ensure a chat session exists for a given booking
        [HttpPost("ensure/{bookingId}")]
        public IActionResult EnsureChat(int bookingId)
        {
            var chatId = _chatService.EnsureChatExists(bookingId);
            return Ok(new { chatId });
        }

        // 2. Send a message via HTTP (fallback if SignalR is not available)
        //[HttpPost("send")]
        //public IActionResult Send([FromBody] MessageDTO dto)
        //{
        //    _chatService.SendMessage(dto);
        //    return Ok(new { success = true });
        //}

        [HttpPost("send")]
        [Authorize]
        public IActionResult Send([FromBody] MessageDTO dto)
        {
            var userIdClaim = User.FindFirst("NameIdentifier");
            if (userIdClaim == null) return Unauthorized();

            dto.SenderId = int.Parse(userIdClaim.Value);

            _chatService.SendMessage(dto);
            return Ok(new { message = "Message sent successfully", success = true });
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
