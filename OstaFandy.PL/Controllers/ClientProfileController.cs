using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientProfileController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientProfileController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("GetClientProfile/{clientId}")]

        public async Task<IActionResult> GetClientProfile(int clientId)
        {
            try
            {
                var clientProfile = await _clientService.GetClientProfile(clientId);

                return Ok(new ResponseDto<ClientProfileDTO>
                {
                    IsSuccess = true,
                    Message = "Client profile retrieved successfully.",
                    Data = clientProfile,
                    StatusCode = StatusCodes.Status200OK
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving client profile.",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet("GetClientOrderHistory/{clientId}")]
        public async Task<IActionResult> GetClientOrderHistory(int clientId)
        {
            try
            {
                var orderHistory = await _clientService.GetClientOrderHistory(clientId);

                return Ok(new ResponseDto<ClientOrderHistoryDTO>
                {
                    IsSuccess = true,
                    Message = "Client order history retrieved successfully.",
                    Data = orderHistory,
                    StatusCode = StatusCodes.Status200OK
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving order history.",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet("GetClientQuotes/{clientId}")]
        public async Task<IActionResult> GetClientQuotes(int clientId)
        {
            try
            {
                var quotes = await _clientService.GetClientQuotes(clientId);

                return Ok(new ResponseDto<List<ClientQuoteDTO>>
                {
                    IsSuccess = true,
                    Message = "Client quotes retrieved successfully.",
                    Data = quotes,
                    StatusCode = StatusCodes.Status200OK
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving quotes.",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPut("UpdateClientProfile/{clientId}")]
        public async Task<IActionResult> UpdateClientProfile(int clientId, [FromBody] UpdateClientProfileDTO updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = "Update data is required.",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ResponseDto<string>
                    {
                        IsSuccess = false,
                        Message = $"Invalid data provided: {string.Join(", ", errors)}",
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _clientService.UpdateClientProfile(clientId, updateDto);

                if (result)
                {
                    return Ok(new ResponseDto<string>
                    {
                        IsSuccess = true,
                        Message = "Client profile updated successfully.",
                        Data = null,
                        StatusCode = StatusCodes.Status200OK
                    });
                }

                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Failed to update client profile.",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {


                var fullException = ex;
                var messages = new List<string>();

                while (fullException != null)
                {
                    messages.Add($"{fullException.GetType().Name}: {fullException.Message}");
                    if (!string.IsNullOrEmpty(fullException.StackTrace))
                    {
                        messages.Add($"Stack: {fullException.StackTrace.Split('\n').FirstOrDefault()}");
                    }
                    fullException = fullException.InnerException;
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = $"DEBUG - Full Exception Details: {string.Join(" | ", messages)}",
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

    }
}
