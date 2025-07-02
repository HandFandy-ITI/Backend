using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.General;
using Stripe;


namespace OstaFandy.PL.BL
{
    public class AutoBookingService : IAutoBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public AutoBookingService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        //get all
        public List<BookingViewDto> GetAllBookings()
        {
            try
            {
                var bookings = _unitOfWork.BookingRepo.GetAll(null, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");

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
                var booking = _unitOfWork.BookingRepo.FirstOrDefault(b=>b.Id==id, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category,Address");
                if (booking == null)
                {
                    throw new KeyNotFoundException($"Booking with ID {id} not found.");
                }
                var bookingDto = _mapper.Map<BookingViewDto>(booking);
                return bookingDto;
            }
            catch(Exception ex)
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
                var availableTimeSlots = await _unitOfWork.BookingRepo
                    .GetAvailableTimeSlotsAsync(reqdata.CategoryId, reqdata.Day, reqdata.UserLatitude, reqdata.UserLongitude, reqdata.EstimatedMinutes);

                return availableTimeSlots;
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

            using var trnsaction= await _unitOfWork.BeginTransactionasync();
            try
            {
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
                var payment=_mapper.Map<Payment>(bookingdto);
                payment.BookingId = booking.Id;
                if (bookingdto.Method== "stripe")
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
               
                
                _unitOfWork.PaymentRepo.Insert(payment);

                var res=await _unitOfWork.SaveAsync();
                if (res > 0) 
                {
                    //await trnsaction.CommitAsync();
                    //return booking.Id;
                   var chat= new Chat
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
            catch(Exception ex) 
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

                if (booking.Status == BookingStatus.Completed || booking.PreferredDate <= DateTime.Now.AddHours(24))
                {
                    return 0;
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
                        payment.Status= PaymentsStatus.Refunded;

                    }
                }
                _unitOfWork.Save();

                return 1; 
            }
            catch (Exception ex)
            {
                return -1; 
            }
        }



    }
}
