using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
     public interface IJWTService
    {
        public string GeneratedToken(UserDto user);
    }
}
