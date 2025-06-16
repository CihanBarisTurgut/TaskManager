using Microsoft.AspNetCore.Identity;
using TaskManager.DTOs;
using TaskManager.Models;
using TaskManager.Services.Jwt;

namespace TaskManager.Services.Users
{
    // Kullanıcı işlemleri için business logic servis sınıfı
    public class UserService : IUserService
    {
        // Service Provider ile dependency'leri lazy loading ile alır
        private readonly IServiceProvider _serviceProvider;

        // Constructor - service provider'ı enjekte eder
        public UserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        // Kullanıcı giriş işlemi
        public async Task<LoginResponseDto> LoginAsync(LoginDTO loginDto)
        {
            // Gerekli servisleri service provider'dan alır
            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var signInManager = _serviceProvider.GetRequiredService<SignInManager<User>>();
            var jwtService = _serviceProvider.GetRequiredService<IJwtService>();
            // Email adresine göre kullanıcıyı bulur
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return new LoginResponseDto(); // Kullanıcı bulunamadı, boş response döner

            // Şifre kontrolü yapar
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return new LoginResponseDto(); // Şifre yanlış, boş response döner

            // Başarılı giriş için JWT token oluşturur
            var token = await jwtService.GenerateToken(user);

            // Kullanıcı bilgileri ve token ile response oluşturur
            return new LoginResponseDto()
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Expiration = DateTime.UtcNow.AddHours(24) // Token 24 saat geçerli
            };
        }

        // Kullanıcı kayıt işlemi
        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                // Gerekli servisleri service provider'dan alır
                var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
                var jwtService = _serviceProvider.GetRequiredService<IJwtService>();
                // VALIDATION RULES - Doğrulama kuralları
                var errors = new List<string>();
                // Email adresinin daha önce kullanılıp kullanılmadığını kontrol eder
                var existingUserByEmail = await userManager.FindByEmailAsync(registerDTO.Email);
                if (existingUserByEmail != null)
                    errors.Add("Bu email adresi zaten kullanımda");
                // Kullanıcı adının daha önce kullanılıp kullanılmadığını kontrol eder
                var existingUserByUsername = await userManager.FindByNameAsync(registerDTO.UserName);
                if (existingUserByUsername != null)
                    errors.Add("Bu kullanıcı adı zaten kullanımda");
                // Hata varsa konsola yazdırır (return yapılmıyor, kod devam ediyor)
                if (errors.Any())
                {
                    errors.ForEach(x => Console.WriteLine(x));
                }
                // Yeni kullanıcı nesnesi oluşturur
                var user = new User
                {
                    FirstName = registerDTO.FirstName,
                    LastName = registerDTO.LastName,
                    Email = registerDTO.Email,
                    UserName = registerDTO.UserName
                };
                // Identity ile kullanıcıyı oluşturur
                var result = await userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                {
                    // Kullanıcı oluşturma hatalarını alır (kullanılmıyor)
                    var userCreationErrors = result.Errors.Select(e => e.Description).ToList();
                }
                // Başarılı kayıt için JWT token oluşturur ve döndürür
                var token = await jwtService.GenerateToken(user);
                return token;
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını döndürür
                return $"ERROR: {ex.Message}";
            }
        }
    }
}