using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }


        //get all addreesses for user 
        [HttpGet("GetAddressesByUserId/{UserId}")]
        public IActionResult GetAddressesByUserId(int UserId)
        {
            var Addresses = _addressService.GetAddressByUserId(UserId);
            return Ok(Addresses);
        }

        [HttpPost("CreateAddress")]
        public IActionResult CreateAddress([FromBody]CreateAddressDTO address) {

            int res = _addressService.CreateAddress(address);

            if (res == 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid Address",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else if (res == -1)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Error Adding Address",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            else
            {
                return Ok(new ResponseDto<string>
                {
                    IsSuccess = true,
                    Message = "Address Added Successfully",
                    Data = null,
                    StatusCode = StatusCodes.Status200OK
                });
            }
        }

        
    }
}
