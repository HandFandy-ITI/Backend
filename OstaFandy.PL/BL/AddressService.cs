using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.Constants;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class AddressService:IAddressService
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddressService(IUnitOfWork unitOfWork,IMapper mapper)
        {
           _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //get address by userId
        public List<AddressDTO> GetAddressByUserId(int userId) {

            try
            {
                if (userId <= 0)
                {
                    throw new ArgumentException("Invalid Id number");
                }
                var address = _unitOfWork.AddressRepo.GetAll(a=>a.UserId == userId && a.IsActive);

                if (address == null) { 
                
                    return new List<AddressDTO>();
                }

                var addressDto= _mapper.Map<List<AddressDTO>>(address);
                return addressDto;

            }
            catch (Exception ex) 
            {
                throw new Exception("error happen when retrive addresses");
                
            }
        }

        //create address
        public int CreateAddress(CreateAddressDTO addressDTO)
        {
            try
            {
                if (addressDTO == null)
                    return 0;

                var address = _mapper.Map<Address>(addressDTO);

                var addressExist = _unitOfWork.AddressRepo.GetAll(a => a.UserId == address.UserId);

                //first address in sys
                if (!addressExist.Any())
                {
                    address.IsDefault = true;
                }
                else if (addressDTO.IsDefault == true)
                {
                    // make all past address false
                    foreach (var old in addressExist)
                    {
                        old.IsDefault = false;
                        _unitOfWork.AddressRepo.Update(old);
                    }
                    address.IsDefault = true;
                }
                else
                {
                    address.IsDefault = false;
                }

                _unitOfWork.AddressRepo.Insert(address);
                int res = _unitOfWork.Save();

                return res > 0 ? res : -1;
            }
            catch
            {
                return -1;
            }
        }

        public int DeleteAddress(int addressId, int userId)
        {
            try
            {
                if (addressId <= 0 || userId <= 0)
                {
                    return 0; // Invalid parameters
                }

                // Get the address to delete
                var address = _unitOfWork.AddressRepo.GetById(addressId);

                if (address == null || address.UserId != userId)
                {
                    return 0; // Address not found or doesn't belong to user
                }

                // Check if address has any bookings
                var hasBookings = _unitOfWork.BookingRepo.GetAll(b => b.AddressId == addressId && b.IsActive).Any();

                if (hasBookings)
                {
                    return -2; // Address has bookings, cannot delete
                }

                // If it's the default address, we need to handle this carefully
                if (address.IsDefault)
                {
                    // Get other active addresses for this user
                    var otherAddresses = _unitOfWork.AddressRepo.GetAll(a =>
                        a.UserId == userId &&
                        a.Id != addressId &&
                        a.IsActive).ToList();

                    // If there are other addresses, make the first one default
                    if (otherAddresses.Any())
                    {
                        var newDefaultAddress = otherAddresses.First();
                        newDefaultAddress.IsDefault = true;
                        _unitOfWork.AddressRepo.Update(newDefaultAddress);
                    }
                }

                // Soft delete by setting IsActive to false
                address.IsActive = false;
                _unitOfWork.AddressRepo.Update(address);

                int result = _unitOfWork.Save();
                return result > 0 ? 1 : -1;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging
                return -1; // Error occurred
            }
        }

        public int SetDefaultAddress(int addressId, int userId)
        {
            try
            {
                if (addressId <= 0 || userId <= 0)
                {
                    return 0; // Invalid parameters
                }

                // Get the address to set as default
                var addressToSetDefault = _unitOfWork.AddressRepo.GetById(addressId);

                if (addressToSetDefault == null || addressToSetDefault.UserId != userId || !addressToSetDefault.IsActive)
                {
                    return 0; // Address not found, doesn't belong to user, or is not active
                }

                // If it's already the default address, no need to update
                if (addressToSetDefault.IsDefault)
                {
                    return 2; // Already default
                }

                // Get all user's addresses
                var userAddresses = _unitOfWork.AddressRepo.GetAll(a => a.UserId == userId && a.IsActive).ToList();

                // Set all addresses to non-default
                foreach (var address in userAddresses)
                {
                    address.IsDefault = false;
                    _unitOfWork.AddressRepo.Update(address);
                }

                // Set the specified address as default
                addressToSetDefault.IsDefault = true;
                _unitOfWork.AddressRepo.Update(addressToSetDefault);

                int result = _unitOfWork.Save();
                return result > 0 ? 1 : -1;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging
                return -1; // Error occurred
            }
        }

    }
}
