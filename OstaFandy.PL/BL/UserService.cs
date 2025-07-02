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

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
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
                var data = _unitOfWork.UserRepo.FirstOrDefault(u => u.Email == Email, "UserTypes");
                if (data == null)
                {
                    return null;
                }
                return _mapper.Map<UserDto>(data);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "error occured while retreiving user email :{Email}", Email);
                return null;

            }
        }

        public int RegisterUser(UserRegesterDto userDto)
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

        public UserDto? GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null; // Invalid ID
                }
                var user = _unitOfWork.UserRepo.FirstOrDefault(u=>u.Id==id, "UserTypes");
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
    }
}
