using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IServiceService
    {
        IEnumerable<ServiceDTO> GetAll();
        IEnumerable<ServiceDTO> GetByCategoryId(int categoryId);
        ServiceDTO? GetById(int id);

        PaginatedResult<ServiceDTO> GetAllPaginated(
            int pageNumber, int pageSize,
            string? search = null, string? status = null,
            string? sortField = null, string? sortOrder = null,
            int? categoryId = null);

        void Add(ServiceCreateDTO dto);
        void Update(ServiceUpdateDTO dto);
        bool SoftDelete(int id);
        bool ToggleStatus(int id);
    }
}
