using OstaFandy.PL.Constants;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IDashboardService
    {
        public int GetAllCompletedJobCount();
        public decimal GetTotalPrice();
        public decimal GetAverageRating();
        int GetCountOfActiveClient();
        PaginationHelper<DashboardDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, bool? isActive = null, List<DashboardFilter> filters = null);
    }
}
