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

        // GET: api/category
        [HttpGet]
        public ActionResult<IEnumerable<CategoryDTO>> GetAll()
        {
            var result = _categoryService.GetAll();
            return Ok(result);
        }

        // GET: api/category/paginated
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

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetById(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromForm] CategoryCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.AddAsync(dto);
            return Ok(new { message = "Category added successfully." });
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryUpdateDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var success = await _categoryService.UpdateAsync(id, dto);
            if (!success)
                return NotFound(new { message = "Category not found." });

            return Ok(new { message = "Category updated." });
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int id)
        {
            bool success = _categoryService.SoftDelete(id);
            return success ? Ok("Category deleted.") : NotFound();
        }

        // PATCH: api/category/{id}/toggle-status
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Policy = "Admin")]
        public IActionResult ToggleStatus(int id)
        {
            bool success = _categoryService.ToggleStatus(id);
            return success
                ? Ok("Category status updated.")
                : BadRequest("Cannot deactivate category with active services.");
        }
    }
}
