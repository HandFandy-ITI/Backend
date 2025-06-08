using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{

        public interface ICategoryService
        {
            IEnumerable<CategoryDTO> GetAll();
            CategoryDTO? GetById(int id);
            void Add(CategoryDTO dto);
            void Update(CategoryDTO dto);
            bool SoftDelete(int id);


        }
}
