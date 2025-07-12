using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Repos;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    ///
    public class AdminHandyManController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlockDateService _blockDateService;
        private readonly IMapper _map;
        private readonly IHandyManService _handymanService;
        private readonly IUserService _userservice;
        private readonly IJWTService _jwtService;


        private readonly ILogger<HandyManService> _logger;



        public AdminHandyManController(IHandyManService HandyManService, IUserService userService, IMapper map, ILogger<HandyManService> logger, IJWTService jwtService, IBlockDateService blockDateService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
            _blockDateService = blockDateService;
            _map = map;
            _handymanService = HandyManService;
            _userservice = userService;

            _logger = logger;
            _jwtService = jwtService;
        }

        #region admin area 
        [HttpGet]
        [EndpointDescription("AdminHandyMan/getall")]
        [EndpointSummary("return all handymen")]
        public IActionResult GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, bool? isActive = null)
        {
            var result = _handymanService.GetAll(searchString, pageNumber, pageSize, isActive);

            if (result.Data == null || !result.Data.Any())
            {
                return Ok(new
                {
                    message = "There are no handymen assigned to system yet",
                    data = new List<AdminHandyManDTO>(),
                    currentPage = result.CurrentPage,
                    totalPages = result.TotalPages,
                    totalCount = result.TotalCount,
                    searchString = result.SearchString
                });
            }

            var mappedData = _map.Map<List<AdminHandyManDTO>>(result.Data);

            return Ok(new
            {
                data = mappedData,
                currentPage = result.CurrentPage,
                totalPages = result.TotalPages,
                totalCount = result.TotalCount,
                searchString = result.SearchString
            });
        }


        [HttpGet("pending")]
        [EndpointDescription("AdminHandyMan/getallpending")]
        [EndpointSummary("return all pending handymen")]
        public IActionResult GetAllPendingHandymen()
        {
            var result = _handymanService.GetAllPendingHandymen();
            if (result == null || !result.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "There are no pending handymen at the moment.",
                    data = new List<AdminHandyManDTO>(),
                    count = 0
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Found {result.Count} pending handymen",
                data = result,
                count = result.Count
            });
        }

        [HttpPut("status/{userId}")]
        [EndpointDescription("AdminHandyMan/updatestatusbyid")]
        [EndpointSummary("Update handyman status by id")]
        public async Task<IActionResult> UpdateHandymanStatusById(int userId, [FromBody] HandymanStatusUpdateDTO statusUpdate)
        {
            if (userId != statusUpdate.UserId)
            {
                return BadRequest(new { message = "User ID in URL must match User ID in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var validStatus = new[] { "Approved", "Rejected" };
            if (!validStatus.Contains(statusUpdate.Status))
            {
                return BadRequest(new { message = "Invalid status. Status must be either 'Approved' or 'Rejected'" });
            }

            // Fix: Use the correct instance field `_handymanService` instead of `HandyManService`
            var result = await _handymanService.UpdateHandymanStatusById(userId, statusUpdate.Status);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Failed to update handyman status. Please try again later."
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Handyman status updated to {statusUpdate.Status} successfully"
            });

        }

        [HttpPost]
        [EndpointDescription("api/AdminHandyMan/CreateHandyman")]
        [EndpointSummary("create handyman")]
        public IActionResult CreateHandyman([FromBody] CreateHandymanDTO createHandymanDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = _handymanService.CreateHandyman(createHandymanDto);
                return CreatedAtAction(nameof(GetHandymanById), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    details = "Please check if the email, phone, or national ID is already registered."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating handyman");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [EndpointDescription("api/AdminHandyMan/GetHandymanById")]
        [EndpointSummary("get handyman by id")]
        public IActionResult GetHandymanById(int id)
        {
            try
            {
                var handyman = _handymanService.GetById(id);
                if (handyman == null)
                {
                    return NotFound($"Handyman with ID {id} not found");
                }
                return Ok(handyman);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in controller while getting handyman by id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [EndpointDescription("api/AdminHandyMan/deletehandyman")]
        [EndpointSummary("SoftDelete for the handyman")]
        public IActionResult deletehandyman(int id)
        {
            try
            {
                bool deleted = _handymanService.DeleteHandyman(id);
                if (!deleted)
                {
                    return NotFound($"Handyman with ID {id} not found or could not be deleted");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting handyman with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
            {

            }
        }

        [HttpPut("{id}")]
        [EndpointDescription("api/AdminHandyMan/EditHandyman")]
        [EndpointSummary("edit handyman, check constraint")]
        public IActionResult EditHandyman(int id, [FromBody] EditHandymanDTO editHandymanDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (id != editHandymanDto.UserId)
                {
                    return BadRequest("Route ID and payload UserId do not match.");
                }

                var updatedHandyman = _handymanService.EditHandyman(editHandymanDto);
                if (updatedHandyman == null)
                {
                    _logger.LogWarning(
                        "EditHandyman: no handyman found with UserId {UserId}",
                        editHandymanDto.UserId
                    );
                    return NotFound(new { Message = $"Handyman with ID {editHandymanDto.UserId} not found." });
                }

                return Ok(updatedHandyman);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in EditHandyman controller for UserId {UserId}",
                    editHandymanDto.UserId
                );
                return StatusCode(500, "An unexpected error occurred while updating the handyman.");
            }

        }


        [Route("Handyman-register")]
        [HttpPost]
        [EndpointDescription("api/AdminHandyMan/register-Handyma")]
        [EndpointSummary("add handyman application")]
        public IActionResult HandyManApplication([FromForm] HandyManApplicationDto handymandto)
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
            var res = _handymanService.CreateHandyManApplicationAsync(handymandto).Result;
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
                var user = _userservice.GetById(res);


                var token = _jwtService.GeneratedToken(user);
                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Message = "registered successfully please wait for aprrovment",
                    Data = token,
                    StatusCode = StatusCodes.Status201Created
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while registeration",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });


            }
        }

        #endregion

        #region admin block date area
        #region get all block date
        [HttpGet("blockdates")]
        [EndpointDescription("AdminHandyMan/GetAllBlockDates")]
        [EndpointSummary("Get all block dates for handymen")]
        public IActionResult GetAllBlockDates(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, DateTime? date = null)
        {
            try
            {
                ;
                var result = _blockDateService.GetAll(searchString, pageNumber, pageSize, status, date);
                if (result.Data == null || !result.Data.Any())
                {
                    return Ok(new
                    {
                        message = "There are no block dates found",
                        data = new List<BlockDateDTO>(),
                        currentPage = result.CurrentPage,
                        totalPages = result.TotalPages,
                        totalCount = result.TotalCount,
                        searchString = result.SearchString
                    });
                }
                return Ok(new
                {
                    data = result.Data,
                    currentPage = result.CurrentPage,
                    totalPages = result.TotalPages,
                    totalCount = result.TotalCount,
                    searchString = result.SearchString
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting block dates.", ex.InnerException?.Message ?? ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        #endregion


        #region category for dropdown
        [HttpGet("GetCategoriesForDropdown")]
        [EndpointDescription("AdminHandyMan/GetCategoriesForDropdown")]
        [EndpointSummary("get all categories to filter with")]
        public IActionResult GetCategoriesForDropdown()
        {
            var categories = _blockDateService.GetAllCategory();
            return Ok(categories.Select(c => new { Id = c.Id, Name = c.Name }));
        }
        #endregion

        #region get all handyman with date

        [HttpGet("GetAllHandymanData")]
        [EndpointDescription("AdminHandyMan/GetAllHandymanData")]
        [EndpointSummary("show all handymen")]
        public IActionResult GetHandymenForDropdown(string searchString = "", int pageNumber = 1, int pageSize = 5, int? categoryId = null)
        {
            var result = _blockDateService.GetAllHandymanData(searchString, pageNumber, pageSize, categoryId);
            return Ok(new
            {
                data = result.Data,
                currentPage = result.CurrentPage,
                totalPages = result.TotalPages,
                totalCount = result.TotalCount,
                searchString = result.SearchString
            });
        }
        #endregion

        #region create block date
        [HttpPost("AddBlockDate")]
        [EndpointDescription("AdminHandyMan/AddBlockDate")]
        [EndpointSummary("create dateblock for handyman")]
        public async Task<IActionResult> AddBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            var result = await _blockDateService.AddBlockDate(HandymanId, Reason, StartDate, EndDate);
            return result ? Ok("block date created") : BadRequest("failed to create blockdate handyman has a job at this date or user deleted from system");
        }

        #endregion

        #region reject handyman ask for vacation
        [HttpPut("RejectBlockDate")]
        [EndpointDescription("AdminHandyMan/RejectBlockDate")]
        [EndpointSummary("Reject handyman request to get vacation")]
        public IActionResult RejectBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            var result = _blockDateService.RejectBlockDate(HandymanId, Reason, StartDate, EndDate);
            if (result)
            {
                return Ok(new { message = "The vacation has been rejected successfully." });
            }
            else
            {
                return BadRequest(new { message = "Error occurred while rejecting vacation." });
            }
        }
        #endregion
        #region Approve handyman ask for vacation
        [HttpPut("ApproveBlockDate")]
        [EndpointDescription("AdminHandyMan/ApproveBlockDate")]
        [EndpointSummary("Reject handyman request to get vacation")]
        public IActionResult ApproveBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate)
        {
            var result = _blockDateService.ApproveBlockDate(HandymanId, Reason, StartDate, EndDate);
            if (result)
            {
                return Ok(new { message = "The vacation has been approved successfully." });
            }
            else
            {
                return BadRequest(new { message = "Error occurred while approving vacation." });
            }
        }
        #endregion

        #region get all admin notification
        [HttpGet("GetNotificationsOfAdmin/{AdminUserId}")]
        [EndpointDescription("AdminHandyMan/GetNotificationsOfAdmin")]
        [EndpointSummary("get all notification for the admin")]
        public IActionResult GetNotifications(int AdminUserId)
        {
            //try
            //{
            //    var notifications = _unitOfWork.NotificationRepo
            //        .GetAll(n => n.UserId == AdminUserId);

            //    var orderedNotifications = notifications
            //        .OrderByDescending(n => n.CreatedAt)
            //        .Take(50)
            //        .ToList();

            //    return Ok(orderedNotifications);
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, new { message = "Error retrieving notifications", error = ex.Message });
            //}
            try
            {
                var notifications = _unitOfWork.NotificationRepo
                    .GetAll(n => n.UserId == AdminUserId);

                var orderedNotifications = notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50)
                    .Select(n => new NotificationDTO
                    {
                        Id = n.Id,
                        UserId = n.UserId,
                        Title = n.Title,
                        Message = n.Message,
                        Type = n.Type,
                        IsRead = n.IsRead,
                        IsActive = n.IsActive,
                        CreatedAt = n.CreatedAt,
                        ActionStatus = GetActionStatusForNotification(n)
                    })
                    .ToList();

                return Ok(orderedNotifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving notifications", error = ex.Message });
            }
        }
        private string? GetActionStatusForNotification(Notification notification)
        {
            if (notification.Type.Contains(",") && notification.Type.Split(',').Length == 4)
            {
                var parts = notification.Type.Split(',');
                if (parts.Length >= 4)
                {
                    var handymanId = int.Parse(parts[0]);
                    var reason = parts[1];

                    var blockDate = _unitOfWork.BlockDateRepo
                        .GetAll(a => a.UserId == handymanId &&
                                   a.Reason.ToLower().Trim() == reason.ToLower().Trim())
                        .OrderByDescending(a => a.Id)
                        .FirstOrDefault();

                    if (blockDate != null)
                    {
                        return blockDate.Status?.ToLower() switch
                        {
                            "approved" => "approved",
                            "denied" => "rejected",
                            _ => null
                        };
                    }
                }
            }

            return null;
        }
        #endregion
        #endregion
    }
}

