using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos

{
    public interface IPaymentRepo
    {
        Task<IEnumerable<Payment>> GetAllAsync(
            string? status = null,
            string? method = null,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<int> GetTotalCountAsync(
            string? status = null,
            string? method = null,
            string? searchTerm = null);

        Task<Payment?> GetByIdAsync(int id);
    }
}