using OstaFandy.PL.DTOs;

public interface ICategoryService
{
    IEnumerable<CategoryDTO> GetAll();
    PaginatedResult<CategoryDTO> GetAllPaginated(int pageNumber, int pageSize, string? search = null, string? status = null);
    CategoryDTO? GetById(int id);
    Task AddAsync(CategoryCreateDTO dto); // changed
    Task<bool> UpdateAsync(int id, CategoryUpdateDTO dto); // changed
    bool SoftDelete(int id);
    bool ToggleStatus(int id);
}
