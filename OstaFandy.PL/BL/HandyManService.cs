using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL
{
    public class HandyManService : IHandyManService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HandyManService> _logger;
        private readonly IMapper _mapper;
        public HandyManService(IUnitOfWork unitOfWork, ILogger<HandyManService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public PaginationHelper<Handyman> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var data = _unitOfWork.HandyManRepo.GetAll(includeProperties: "User,Specialization,DefaultAddress,BlockDates,JobAssignments").ToList();

                if (data == null)
                {
                    return new PaginationHelper<Handyman>
                    {
                        Data = new List<Handyman>(),
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        TotalCount = 0,
                        SearchString = searchString
                    };
                }

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchString))
                {
                    data = data.Where(h =>
                        h.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.User.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        h.Specialization.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                return PaginationHelper<Handyman>.Create(data, pageNumber, pageSize, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all handymen");
                return new PaginationHelper<Handyman>
                {
                    Data = new List<Handyman>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0,
                    SearchString = searchString
                };
            }
        }

        public List<AdminHandyManDTO> GetAllPendingHandymen()
        {
            try
            {
                var pendingHandymen = _unitOfWork.HandyManRepo.GetAll(h => h.Status == "Pending", "User,Specialization,DefaultAddress,BlockDates,JobAssignments").ToList();
                return _mapper.Map<List<AdminHandyManDTO>>(pendingHandymen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all pending handymen");
                return new List<AdminHandyManDTO>();
            }
        }
        public async Task<bool> UpdateHandymanStatusById(int userId, string status)
        {
            try
            {
                var handyman = _unitOfWork.HandyManRepo.GetById(userId);
                if (handyman == null)
                {
                    _logger.LogWarning($"Handyman with ID {userId} not found");
                    return false;
                }
                handyman.Status = status;
                _unitOfWork.HandyManRepo.Update(handyman);
                _unitOfWork.Save();
                _logger.LogInformation($"Handyman {userId} status updated to {status}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating handyman status for UserId: {userId}");
                return false;
            }
        }
    }
}

