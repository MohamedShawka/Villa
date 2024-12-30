using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VillaApi.Data;
using VillaApi.Models;
using VillaApi.Models.Dtos;
using VillaApi.Repository.IRepository;

namespace VillaApi.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _db;
        private string SecretKey;
        public UserRepo(AppDbContext db,IConfiguration configuration)
        {
            _db = db;
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x=>x.UserName.ToLower()==username.ToLower());
            if (user == null) {
                return true;
            }
                return false;

            }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName == loginDto.UserName 
            && u.Password==loginDto.Password);
            if (user == null)
            {
                return new  LoginResponseDto 
                {
                    Token = "",
                    User = null
                };
            }
            //if user was found generate JWT token
            var tokenHandler=new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor { 
            
                Subject=new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new (new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDto = new()
            {
                Token = tokenHandler.WriteToken(token),
                User=user
            };
            return loginResponseDto;
        }

        public async Task<LocalUser> Register(RegisterationDto registerationDto)
        {
            LocalUser localUser = new() {
            UserName = registerationDto.UserName,
            Password = registerationDto.Password,
            Name = registerationDto.Name,
            Role = registerationDto.Role
            };
            _db.LocalUsers.Add(localUser);
            await _db.SaveChangesAsync();
            localUser.Password = "";
            return localUser;
        }
    }
}
