using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(int userId)
        {
            await _userService.VerifyEmail(userId);
            return Redirect("http://localhost:4200/login?verified=true");
        }

        [HttpPost("register-Customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] UserRegesterDto registerCustomerDto)
        {
          
            var res = await _userService.RegisterUser(registerCustomerDto);
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
                await _userService.SendEmailConfirmationAsync(user);

                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Message = "registered successfully",
                    Data = user.Email,
                    StatusCode = StatusCodes.Status201Created
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Oops! Something went wrong while creating your account. Please try again.",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });


            }
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] string userEmail)
        {
            var user = _userService.GetUserByEmail(userEmail);
            if (user == null || user.EmailConfirmed)
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "User not found or already confirmed",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });

            await _userService.SendEmailConfirmationAsync(user);

            return Ok(new ResponseDto<string>
            {
                IsSuccess=true,
                Message= "Verification email resent.",
                Data=null,
                StatusCode= StatusCodes.Status200OK
            });
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Invalid input",
                        Data = string.Join(", ", errors),
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                var user = _userService.GetUserByEmail(userLoginDto.Email);

                // Check if user is null or hash is missing
                if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    return Unauthorized(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password",
                        Data = null,
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }
                else if (!user.IsActive)
                {
                    return Unauthorized(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Your account is inactive. Please contact support.",
                        Data = null,
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }
                else if (user.HandymanStatus == "Pending")
                {
                    return Unauthorized(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Your handyman application is still under review.",
                        Data = "Pending",
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }
                else if (user.HandymanStatus == "Rejected")
                {
                    return Unauthorized(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Your handyman application has been rejected.",
                        Data = "Rejected",
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }
                else if (user.EmailConfirmed == false)
                {
                    return Unauthorized(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Email is not verified. Please check your email and verify your account before logging in.",
                        Data = null,
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }

                // Try verifying password
                bool passwordMatches = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash);

                if (!passwordMatches)
                {
                    return Unauthorized(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password",
                        Data = null,
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }

                // Generate token and return
                var token = _jwtService.GeneratedToken(user);

                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Message = "Login Successful",
                    Data = token,
                    StatusCode = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                // Catch anything unexpected and return 500 with message
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred.",
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string emailAddress)
        {
            var user = _userService.GetUserByEmail(emailAddress);
            if (user == null)
            {
                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            var res = await _userService.ForgotPassword(user);
            if(!res)
            {
                return StatusCode(500, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Failed to send OTP, please try again later.",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }

            return Ok(new ResponseDto<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "OTP sent successfully to your email",
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var res = _userService.ResetPassword(dto);
            if (res == 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Invalid OTP",
                    StatusCode = StatusCodes.Status400BadRequest

                });
            }
            else if(res == -1)
            {

                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Wronge Email",
                    StatusCode = StatusCodes.Status404NotFound

                });
            }
            else if(res==-2)
            {
                return StatusCode(500, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Failed to reset password, please try again later.",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
            else
            {
                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Data = null,
                    Message = "Password Reset",
                    StatusCode = StatusCodes.Status200OK
                });
            }
        }
    }
}
