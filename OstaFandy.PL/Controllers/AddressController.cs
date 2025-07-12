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

        [HttpDelete("DeleteAddress/{addressId}")]
        public IActionResult DeleteAddress(int addressId, [FromQuery] int userId)
        {
            if (addressId <= 0 || userId <= 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid address ID or user ID",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            int result = _addressService.DeleteAddress(addressId, userId);

            switch (result)
            {
                case 0:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Address not found or you don't have permission to delete it",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });

                case -1:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Error occurred while deleting address",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });

                case -2:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Cannot delete address because it has associated bookings",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });

                case 1:
                    return Ok(new ResponseDto<string>
                    {
                        IsSuccess = true,
                        Message = "Address deleted successfully",
                        Data = null,
                        StatusCode = StatusCodes.Status200OK
                    });

                default:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Unexpected error occurred",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
            }
        }

        [HttpPut("SetDefaultAddress/{addressId}")]
        public IActionResult SetDefaultAddress(int addressId, [FromQuery] int userId)
        {
            if (addressId <= 0 || userId <= 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid address ID or user ID",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            int result = _addressService.SetDefaultAddress(addressId, userId);

            switch (result)
            {
                case 0:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Address not found, inactive, or you don't have permission to modify it",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });

                case -1:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Error occurred while setting default address",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });

                case 1:
                    return Ok(new ResponseDto<string>
                    {
                        IsSuccess = true,
                        Message = "Address set as default successfully",
                        Data = null,
                        StatusCode = StatusCodes.Status200OK
                    });

                case 2:
                    return Ok(new ResponseDto<string>
                    {
                        IsSuccess = true,
                        Message = "Address is already set as default",
                        Data = null,
                        StatusCode = StatusCodes.Status200OK
                    });

                default:
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Unexpected error occurred",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
            }
        }



    }
}
