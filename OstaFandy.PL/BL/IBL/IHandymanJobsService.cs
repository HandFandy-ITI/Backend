using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IHandymanJobsService
    {
        PaginationHelper<HandymanJobsDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, int? handymanId = null);
        
        bool SentNotificationToClientToUpdataStatus(int jobId, string status);
    }
}
