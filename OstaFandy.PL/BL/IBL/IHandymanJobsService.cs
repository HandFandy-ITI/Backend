using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IHandymanJobsService
    {
        PaginationHelper<HandymanJobsDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, int? handymanId = null);
        
        bool SentNotificationToClientToUpdataStatus(int jobId, string status);

        public bool AddQuote(int jobId, decimal price, string Notes);
        PaginationHelper<AllQuotes> GetHandymanQuotes(int handymanId, int pageNumber, int pageSize, string searchString);
    }
}
