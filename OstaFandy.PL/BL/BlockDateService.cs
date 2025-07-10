using System.Collections.Immutable;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OstaFandy.PL.BL
{
    public class BlockDateService : IBlockDateService
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public BlockDateService(IMapper mapper, ILogger<BlockDateService> logger, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #region get all block dates
        public PaginationHelper<BlockDateDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, DateTime? Date = null)
        {
            try
            {
                var blockDates = _unitOfWork.BlockDateRepo.GetAll(includeProperties: "User.User").AsQueryable();
                if (!string.IsNullOrEmpty(searchString))
                {
                    blockDates = blockDates
                        .Where(b => b.User.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                    b.User.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                    b.User.User.Phone.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }
                if(Date!= null)
                {
                    blockDates = blockDates
                        .Where(b => b.StartDate.Date == Date || b.EndDate.Date == Date);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    blockDates = blockDates
                        .Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                }
                var x = blockDates.Select(b => new BlockDateDTO
                {
                    UserId = b.Id,
                    Email = b.User.User.Email ?? "",
                    Phone = b.User.User.Phone ?? "",
                    Name = $"{b.User.User.FirstName ?? ""} {b.User.User.LastName ?? ""}",
                    Status = b.Status,
                    Reason = b.Reason,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                });
                var dtolist = x.ToList();
                return PaginationHelper<BlockDateDTO>.Create(x, pageNumber, pageSize, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting block dates.");
                throw;
            }
        }

        public PaginationHelper<HandymanSummaryDTO> GetAllHandymanData(string searchString = "", int pageNumber = 1, int pageSize = 5, int? categoryId = null)
        {
            try
            {
                var data = _unitOfWork.HandyManRepo.GetAll(a => a.Status == "Approved", includeProperties: "User,Specialization,DefaultAddress,BlockDates,JobAssignments").ToList();

                if (data == null)
                {
                    return new PaginationHelper<HandymanSummaryDTO>
                    {
                        Data = new List<HandymanSummaryDTO>(),
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        TotalCount = 0,
                        SearchString = searchString
                    };
                }

 
                var x = data.Select(b => new HandymanSummaryDTO()
                {
                    UserId = b.UserId,
                    Name = $"{b.User.FirstName} {b.User.LastName}", 
                    Email = b.User.Email,
                    Phone = b.User.Phone,
                    Specialization = b.Specialization.Name,
                    SpecializationId = b.SpecializationId
                }).ToList(); 

                if (!string.IsNullOrEmpty(searchString))
                {
                    x = x.Where(h =>
                        h.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.Phone.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.Specialization.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if(categoryId != null)
                {
                    x = x.Where(a => a.SpecializationId == categoryId).ToList();
                }


                return PaginationHelper<HandymanSummaryDTO>.Create(x, pageNumber, pageSize, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all handymen");
                return new PaginationHelper<HandymanSummaryDTO>
                {
                    Data = new List<HandymanSummaryDTO>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0,
                    SearchString = searchString
                };
            }
        }
        #endregion

        public List<Category> GetAllCategory()
        {
            try
            {
                var catetories = _unitOfWork.CategoryRepo.GetAll(a => a.IsActive == true).ToList();
                if(catetories == null)
                {
                    catetories = new List<Category>();
                    return catetories;
                }
                return catetories;

            }catch(Exception ex)
            {
                _logger.LogError(ex, "error while get all catetory");
                throw;
            }
        }


        public async Task<bool> AddBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            try
            {
                var handyman = _unitOfWork.HandyManRepo.FirstOrDefault(h => h.UserId == HandymanId, includeProperties:"User");
                if (handyman == null)
                {
                    _logger.LogError("handyman does not exist");
                    return false;
                }
                if (!handyman.User.IsActive)
                {
                    _logger.LogError("handyman is not active");
                    return false;
                }
                var jobs = _unitOfWork.JobAssignmentRepo.CheckJobInSpecificDate(StartDate, EndDate);
                if (jobs)
                {
                    _logger.LogError("Handyman has jobs in this period");
                    return false;
                }

                var existingBlockDates = _unitOfWork.BlockDateRepo.GetAll(a => a.UserId == HandymanId);
                foreach (var blockDate in existingBlockDates)
                {
                    if (AreDatesOverlapping(DateOnly.FromDateTime(blockDate.StartDate),DateOnly.FromDateTime(blockDate.EndDate), StartDate,EndDate))
                    {
                        _logger.LogError("Handyman already has a vacation in this period");
                        return false;
                    }
                }

                var x = new BlockDate()
                    {
                        UserId = HandymanId,
                        Reason = Reason,
                        StartDate = StartDate.ToDateTime(TimeOnly.MinValue),
                        EndDate = EndDate.ToDateTime(TimeOnly.MaxValue),
                        IsActive = true,
                        Status = "Approved"
                    };
                    _unitOfWork.BlockDateRepo.Insert(x);
                    _unitOfWork.Save();
               await _notificationService.SendNotificationToHandyman(HandymanId.ToString(), $"you have days off start from {StartDate} to {EndDate}");
                var notification = new Notification
                {
                    UserId = HandymanId,
                    Title = "Days OFF",
                    Message = $"you have days off start from {StartDate} to {EndDate}",
                    Type = "vacation",
                    IsRead = false,
                    IsActive = true
                };
                _unitOfWork.NotificationRepo.Insert(notification);
                _unitOfWork.Save();
                return true;
                
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "error will adding block date");
                throw;
            }
         }
        private bool AreDatesOverlapping(DateOnly startDate1, DateOnly endDate1, DateOnly startDate2, DateOnly endDate2)
        {
            return !(endDate1 < startDate2 || endDate2 < startDate1);
        }

        public bool RejectBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            try
            {
                var blockDate = _unitOfWork.BlockDateRepo
                    .GetAll(a => a.Reason.ToLower().Trim() == Reason.ToLower().Trim())
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                if (blockDate == null)
                {
                    _logger.LogWarning("BlockDate not found for HandymanId: {HandymanId}, StartDate: {StartDate}, EndDate: {EndDate}, Reason: {Reason}", HandymanId, StartDate, EndDate, Reason);
                    return false;
                }

                blockDate.Status = "Denied";
                blockDate.IsActive = false;
                _unitOfWork.BlockDateRepo.Update(blockDate);

                var originalNotification = _unitOfWork.NotificationRepo
                    .GetAll(n => n.Type.Contains($"{HandymanId},{Reason}"))
                    .FirstOrDefault();
                if (originalNotification != null)
                {
                    originalNotification.IsRead = true;
                    _unitOfWork.NotificationRepo.Update(originalNotification);
                }

                _notificationService.SendNotificationToHandyman(HandymanId.ToString(), $"your vacation request has been Denied which was asking for vacation from {StartDate} to {EndDate}");

                var notification = new Notification
                {
                    UserId = HandymanId,
                    Title = "Days OFF",
                    Message = $"you have days off start from {StartDate} to {EndDate}",
                    Type = "vacation",
                    IsRead = false,
                    IsActive = true
                };
                _unitOfWork.NotificationRepo.Insert(notification);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error will adding block date");
                throw;
            }
        }


        public bool ApproveBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            try
            {
                var blockDate = _unitOfWork.BlockDateRepo
                    .GetAll(a => a.Reason.ToLower().Trim() == Reason.ToLower().Trim())
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                if (blockDate == null)
                {
                    _logger.LogWarning("BlockDate not found for HandymanId: {HandymanId}, StartDate: {StartDate}, EndDate: {EndDate}, Reason: {Reason}", HandymanId, StartDate, EndDate, Reason);
                    return false;
                }

                blockDate.Status = "Approved";
                blockDate.IsActive = true;
                _unitOfWork.BlockDateRepo.Update(blockDate);

                var originalNotification = _unitOfWork.NotificationRepo
                    .GetAll(n => n.Type.Contains($"{HandymanId},{Reason}"))
                    .FirstOrDefault();
                if (originalNotification != null)
                {
                    originalNotification.IsRead = true;
                    _unitOfWork.NotificationRepo.Update(originalNotification);
                }

                _notificationService.SendNotificationToHandyman(HandymanId.ToString(), $"your vacation request has been Approved which was asking for vacation from {StartDate} to {EndDate}");

                var notification = new Notification
                {
                    UserId = HandymanId,
                    Title = "Days OFF",
                    Message = $"you have days off start from {StartDate} to {EndDate}",
                    Type = "vacation",
                    IsRead = false,
                    IsActive = true
                };
                _unitOfWork.NotificationRepo.Insert(notification);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error will adding block date");
                throw;
            }
        }
    }
}
