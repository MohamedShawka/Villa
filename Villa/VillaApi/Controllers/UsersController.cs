using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VillaApi.Models;
using VillaApi.Models.Dtos;
using VillaApi.Repository.IRepository;

namespace VillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public UsersController(IUserRepo userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            this._response = new ();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterationDto registerDto)
        {
            bool IfUserNameUnique = _userRepo.IsUniqueUser(registerDto.UserName);
            
            if (!IfUserNameUnique)

            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("User already exists");
                return BadRequest(_response);
            }
             var user= await _userRepo.Register(registerDto);
            if (user == null) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Error While Registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var loginResponse= await _userRepo.Login(login);
            if(loginResponse.User==null || string.IsNullOrEmpty(loginResponse.Token))
            {

                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Invalid username or password");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }
    }
}
