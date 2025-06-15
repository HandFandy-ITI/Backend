using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;

        public ServiceService(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        public IEnumerable<ServiceDTO> GetAll()
        {
            var services = _unit.ServiceRepo.GetAll(s => s.IsActive, includeProperties: "Category").ToList();
            return _mapper.Map<IEnumerable<ServiceDTO>>(services);
        }

        public PaginatedResult<ServiceDTO> GetAllPaginated(
      int pageNumber,
      int pageSize,
      string? search = null,
      string? status = null,
      string? sortField = null,
      string? sortOrder = null,
      int? categoryId = null)
        {
            var query = _unit.ServiceRepo.GetAll(includeProperties: "Category").AsQueryable();

            // Filters
            if (categoryId.HasValue)
                query = query.Where(s => s.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(search) ||
                    s.Description.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(s => (status == "Active") ? s.IsActive : !s.IsActive);
            }

            // Sorting
            query = ApplySorting(query, sortField, sortOrder);

            // Pagination
            var totalItems = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedResult<ServiceDTO>
            {
                TotalItems = totalItems,
                Items = _mapper.Map<List<ServiceDTO>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        private IQueryable<Service> ApplySorting(IQueryable<Service> query, string? sortField, string? sortOrder)
        {
            var ascending = string.IsNullOrEmpty(sortOrder) || sortOrder.ToLower() == "asc";
            sortField = sortField?.ToLower() ?? "name";

            return sortField switch
            {
                "name" => ascending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                "fixedprice" => ascending ? query.OrderBy(s => s.FixedPrice) : query.OrderByDescending(s => s.FixedPrice),
                "estimatedminutes" => ascending ? query.OrderBy(s => s.EstimatedMinutes) : query.OrderByDescending(s => s.EstimatedMinutes),
                _ => query.OrderBy(s => s.Name)
            };
        }

        public IEnumerable<ServiceDTO> GetByCategoryId(int categoryId)
        {
            var services = _unit.ServiceRepo.GetAll(s => s.IsActive && s.CategoryId == categoryId).ToList();
            return _mapper.Map<IEnumerable<ServiceDTO>>(services);
        }

        public ServiceDTO? GetById(int id)
        {
            var service = _unit.ServiceRepo.GetById(id);
            return service == null ? null : _mapper.Map<ServiceDTO>(service);
        }

        public void Add(ServiceCreateDTO dto)
        {
            var service = _mapper.Map<Service>(dto);
            service.CreatedAt = DateTime.UtcNow;
            service.IsActive = true;

            _unit.ServiceRepo.Insert(service);
            _unit.Save();
        }

        public void Update(ServiceUpdateDTO dto)
        {
            var existingService = _unit.ServiceRepo.GetById(dto.Id);
            if (existingService == null)
                throw new Exception("Service not found");

            _mapper.Map(dto, existingService);
            existingService.UpdatedAt = DateTime.UtcNow;

            _unit.ServiceRepo.Update(existingService);
            _unit.Save();
        }

        public bool SoftDelete(int id)
        {
            var service = _unit.ServiceRepo.GetById(id);
            if (service == null) return false;

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;

            _unit.ServiceRepo.Update(service);
            _unit.Save();
            return true;
        }

        public bool ToggleStatus(int id)
        {
            var service = _unit.ServiceRepo.GetById(id);
            if (service == null) return false;

            service.IsActive = !service.IsActive;
            service.UpdatedAt = DateTime.UtcNow;

            _unit.ServiceRepo.Update(service);
            _unit.Save();
            return true;
        }
    }
}
