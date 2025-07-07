using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OstaFandy.PL.BL
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ClientService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public PaginationHelper<AdminDisplayClientDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, bool? isActive = null)
        {
            Expression<Func<User, bool>> filter = u => u.Client != null;

            if (!string.IsNullOrEmpty(searchString) && isActive.HasValue)
            {
                var searchLower = searchString.ToLower();
                filter = u => u.Client != null &&
                             u.IsActive == isActive.Value &&
                             (u.FirstName.ToLower().Contains(searchLower) ||
                              u.LastName.ToLower().Contains(searchLower) ||
                              u.Email.ToLower().Contains(searchLower) ||
                              u.Phone.Contains(searchString));
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                filter = u => u.Client != null &&
                             (u.FirstName.ToLower().Contains(searchLower) ||
                              u.LastName.ToLower().Contains(searchLower) ||
                              u.Email.ToLower().Contains(searchLower) ||
                              u.Phone.Contains(searchString));
            }
            else if (isActive.HasValue)
            {
                filter = u => u.Client != null && u.IsActive == isActive.Value;
            }

            var users = _unitOfWork.UserRepo.GetAll(filter, "Client,Client.DefaultAddress,Addresses,Client.Bookings,Client.Bookings.Payments").ToList();
            var clientDTOs = _mapper.Map<List<AdminDisplayClientDTO>>(users);
            return PaginationHelper<AdminDisplayClientDTO>.Create(clientDTOs, pageNumber, pageSize, searchString);
        }

        public AdminDisplayClientDTO GetById(int id)
        {
            var user = _unitOfWork.UserRepo.GetAll(u => u.Id == id && u.Client != null,
                "Client,Client.DefaultAddress,Addresses,Client.Bookings,Client.Bookings.Payments").FirstOrDefault();

            if (user == null)
                return null;

            return _mapper.Map<AdminDisplayClientDTO>(user);
        }

        public AdminEditClientDTO EditClientDTO(AdminEditClientDTO editClientDto)
        {
            try
            {

                var client = _unitOfWork.UserRepo.FirstOrDefault(a => a.Id == editClientDto.Id, "Client,Addresses");
                if (client == null)
                {
                    throw new KeyNotFoundException("Client not found");
                }
                _mapper.Map(editClientDto, client);
                _unitOfWork.UserRepo.Update(client);
                _unitOfWork.Save();
                return editClientDto;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the client data", ex);
            }

        }

        public bool DeleteClient(int id)
        {
            try
            {
                bool success = _unitOfWork.UserRepo.SoftDelete(id);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting handyman with id {Id}", id);
                throw;
            }
        }



        public async Task<ClientProfileDTO> GetClientProfile(int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    throw new ArgumentException("Invalid client ID.");
                }

                var client = await _unitOfWork.ClientRepo.GetClientWithProfileData(clientId);

                if (client == null)
                {
                    throw new KeyNotFoundException($"Client with ID {clientId} not found.");
                }

                var clientProfile = _mapper.Map<ClientProfileDTO>(client);
                return clientProfile;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is KeyNotFoundException))
            {
                throw new ApplicationException("An error occurred while retrieving client profile.", ex);
            }
        }

        public async Task<ClientOrderHistoryDTO> GetClientOrderHistory(int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    throw new ArgumentException("Invalid client ID.");
                }

                var client = await _unitOfWork.ClientRepo.GetClientWithBookingHistory(clientId);
                if (client == null)
                {
                    throw new KeyNotFoundException($"Client with ID {clientId} not found.");
                }

                var orderHistory = new ClientOrderHistoryDTO
                {
                    ClientId = client.UserId,
                    ClientName = $"{client.User.FirstName} {client.User.LastName}",
                    Email = client.User.Email,
                    Phone = client.User.Phone,
                    TotalOrders = client.Bookings?.Count ?? 0,
                    CompletedOrders = client.Bookings?.Count(b => b.Status == "Completed") ?? 0,
                    PendingOrders = client.Bookings?.Count(b => b.Status == "Pending") ?? 0,
                    CancelledOrders = client.Bookings?.Count(b => b.Status == "Cancelled") ?? 0,
                    TotalSpent = client.Bookings?.Where(b => b.Status == "Completed").Sum(b => b.TotalPrice ?? 0) ?? 0,
                    Orders = client.Bookings?.Select(b => new ClientOrderDTO
                    {
                        BookingId = b.Id,
                        OrderDate = b.CreatedAt,
                        PreferredDate = b.PreferredDate,
                        Status = b.Status,
                        TotalPrice = b.TotalPrice ?? 0,
                        EstimatedMinutes = b.EstimatedMinutes ?? 0,
                        Note = b.Note,
                        HandymanName = b.JobAssignment?.Handyman?.User != null
                            ? $"{b.JobAssignment.Handyman.User.FirstName} {b.JobAssignment.Handyman.User.LastName}"
                            : "Not Assigned",
                        Address = new ClientOrderAddressDTO
                        {
                            FullAddress = b.Address.Address1,
                            City = b.Address.City,
                            Latitude = b.Address.Latitude,
                            Longitude = b.Address.Longitude
                        },
                        Services = b.BookingServices?.Select(bs => new ClientOrderServiceDTO
                        {
                            ServiceName = bs.Service.Name,
                            CategoryName = bs.Service.Category.Name,
                            FixedPrice = bs.Service.FixedPrice,
                            EstimatedMinutes = bs.Service.EstimatedMinutes
                        }).ToList() ?? new List<ClientOrderServiceDTO>(),
                        Payment = b.Payments?.FirstOrDefault() != null ? new ClientPaymentDTO
                        {
                            Amount = b.Payments.First().Amount,
                            Method = b.Payments.First().Method,
                            Status = b.Payments.First().Status,
                            PaymentDate = b.Payments.First().CreatedAt
                        } : null,
                        Review = b.Reviews?.FirstOrDefault() != null ? new ClientReviewDTO
                        {
                            Rating = b.Reviews.First().Rating,
                            Comment = b.Reviews.First().Comment,
                            ReviewDate = b.Reviews.First().CreatedAt
                        } : null
                    }).OrderByDescending(o => o.OrderDate).ToList() ?? new List<ClientOrderDTO>()
                };

                return orderHistory;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is KeyNotFoundException))
            {
                throw new ApplicationException("An error occurred while retrieving client order history.", ex);
            }
        }

        public async Task<List<ClientQuoteDTO>> GetClientQuotes(int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    throw new ArgumentException("Invalid client ID.");
                }

                var client = await _unitOfWork.ClientRepo.GetClientWithBookingHistory(clientId);
                if (client == null)
                {
                    throw new KeyNotFoundException($"Client with ID {clientId} not found.");
                }

                var quotes = new List<ClientQuoteDTO>();

                if (client.Bookings != null)
                {
                    foreach (var booking in client.Bookings)
                    {
                        if (booking.JobAssignment?.Quotes != null)
                        {
                            foreach (var quote in booking.JobAssignment.Quotes)
                            {
                                quotes.Add(new ClientQuoteDTO
                                {
                                    QuoteId = quote.Id,
                                    BookingId = booking.Id,
                                    HandymanName = booking.JobAssignment.Handyman?.User != null
                                        ? $"{booking.JobAssignment.Handyman.User.FirstName} {booking.JobAssignment.Handyman.User.LastName}"
                                        : "Unknown",
                                    handymanId = booking.JobAssignment.HandymanId,
                                    addressId = booking.AddressId,
                                    Price = quote.Price,
                                    Notes = quote.Notes,
                                    Status = quote.Status,
                                    CreatedAt = quote.CreatedAt,
                                    BookingDate = booking.PreferredDate,
                                    Services = booking.BookingServices?.Select(bs => bs.Service.Name).ToList() ?? new List<string>(),
                                    CategoryName = booking.BookingServices?.FirstOrDefault()?.Service?.Category?.Name ?? "Unknown"
                                });
                            }
                        }
                    }
                }

                return quotes.OrderByDescending(q => q.CreatedAt).ToList();
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is KeyNotFoundException))
            {
                throw new ApplicationException("An error occurred while retrieving client quotes.", ex);
            }
        }

        public async Task<bool> UpdateClientProfile(int clientId, UpdateClientProfileDTO updateDto)
        {
            if (clientId <= 0)
            {
                throw new ArgumentException("Invalid client ID.");
            }

            var client = await _unitOfWork.ClientRepo.GetClientByUserId(clientId);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client with ID {clientId} not found.");
            }

            var user = await _unitOfWork.UserRepo.GetByIdAsync(client.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {client.UserId} not found.");
            }

            if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                user.FirstName = updateDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                user.LastName = updateDto.LastName;

            if (!string.IsNullOrWhiteSpace(updateDto.Phone))
                user.Phone = updateDto.Phone;

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
            {
                var existingUser = _unitOfWork.UserRepo.GetAll(u => u.Email == updateDto.Email && u.Id != user.Id).FirstOrDefault();
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email is already in use by another user.");
                }
                user.Email = updateDto.Email;
            }

            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserRepo.Update(user);
            var result = await _unitOfWork.SaveAsync();

            return result > 0;
        }

        

    }
    

    

    
    
}
