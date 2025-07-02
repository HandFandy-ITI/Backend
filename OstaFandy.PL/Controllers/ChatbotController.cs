using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatBotService _ChatBotService;

        public ChatbotController(IChatBotService ChatBotService)
        {
            _ChatBotService = ChatBotService;
        }

        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestService([FromBody] UserChatRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("UserId and Message are required.");


            var suggestion = await _ChatBotService.ChatbotHandeller(dto.UserId,dto.Message);

            return Ok(new { suggestedService = suggestion.Trim() });
        }
    }

}
