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

        public void Add(ServiceDTO dto)
        {
            var service = _mapper.Map<Service>(dto);
            service.CreatedAt = DateTime.UtcNow;
            service.IsActive = true;

            _unit.ServiceRepo.Insert(service);
            _unit.Save();
        }

        public void Update(ServiceDTO dto)
        {
            var service = _mapper.Map<Service>(dto);
            service.UpdatedAt = DateTime.UtcNow;

            _unit.ServiceRepo.Update(service);
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
    }
}
