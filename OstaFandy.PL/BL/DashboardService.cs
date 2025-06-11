using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using System.Linq.Expressions;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;
using AutoMapper;
using OstaFandy.PL.Constants;

namespace OstaFandy.PL.BL
{
    public class DashboardService : IDashboardService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger, IMapper mapper)
        {
            _mapper = mapper;
            this.unitOfWork = unitOfWork;
            _logger = logger;
        }
        #region Statisic
        public int GetAllCompletedJobCount()
        {
            try
            {
                return unitOfWork.BookingRepo.GetAllCompletedJobCount();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching completed job count.");
                throw;
            }
        }
        public decimal GetTotalPrice()
        {
            try
            {
                return unitOfWork.BookingRepo.GetTotalPrice();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching total price.");
                throw;
            }
        }
        public decimal GetAverageRating()
        {
            try
            {
                return unitOfWork.ReviewRepo.GetAverageRating();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching average rating.");
                throw;
            }
        }

        public int GetCountOfActiveClient()
        {
            try
            {
                return unitOfWork.UserRepo.GetCountOfActiveClient();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching count of active clients.");
                throw;
            }

        }
        #endregion

        public PaginationHelper<DashboardDTO> GetAll(
        string searchString = "",
        int pageNumber = 1,
        int pageSize = 5,
        bool? isActive = null,
        List<DashboardFilter> filters = null)
        {
            try
            {
                var bookingsQuery = unitOfWork.BookingRepo.GetAll(
                    b => b.Status == "Completed",
                    "Client,Client.User,JobAssignment,JobAssignment.Handyman,JobAssignment.Handyman.User," +
                    "BookingServices,BookingServices.Service,BookingServices.Service.Category," +
                    "Address,Reviews,Payments"
                ).AsQueryable();

                if (filters != null && filters.Any())
                {
                    bookingsQuery = ApplyFilters(bookingsQuery, filters);
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    bookingsQuery = ApplySearchFilter(bookingsQuery, searchString);
                }

                if (isActive.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Client.User.IsActive == isActive.Value);
                }

                var bookings = bookingsQuery.ToList();

                var dashboardDTOs = _mapper.Map<List<DashboardDTO>>(bookings);

                return PaginationHelper<DashboardDTO>.Create(dashboardDTOs, pageNumber, pageSize, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching dashboard data.");
                throw;
            }
        }

        private IQueryable<Booking> ApplyFilters(IQueryable<Booking> query, List<DashboardFilter> filters)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case DashboardFilter.Today:
                        query = query.Where(b => b.CreatedAt.Date == today);
                        break;
                    case DashboardFilter.ThisWeek:
                        query = query.Where(b => b.CreatedAt.Date >= startOfWeek);
                        break;
                    case DashboardFilter.ThisMonth:
                        query = query.Where(b => b.CreatedAt.Date >= startOfMonth);
                        break;
                    case DashboardFilter.RevenueGreaterThan1000:
                        query = query.Where(b => b.TotalPrice > 1000);
                        break;
                    case DashboardFilter.JobsGreaterThan10:
                        var handymenWithManyJobs = unitOfWork.JobAssignmentRepo.GetAll()
                            .Where(ja => ja.Status == "Completed")
                            .GroupBy(ja => ja.HandymanId)
                            .Where(g => g.Count() > 10)
                            .Select(g => g.Key)
                            .ToList();

                        query = query.Where(b => b.JobAssignment != null &&
                                               handymenWithManyJobs.Contains(b.JobAssignment.HandymanId));
                        break;
                    case DashboardFilter.RatingGreaterThan4:
                        query = query.Where(b =>
                            b.Reviews.Any() && b.Reviews.Average(r => r.Rating) > 4);
                        break;
                    case DashboardFilter.PendingApproval:
                        query = query.Where(b => b.JobAssignment != null &&
                                               b.JobAssignment.Handyman.Status == "Pending");
                        break;
                }
            }

            return query;
        }

        private IQueryable<Booking> ApplySearchFilter(IQueryable<Booking> query, string searchString)
        {
            var searchLower = searchString.ToLower();
            return query.Where(b =>
                b.Client.User.FirstName.ToLower().Contains(searchLower) ||
                b.Client.User.LastName.ToLower().Contains(searchLower) ||
                (b.JobAssignment != null &&
                 (b.JobAssignment.Handyman.User.FirstName.ToLower().Contains(searchLower) ||
                  b.JobAssignment.Handyman.User.LastName.ToLower().Contains(searchLower))) ||
                b.BookingServices.Any(bs => bs.Service.Name.ToLower().Contains(searchLower)) ||
                b.BookingServices.Any(bs => bs.Service.Category.Name.ToLower().Contains(searchLower)) ||
                b.Address.City.ToLower().Contains(searchLower)
            );
        }
    }

}

