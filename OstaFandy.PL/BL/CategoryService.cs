using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        public IEnumerable<CategoryDTO> GetAll()
        {
            var categories = _unit.CategoryRepo.GetAll(c => c.IsActive).ToList();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public PaginatedResult<CategoryDTO> GetAllPaginated(int pageNumber, int pageSize, string? search = null, string? status = null)
        {
            var query = _unit.CategoryRepo.GetAll();

            // ✅ Cleaned Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(search))
                );
            }

            // ✅ Status filter remains same
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                if (status == "Active")
                    query = query.Where(c => c.IsActive);
                else if (status == "Inactive")
                    query = query.Where(c => !c.IsActive);
            }

            var totalItems = query.Count();

            var items = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<CategoryDTO>
            {
                TotalItems = totalItems,
                Items = _mapper.Map<List<CategoryDTO>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public CategoryDTO? GetById(int id)
        {
            var category = _unit.CategoryRepo.GetById(id);
            return category == null ? null : _mapper.Map<CategoryDTO>(category);
        }

        public void Add(CategoryCreateDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = true;

            _unit.CategoryRepo.Insert(category);
            _unit.Save();
        }

        public void Update(CategoryDTO dto)
        {
            var category = _unit.CategoryRepo.GetById(dto.Id);
            if (category == null) throw new Exception("Category not found");

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            _unit.CategoryRepo.Update(category);
            _unit.Save();
        }

        public bool SoftDelete(int id)
        {
            var category = _unit.CategoryRepo.GetById(id);
            if (category == null) return false;

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            _unit.CategoryRepo.Update(category);
            _unit.Save();
            return true;
        }

        public bool ToggleStatus(int id)
        {
            var category = _unit.CategoryRepo.GetById(id);
            if (category == null) return false;

            if (category.IsActive)
            {
                var activeServices = _unit.ServiceRepo.GetAll(s => s.CategoryId == id && s.IsActive).Any();
                if (activeServices)
                    return false;
            }

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            _unit.CategoryRepo.Update(category);
            _unit.Save();
            return true;
        }
    }
}
