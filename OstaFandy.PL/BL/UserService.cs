using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.Constants;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.General;

namespace OstaFandy.PL.BL
{
    public class UserService : IUserService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IMapper mapper,IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
        }

        public List<User> GetAlluser()
        {
            try
            {
                var data = _unitOfWork.UserRepo.GetAll().ToList();
                if (data == null)
                {
                    return null;
                }
                return data;

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error occured while retreiving all users");
                return null;

            }
        }

        public UserDto? GetUserByEmail(string Email)
        {
            try
            {
                var data = _unitOfWork.UserRepo.FirstOrDefault(u => u.Email == Email, "UserTypes,Handyman");
                var dto = _mapper.Map<UserDto>(data);
                if (data == null)
                {
                    return null;
                }
                else if (data.Handyman != null)
                {
                    dto.HandymanStatus = data.Handyman.Status;
                }

                return dto;

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error occured while retreiving user email :{Email}", Email);
                return null;

            }
        }

        public async Task VerifyEmail(int ID)
        {
            try
            {
                var user=_unitOfWork.UserRepo.GetById(ID);
                user.EmailConfirmed= true;

                _unitOfWork.UserRepo.Update(user);
                 await _unitOfWork.SaveAsync();

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<int> RegisterUser(UserRegesterDto userDto)
        {
            int res = 0;
            try
            {

                if (userDto == null)
                {
                    return 0;//invalid input
                }

                var existingUser = _unitOfWork.UserRepo.FirstOrDefault(u => u.Email == userDto.Email);
                if (existingUser != null)
                {
                    return -1;//existing user
                }

                if (userDto.Password != userDto.ConfirmPassword)
                {
                    return -2;//password mismatch
                }

                //map dto to entity
                var user = _mapper.Map<User>(userDto);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                user.IsActive = true;
                user.UserTypes.Add(_unitOfWork.UserTypeRepo.FirstOrDefault(s => s.TypeName == General.UserType.Customer));
                //add user to db
                _unitOfWork.UserRepo.Insert(user);
                res = _unitOfWork.Save();
                var client = new Client
                {
                    UserId = user.Id,
                };
                _unitOfWork.ClientRepo.Insert(client);
                _unitOfWork.Save();
                if (res > 0)
                {
                    return user.Id;
                }

            }
            catch (Exception)
            {

                return -3;//error while saving user
            }
            return res;
        }

        public async Task SendEmailConfirmationAsync(UserDto user)
        {
            var verificationLink = $"https://ostafandy.runasp.net/api/Auth/verify-email?userId={user.Id}";
            var emailContent = new EmailContentDto
            {
                to = user.Email,
                subject = "Please Verify Your Email",
                body = $"""
<div style="font-family: Arial, sans-serif; color: #333333; max-width:600px; margin:auto; padding:20px; background-color: #ffffff; border:1px solid #c0c0c0; border-radius:8px;">
  <h1 style="color: #004e98; margin-bottom: 10px;">Welcome to Our Platform!</h1>

  <p style="font-size:16px; line-height:1.5;">
    Hello <b>{user.FirstName}</b>,
  </p>

  <p style="font-size:16px; line-height:1.5;">
    Thank you for registering. Please click the link below to verify your email address and activate your account:
  </p>

  <p style="text-align:center; margin:20px 0;">
    <a href="{verificationLink}" style="background-color:#004e98; color:#fff; padding:10px 20px; text-decoration:none; border-radius:4px;">Verify Email</a>
  </p>

  <p style="font-size:14px; color: #777777; margin-top: 30px; line-height:1.4;">
    If you have any questions or didn’t register, please ignore this email.
  </p>
</div>
"""
            };

            await _emailService.SendEmailAsync(emailContent);
        }


        public UserDto? GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null; // Invalid ID
                }
                var user = _unitOfWork.UserRepo.FirstOrDefault(u => u.Id == id, "UserTypes");
                if (user == null)
                {
                    return null; // User not found
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception)
            {
                return null; // Error occurred
            }
        }

        public async Task<bool> ForgotPassword(UserDto user)
        {
            try
            {
                var res = 0;
                var otp = new Random().Next(100000, 999999).ToString();

                var passwordReset = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = otp,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.PasswordResetTokenRepo.Insert(passwordReset);
                res = _unitOfWork.Save();

                if (res > 0)
                {
                    #region email body
                    var emailContent = new EmailContentDto
                    {
                        to = user.Email,
                        subject = "Password Reset OTP",
                        body = $"""
<div style="font-family: Arial, sans-serif; color: #333333; max-width:600px; margin:auto; padding:20px; background-color: #ffffff; border:1px solid #c0c0c0; border-radius:8px;">
  <h1 style="color: #004e98; margin-bottom: 10px;">Password Reset Request</h1>

  <p style="font-size:16px; line-height:1.5;">
    Hello,<br/>
    We received a request to reset your password.
  </p>

  <p style="font-size:16px; line-height:1.5;">
    Please use the following <strong>One-Time Password (OTP)</strong> to reset your password:
  </p>

  <p style="font-size:24px; font-weight:bold; color:#004e98; text-align:center; margin:20px 0;">
    {otp}
  </p>

  <p style="font-size:16px; line-height:1.5;">
    This OTP is valid for the next <strong>15 minutes</strong>. If you didn't request a password reset, please ignore this email.
  </p>

  <p style="font-size:14px; color: #777777; margin-top: 30px; line-height:1.4;">
    If you have any questions or need further assistance, feel free to contact us anytime.
  </p>
</div>
"""
                    };
                    #endregion

                    await _emailService.SendEmailAsync(emailContent);
                    return true;
                }
                return false;
                

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = _unitOfWork.UserRepo.FirstOrDefault(u => u.Email == resetPasswordDto.Email);
                if (user == null) { return -1; } //not found 

                var token = _unitOfWork.PasswordResetTokenRepo.FirstOrDefault(t => t.UserId == user.Id && t.Token == resetPasswordDto.Otp && !t.IsUsed && t.ExpiryDate > DateTime.UtcNow);

                if (token == null)
                {
                    return 0;// invalid or expired token
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
                token.IsUsed = true;
                _unitOfWork.PasswordResetTokenRepo.Update(token);
                _unitOfWork.UserRepo.Update(user);
                return _unitOfWork.Save();
            }

            catch
            {
                return -2;//error
            }

        }

    }
}
