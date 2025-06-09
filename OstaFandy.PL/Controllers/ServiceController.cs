using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ServiceDTO>> GetAll()
        {
            var result = _serviceService.GetAll();
            return Ok(result);
        }

        [HttpGet("by-category/{categoryId}")]
        public ActionResult<IEnumerable<ServiceDTO>> GetByCategoryId(int categoryId)
        {
            var result = _serviceService.GetByCategoryId(categoryId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<ServiceDTO> GetById(int id)
        {
            var service = _serviceService.GetById(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult Add(ServiceDTO dto)
        {
            _serviceService.Add(dto);
            return Ok("Service added.");
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Update(int id, ServiceDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");
            _serviceService.Update(dto);
            return Ok("Service updated.");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int id)
        {
            bool success = _serviceService.SoftDelete(id);
            return success ? Ok("Service deleted.") : NotFound();
        }
    }
}
