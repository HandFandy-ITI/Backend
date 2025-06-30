using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;


namespace OstaFandy.PL.BL
{
    public class AutoBookingService : IAutoBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AutoBookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public async Task<int> CreateBooking(CreateBookingDTO bookingdto)
        {
            if(bookingdto == null) { return 0; }

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
                _unitOfWork.PaymentRepo.Insert(payment);

                var res=await _unitOfWork.SaveAsync();
                if (res > 0) 
                {
                    await trnsaction.CommitAsync();
                    return booking.Id;
                }
                else
                {
                    await trnsaction.RollbackAsync();
                    return -1;
                }

            }
            catch(Exception ex) 
            {
                await trnsaction.RollbackAsync();
                Console.WriteLine(ex.ToString());
                return -2;
            }
        }

        //update statues 



    }
}
