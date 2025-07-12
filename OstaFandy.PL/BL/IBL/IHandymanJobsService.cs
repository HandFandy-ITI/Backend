using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IHandymanJobsService
    {
        PaginationHelper<HandymanJobsDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, int? handymanId = null);
        
         bool  SentNotificationToClientToUpdataStatus(int jobId, string status);

         Task<bool> AddQuote(int jobId, decimal price, string Notes, int EstimatedMinutes);
        PaginationHelper<AllQuotes> GetHandymanQuotes(int handymanId, int pageNumber, int pageSize, string searchString);
        public Task<bool> ProcessQuoteResponseAsync(int quoteId, string action, int clientUserId, CreateBookingDTO bookingDto = null);
        public QuoteDetailsDTO GetQuoteDetails(int quoteId);

          Task<bool> ApplyForBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate);
        PaginationHelper<HandymanBlockDateDTO> GetHandymanBlockDate(int pageNumber = 1, int pageSize = 5, string? status = null, DateTime? Date = null, int? handymanId = null, string searchString = "");

    }
}
