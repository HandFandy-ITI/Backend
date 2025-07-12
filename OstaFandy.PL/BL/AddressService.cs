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

    }
}
