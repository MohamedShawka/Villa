using System.Linq.Expressions;
using VillaApi.Models;

namespace VillaApi.Repository.IRepository
{
    public interface IVillaRepo : IRepository<Villa>
    {

     
        Task<Villa> Update(Villa entity);
        
    }
}
