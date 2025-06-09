using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
  
        public interface IServiceService
        {
            IEnumerable<ServiceDTO> GetAll();
            IEnumerable<ServiceDTO> GetByCategoryId(int categoryId);
            ServiceDTO? GetById(int id);
            void Add(ServiceDTO dto);
            void Update(ServiceDTO dto);
            bool SoftDelete(int id);
        }

    
}
