using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //dd


            #region service catalog
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Service, ServiceDTO>().ReverseMap();
            #endregion


            #region User
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserType, UserTypeDto>().ReverseMap();
            CreateMap<User, UserRegesterDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            #endregion

            #region Payment

            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src =>
                    $"{src.Booking.Client.User.FirstName} {src.Booking.Client.User.LastName}"))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<Payment, PaymentDetailsDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src =>
                    $"{src.Booking.Client.User.FirstName} {src.Booking.Client.User.LastName}"))
                .ForMember(dest => dest.Receipt, opt => opt.MapFrom(src => src.ReceiptUrl))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt));

            #endregion

            {
              

                CreateMap<User, UserDto>().ReverseMap();
                CreateMap<UserType, UserTypeDto>().ReverseMap();
                CreateMap<Address, AddressDTO>().ReverseMap();
                CreateMap<JobAssignment, JobAssignmentDTO>().ReverseMap();

                CreateMap<BlockDate, AdminBlockDateDTO>()
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                    .ReverseMap();

                CreateMap<Handyman, AdminHandyManDTO>()
                    .AfterMap((src, dest) =>
                    {
                        if (src.User != null)
                        {
                            dest.FirstName = src.User.FirstName;
                            dest.LastName = src.User.LastName;
                            dest.Email = src.User.Email;
                            dest.Phone = src.User.Phone;
                            dest.IsActive = src.User.IsActive;
                            dest.CreatedAt = src.User.CreatedAt;
                        }

                        dest.UpdatedAt = src.UpdatedAt;
                        dest.Latitude = src.Latitude;
                        dest.Longitude = src.Longitude;
                        dest.NationalId = src.NationalId;
                        dest.NationalIdImg = src.NationalIdImg;
                        dest.Img = src.Img;
                        dest.ExperienceYears = src.ExperienceYears;
                        dest.Status = src.Status;

                        dest.SpecializationCategory = src.Specialization?.Name;
                        dest.DefaultAddressPlace = src.DefaultAddress?.City;


                        dest.AdminBlockDateDTO = src.BlockDates?.Select(bd => new AdminBlockDateDTO
                        {
                            Id = bd.Id,
                            UserId = bd.UserId,
                            Reason = bd.Reason,
                            StartDate = bd.StartDate,
                            EndDate = bd.EndDate,
                            IsActive = bd.IsActive,
                            CreatedAt = bd.CreatedAt,
                            UpdatedAt = bd.UpdatedAt
                        }).ToList();
                    });
            }
           
            CreateMap<EditHandymanDTO, Handyman>()
               .AfterMap((src, dest) =>
               {
                   if (dest.User != null)
                   {
                       dest.User.FirstName = src.FirstName;
                       dest.User.LastName = src.LastName;
                       dest.User.Email = src.Email;
                       dest.User.Phone = src.Phone;
                       dest.User.UpdatedAt = DateTime.UtcNow;
                   }
               })
               .ReverseMap();

            CreateMap<User, AdminDisplayClientDTO>()
                    .AfterMap((src, dest) =>
                    {
                        if (src.Client != null)
                        {
                            dest.ClientUserId = src.Client.UserId;
                            dest.DefaultAddressId = src.Client.DefaultAddressId;

                            if (src.Client.DefaultAddress != null)
                            {
                                dest.DefaultAddress = new AddressDTO
                                {
                                    Id = src.Client.DefaultAddress.Id,
                                    Address1 = src.Client.DefaultAddress.Address1,
                                    City = src.Client.DefaultAddress.City,
                                    Latitude = src.Client.DefaultAddress.Latitude,
                                    Longitude = src.Client.DefaultAddress.Longitude,
                                    AddressType = src.Client.DefaultAddress.AddressType,
                                    IsDefault = src.Client.DefaultAddress.IsDefault,
                                    IsActive = src.Client.DefaultAddress.IsActive,
                                    CreatedAt = src.Client.DefaultAddress.CreatedAt
                                };
                            }

                            dest.Addresses = src.Addresses?
                                .Where(a => a.IsActive)
                                .Select(address => new AddressDTO
                                {
                                    Id = address.Id,
                                    Address1 = address.Address1,
                                    City = address.City,
                                    Latitude = address.Latitude,
                                    Longitude = address.Longitude,
                                    AddressType = address.AddressType,
                                    IsDefault = address.IsDefault,
                                    IsActive = address.IsActive,
                                    CreatedAt = address.CreatedAt
                                }).ToList() ?? new List<AddressDTO>();

                            var bookings = src.Client.Bookings ?? new List<Booking>();
                            dest.TotalBookings = bookings.Count;
                            dest.ActiveBookings = bookings.Count(b => b.IsActive &&
                                (b.Status == "Pending" || b.Status == "Confirmed" || b.Status == "InProgress"));
                            dest.TotalSpent = bookings
                                .Where(b => b.TotalPrice.HasValue)
                                .Sum(b => b.TotalPrice ?? 0m);
                        }
                    });


            CreateMap<AdminEditClientDTO, User>()
                .AfterMap((src, dest) =>
                {
                    if (dest.Client != null)
                    {
                        dest.Client.DefaultAddressId = src.DefaultAddressId;
                        dest.UpdatedAt = DateTime.UtcNow;
                        //dest.Client.DefaultAddress = new Address
                        //{
                        //    Id = src.DefaultAddress.Id,
                        //    Address1 = src.DefaultAddress.Address1,
                        //    City = src.DefaultAddress.City,
                        //    Latitude = src.DefaultAddress.Latitude,
                        //    Longitude = src.DefaultAddress.Longitude,
                        //    AddressType = src.DefaultAddress.AddressType,
                        //    IsDefault = src.DefaultAddress.IsDefault,
                        //    IsActive = src.DefaultAddress.IsActive,
                        //    CreatedAt = DateTime.UtcNow
                        //};
                    }
                })
                .ReverseMap();
        }
    }
}
