using AutoMapper;
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
                var bookings = _unitOfWork.BookingRepo.GetAll(null, "Client.User,JobAssignment.Handyman.User,BookingServices.Service.Category");

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


    }
}
