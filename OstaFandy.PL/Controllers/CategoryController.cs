using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> GetAll()
        {
            var result = _categoryService.GetAll();
            return Ok(result);
        }


        //[HttpGet("paginated")]
        //public ActionResult<PaginatedResult<CategoryDTO>> GetPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    var result = _categoryService.GetAllPaginated(pageNumber, pageSize);
        //    return Ok(result);
        //}
        [HttpGet("paginated")]
        public ActionResult<PaginatedResult<CategoryDTO>> GetPaginated(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? status = null)
        {
            var result = _categoryService.GetAllPaginated(pageNumber, pageSize, search, status);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetById(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult Add([FromBody] CategoryCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _categoryService.Add(dto);
            return Ok(new { message = "Category added successfully." });
        }



        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Update(int id, CategoryDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");
            _categoryService.Update(dto);
            return Ok("Category updated.");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int id)
        {
            bool success = _categoryService.SoftDelete(id);
            return success ? Ok("Category deleted.") : NotFound();
        }
    }
}
