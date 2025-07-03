using OstaFandy.DAL.Entities;
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

        Task<ClientProfileDTO> GetClientProfile(int clientId);
        Task<ClientOrderHistoryDTO> GetClientOrderHistory(int clientId);
        Task<List<ClientQuoteDTO>> GetClientQuotes(int clientId);
        Task<bool> UpdateClientProfile(int clientId, UpdateClientProfileDTO updateDto);
        
    }
}
