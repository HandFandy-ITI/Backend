using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IUserService
    {
        public UserDto? GetUserByEmail(string Email);

        public int RegisterUser(UserRegesterDto userDto);

        public UserDto? GetById(int id);

        public List<User> GetAlluser();
    }
}
