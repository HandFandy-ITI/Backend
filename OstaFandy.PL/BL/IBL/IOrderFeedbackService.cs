using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IOrderFeedbackService
    {
        PaginationHelper<OrderFeedbackDto> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5);
    }
}
