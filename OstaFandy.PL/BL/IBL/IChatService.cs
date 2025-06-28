using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IChatService
    {
        int EnsureChatExists(int bookingId);
        void SendMessage(MessageDTO dto);
        IEnumerable<MessageDTO> GetMessages(int chatId);
    }
}
