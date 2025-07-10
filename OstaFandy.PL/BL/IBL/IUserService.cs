using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IUserService
    {
        public UserDto? GetUserByEmail(string Email);

       Task VerifyEmail(int ID);

        Task SendEmailConfirmationAsync(UserDto user);

        Task<int> RegisterUser(UserRegesterDto userDto);

        public UserDto? GetById(int id);

        public List<User> GetAlluser();

        public Task<bool> ForgotPassword(UserDto user);

        public int ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
