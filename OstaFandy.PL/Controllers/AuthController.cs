using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IJWTService _jwtService;
        readonly IUserService _userService;
        public AuthController(IJWTService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("register-Customer")]
        public IActionResult RegisterCustomer([FromBody] UserRegesterDto registerCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid input",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            var res = _userService.RegisterUser(registerCustomerDto);
            if (res == 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid input",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (res == -1)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "User already exists",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (res == -2)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Password and confirm password do not match",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (res > 0)
            {
                var user = _userService.GetById(res);

                var token = _jwtService.GeneratedToken(user);
                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Message = "User registered successfully",
                    Data = token,
                    StatusCode = StatusCodes.Status201Created
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while registering the user",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });


            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid input",
                    Data = string.Join(", ", errors),
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            var user = _userService.GetUserByEmail(userLoginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid email or password",
                    Data = null,
                    StatusCode = StatusCodes.Status401Unauthorized
                });
            }
            var token = _jwtService.GeneratedToken(user);
            return Ok(new ResponseDto<string>
            {
                IsSuccess = true,
                Message = "Login Successful",
                Data = token,
                StatusCode = StatusCodes.Status200OK
            });


        }
    }
}
