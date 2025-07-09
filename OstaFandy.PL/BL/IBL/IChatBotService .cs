using OstaFandy.DAL.Entities;

namespace OstaFandy.PL.BL.IBL
{
    public interface IChatBotService
    {
        Task<string> ChatbotHandeller(string userId, String userMessage);
    }
}
