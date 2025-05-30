using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IUserService
    {
        public UserDto? GetUserByEmail(string Email);

    }
}
