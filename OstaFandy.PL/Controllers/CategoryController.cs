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

        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetById(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult Add(CategoryDTO dto)
        {
            _categoryService.Add(dto);
            return Ok("Category added successfully.");
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
