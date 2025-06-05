using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class UserService:IUserService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork,ILogger<UserService> logger,IMapper mapper)
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
    }
}
