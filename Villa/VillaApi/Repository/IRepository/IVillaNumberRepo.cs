using VillaApi.Models;

namespace VillaApi.Repository.IRepository
{
    public interface IVillaNumberRepo : IRepository<VillaNumber>
    {
        Task<VillaNumber> Update(VillaNumber entity);
    }
}
