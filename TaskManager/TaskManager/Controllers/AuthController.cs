using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.DTOs;
using TaskManager.Models;
using TaskManager.Services.Jwt;
using TaskManager.Services.Users;

namespace TaskManager.Controllers
{
    // API controller olarak işaretlenir ve route yapılandırması
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Dependency injection için private readonly alanlar
        private readonly UserManager<User> _userManager;        
        private readonly SignInManager<User> _signInManager;    
        private readonly IJwtService _jwtService;               
        private readonly IUserService _userService;             

        // Constructor - bağımlılıkları enjekte eder
        public AuthController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _userService = userService;
        }

        /// <summary>
        /// Kullanıcıyı kayıt etmek için kullanılan api
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]  // POST api/auth/register endpoint'i
        public async Task<string> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            // Kullanıcı kayıt işlemini user service'e yönlendirir
            return await _userService.RegisterAsync(registerDTO);
        }

        /// <summary>
        /// Kullanıcının giriş yapması için kullanılan api
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]     // POST api/auth/login endpoint'i
        public async Task<LoginResponseDto> LoginAsync(LoginDTO loginDto)
        {
            // Kullanıcı giriş işlemini user service'e yönlendirir
            return await _userService.LoginAsync(loginDto);
        }
    }
}