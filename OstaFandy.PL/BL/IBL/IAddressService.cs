using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IAddressService
    {
        List<AddressDTO> GetAddressByUserId(int userId);

        int CreateAddress(CreateAddressDTO addressDTO);
        int DeleteAddress(int addressId, int userId);
        int SetDefaultAddress(int addressId, int userId);
    }
}
