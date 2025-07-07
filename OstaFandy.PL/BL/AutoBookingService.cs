using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.General;
using OstaFandy.PL.utils;
using Stripe;


namespace OstaFandy.PL.BL
{
    public class AutoBookingService : IAutoBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public AutoBookingService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        //get all
        public List<BookingViewDto> GetAllBookings()
        {
            try
            {
                var bookings = _unitOfWork.BookingRepo.GetAll(b=>b.Status != BookingStatus.Cancelled, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");

                if (bookings == null || !bookings.Any())
                {
                    return new List<BookingViewDto>();
                }

                var bookingDtos = _mapper.Map<List<BookingViewDto>>(bookings);

                return bookingDtos;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while retrieving bookings.", ex);
            }
        }

        //get booking by id
        public BookingViewDto GetBookingById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid booking ID.");
                }
                var booking = _unitOfWork.BookingRepo.FirstOrDefault(b => b.Id == id, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");
                if (booking == null)
                {
                    throw new KeyNotFoundException($"Booking with ID {id} not found.");
                }
                var bookingDto = _mapper.Map<BookingViewDto>(booking);
                return bookingDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving the booking.", ex);
            }

        }
        //get booking by client id
        public List<BookingViewDto> GetBookingsByClientId(int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    throw new ArgumentException("Invalid client ID.");
                }
                var bookings = _unitOfWork.BookingRepo.GetAll(b => b.ClientId == clientId, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");
                if (bookings == null || !bookings.Any())
                {
                    return new List<BookingViewDto>();
                }
                var bookingDtos = _mapper.Map<List<BookingViewDto>>(bookings);
                return bookingDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving bookings for the client.", ex);
            }
        }
        //get booking by handyman id
        public List<BookingViewDto> GetBookingsByHandyManId(int HandyManId)
        {
            try
            {
                if (HandyManId <= 0)
                {
                    throw new ArgumentException("Invalid client ID.");
                }
                var bookings = _unitOfWork.BookingRepo.GetAll(b => b.JobAssignment.Handyman.UserId == HandyManId, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");
                if (bookings == null || !bookings.Any())
                {
                    return new List<BookingViewDto>();
                }
                var bookingDtos = _mapper.Map<List<BookingViewDto>>(bookings);
                return bookingDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving bookings for the client.", ex);
            }
        }
        //get free time slot
        public async Task<List<AvailableTimeSlot>> GetAvailableTimeSlotAsync(AvailableTimeSlotsRequestDto reqdata)
        {
            try
            {
                var (slots, areaNotSupported) = await _unitOfWork.BookingRepo
                            .GetAvailableTimeSlotsAsync(reqdata.CategoryId, reqdata.Day, reqdata.UserLatitude, reqdata.UserLongitude, reqdata.EstimatedMinutes);

                if (areaNotSupported)
                {
                    return null;
                }
               
                   return slots;
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting available time slots", ex);
            }
        }
        //create booking
        public async Task<BookingChatResponse?> CreateBooking(CreateBookingDTO bookingdto)
        {
            if (bookingdto == null) return null;

            using var trnsaction = await _unitOfWork.BeginTransactionasync();
            try
            {
                //email
                var email = new EmailContentDto();
                //get user
                var user= _unitOfWork.UserRepo.GetById(bookingdto.ClientId);
                //booking 
                var booking = _mapper.Map<Booking>(bookingdto);
                _unitOfWork.BookingRepo.Insert(booking);
                await _unitOfWork.SaveAsync();

                //job assign
                var jobassign = _mapper.Map<JobAssignment>(bookingdto);
                jobassign.BookingId = booking.Id;
                jobassign.AssignedAt = DateTime.Now;
                jobassign.CreatedAt = DateTime.Now;
                _unitOfWork.JobAssignmentRepo.Insert(jobassign);


                //booking service part
                foreach (var S in bookingdto.serviceDto)
                {
                    var bookingservice = new BookingService
                    {
                        ServiceId = S.ServiceId,
                        BookingId = booking.Id,
                        Quantity = S.Quantity > 0 ? S.Quantity : 1
                    };
                    _unitOfWork.BookingServiceRepo.Insert(bookingservice);
                }

                //payment
                var payment = _mapper.Map<Payment>(bookingdto);
                payment.BookingId = booking.Id;
                if (bookingdto.Method == "stripe")
                {
                    var paymentIntentService = new PaymentIntentService();
                    var paymentIntent = await paymentIntentService.GetAsync(bookingdto.PaymentIntentId);

                    var chargeService = new ChargeService();
                    var charge = chargeService.Get(paymentIntent.LatestChargeId);

                    if (charge != null)
                    {
                        payment.ReceiptUrl = charge.ReceiptUrl;
                    }

                }

                email.to = user.Email;
                email.subject = "Place booking";
                email.body = $"""
                        <div style="font-family: Arial, sans-serif; color: #333333; max-width:600px; margin:auto; padding:20px; background-color: #ffffff; border:1px solid #c0c0c0; border-radius:8px;">
                          <h1 style="color: #004e98; margin-bottom: 10px;">Thank You for Trusting Us!</h1>

                          <p style="font-size:16px; line-height:1.5;">
                            Hello <strong>{user.FirstName} {user.LastName}</strong>,<br/>
                            Your booking <strong>#{booking.Id}</strong> is confirmed for <strong>{booking.PreferredDate:dddd, MMM dd, yyyy}</strong>.
                          </p>

                          <p style="font-size:16px; line-height:1.5;">
                            <strong>Total Price:</strong> {bookingdto.TotalPrice:C}
                          </p>

                          {(string.IsNullOrEmpty(payment.ReceiptUrl) ? "" : $"""
                          <p style="font-size:16px; line-height:1.5;">
                            You can view your payment receipt by clicking the button below:
                          </p>
                          <a href="{payment.ReceiptUrl}" target="_blank"
                             style="
                               display: inline-block;
                               padding: 12px 25px;
                               background-color: #004e98;
                               color: #ffffff;
                               font-weight: bold;
                               text-decoration: none;
                               border-radius: 5px;
                               transition: background-color 0.3s ease;
                             "
                             onmouseover="this.style.backgroundColor='#003770'"
                             onmouseout="this.style.backgroundColor='#004e98'"
                          >View Payment Receipt</a>
                          """)}

                          <p style="font-size:14px; color: #777777; margin-top: 30px; line-height:1.4;">
                            If you have any questions, feel free to contact us anytime.
                          </p>
                        </div>
                        """;

               await _emailService.SendEmailAsync(email);

                _unitOfWork.PaymentRepo.Insert(payment);

                var res = await _unitOfWork.SaveAsync();
                if (res > 0)
                {
                    //await trnsaction.CommitAsync();
                    //return booking.Id;
                    var chat = new Chat
                    {
                        BookingId = booking.Id,
                        StartedAt = DateTime.UtcNow
                    };
                    _unitOfWork.ChatRepo.Insert(chat);

                    await _unitOfWork.SaveAsync();

                    // Ensure chat exists and get chatId
                    int chatId = chat.Id;
                    await trnsaction.CommitAsync();

                    // Return both bookingId + chatId
                    return new BookingChatResponse
                    {
                        BookingId = booking.Id,
                        ChatId = chatId
                    };
                }
                else
                {
                    await trnsaction.RollbackAsync();
                    return null;
                }

            }
            catch (Exception ex)
            {
                await trnsaction.RollbackAsync();
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        //update statues 
        public int CancelBooking(int bookingId)
        {
            try
            {
                var booking = _unitOfWork.BookingRepo.FirstOrDefault(b => b.Id == bookingId, "JobAssignment,Payments");

                if (booking.Status == BookingStatus.Completed)
                {
                    return 0;
                }

                if(booking.PreferredDate <= DateTime.Now.AddHours(24))
                {
                    return -1;
                }

                booking.Status = BookingStatus.Cancelled;
                booking.IsActive = false;
                booking.JobAssignment.Status = JobAssignmentsStatus.Cancelled;
                booking.JobAssignment.IsActive = false;

                var stripeSecretKey = _configuration["Stripe:SecretKey"];
                var stripeClient = new StripeClient(stripeSecretKey);
                var refundService = new RefundService(stripeClient);
                var chargeService = new ChargeService(stripeClient);






                foreach (var payment in booking.Payments)
                {
                    if (payment.Method == "stripe")
                    {
                        var charges = chargeService.List(new ChargeListOptions
                        {
                            PaymentIntent = payment.PaymentIntentId,
                            Limit = 1
                        });

                        var chargeId = charges.Data.FirstOrDefault()?.Id;

                        var refundOptions = new RefundCreateOptions
                        {
                            Charge = chargeId
                        };
                        var refund = refundService.Create(refundOptions);
                        payment.Status = PaymentsStatus.Refunded;

                    }
                    else
                    {
                        payment.Status = PaymentsStatus.Failed;
                    }
                }
                _unitOfWork.Save();

                return 1;
            }
            catch (Exception ex)
            {
                return -2;
            }
        }

        //get all booking pagination
        public PaginatedResult<BookingViewDto> GetBookings(string handymanName = "", string status = "", bool? isActive = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _unitOfWork.BookingRepo.GetAll(null, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");

            if (!string.IsNullOrWhiteSpace(handymanName))
            {
                handymanName = handymanName.Trim().ToLower();

                bool isBookingIdNumber = int.TryParse(handymanName, out int bookingIdNum);

                query = query.Where(b =>
                    b.JobAssignment != null &&
                    b.JobAssignment.Handyman != null &&
                    (
                        (b.JobAssignment.Handyman.User.FirstName + " " + b.JobAssignment.Handyman.User.LastName).ToLower()
                    ).Contains(handymanName) ||(isBookingIdNumber && b.Id == bookingIdNum)
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            if (isActive.HasValue)
            {
                query = query.Where(b => b.IsActive == isActive.Value);
            }

            int totalCount = query.Count();

            var data = query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var datamapper = _mapper.Map<List<BookingViewDto>>(data);

            return new PaginatedResult<BookingViewDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount,
                Items = datamapper
            };
        }



    }
}
