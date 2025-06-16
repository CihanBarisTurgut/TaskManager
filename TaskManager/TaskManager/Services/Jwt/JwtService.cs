// Services/JwtService.cs
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Constants;
using TaskManager.Models;

namespace TaskManager.Services.Jwt
{
    // JWT token oluşturma ve yönetimi için servis sınıfı
    public class JwtService : IJwtService
    {
        // Konfigürasyon ayarlarına erişim için dependency injection
        private readonly IConfiguration _configuration;
        // Constructor - konfigürasyon servisini enjekte eder
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // Kullanıcı için JWT token oluşturma metodu
        public async Task<string> GenerateToken(User user)
        {
            // appsettings.json'dan JWT ayarlarını okur
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];     // Token imzalama anahtarı
            var issuer = jwtSettings["Issuer"];           // Token yayınlayıcısı
            var audience = jwtSettings["Audience"];       // Token hedef kitlesi
            // Secret key'i byte array'e çevirir
            var key = Encoding.ASCII.GetBytes(secretKey);
            // Token descriptor'ını oluşturur
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Token içerisine kullanıcı bilgilerini claim olarak ekler
                Subject = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // Kullanıcı ID'si
                   new Claim(ClaimTypes.Name, user.UserName),                 // Kullanıcı adı
                   new Claim(ClaimTypes.Email, user.Email),                   // Email adresi
                   new Claim(ClaimConstants.FirstName, user.FirstName),       // Ad
                   new Claim(ClaimConstants.LastName, user.LastName)          // Soyad
               }),
                Expires = DateTime.UtcNow.AddHours(24),                        // Token 24 saat geçerli
                Issuer = issuer,                                               // Token yayınlayıcısı
                Audience = audience,                                           // Token hedef kitlesi
                SigningCredentials = new SigningCredentials(                   // Token imzalama bilgileri
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            // Token handler oluşturur ve token'ı yaratır
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Token'ı string formatında döndürür
            return tokenHandler.WriteToken(token);
        }
    }
}