using VillaApi.Models;
using VillaApi.Models.Dtos;

namespace VillaApi.Repository.IRepository
{
    public interface IUserRepo
    {
        bool IsUniqueUser(string username);

        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<LocalUser> Register(RegisterationDto registerationDto);

    }
}
