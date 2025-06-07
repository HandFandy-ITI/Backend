using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IClientService
    {
        PaginationHelper<AdminDisplayClientDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, bool? isActive = null);
        AdminDisplayClientDTO GetById(int id);

        AdminEditClientDTO EditClientDTO(AdminEditClientDTO editClientDto);

        bool DeleteClient(int id);
    }
}
