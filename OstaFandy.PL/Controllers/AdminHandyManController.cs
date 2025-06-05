using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Repos.IRepos;
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
    }
}

