using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{

        public interface ICategoryService
        {
            IEnumerable<CategoryDTO> GetAll();

        PaginatedResult<CategoryDTO> GetAllPaginated(int pageNumber, int pageSize, string? search = null, string? status = null);

        CategoryDTO? GetById(int id);
           void Add(CategoryCreateDTO dto);

            void Update(CategoryDTO dto);
            bool SoftDelete(int id);


        }
}
