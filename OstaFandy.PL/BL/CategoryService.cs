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

        public CategoryDTO? GetById(int id)
        {
            var category = _unit.CategoryRepo.GetById(id);
            return category == null ? null : _mapper.Map<CategoryDTO>(category);
        }

        public void Add(CategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = true;

            _unit.CategoryRepo.Insert(category);
            _unit.Save();
        }

        public void Update(CategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
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
    }
}
