using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminHandyManController : ControllerBase
    {
        private readonly IMapper _map;
        private readonly IHandyManService _handymanService;
        private readonly IUserService _userservice;

        public AdminHandyManController(IHandyManService HandyManService, IUserService userService, IMapper map)
        {
            _map = map;
            _handymanService = HandyManService;
            _userservice = userService;
        }
        [HttpGet]
        [EndpointDescription("AdminHandyMan/getall")]
        [EndpointSummary("return all handymen")]
        public IActionResult GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5)
        {
            var result = _handymanService.GetAll(searchString, pageNumber, pageSize);

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

    }
}

