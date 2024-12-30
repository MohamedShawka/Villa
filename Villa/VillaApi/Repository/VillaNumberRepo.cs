using VillaApi.Data;
using VillaApi.Models;
using VillaApi.Repository.IRepository;

namespace VillaApi.Repository
{
    public class VillaNumberRepo : Repository<VillaNumber>, IVillaNumberRepo
    {
        private readonly AppDbContext _context;
        public VillaNumberRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<VillaNumber> Update(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
