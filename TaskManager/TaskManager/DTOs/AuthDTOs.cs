using System.ComponentModel.DataAnnotations;

namespace TaskManager.DTOs
{
    // Kullanıcı kayıt işlemi için veri transfer nesnesi
    public class RegisterDTO
    {
        [Required] // Ad alanı zorunlu
        public string FirstName { get; set; }

        [Required] // Soyad alanı zorunlu
        public string LastName { get; set; }

        [Required] // Email alanı zorunlu
        [EmailAddress] // Geçerli email formatı kontrolü
        public string Email { get; set; }

        [Required] // Kullanıcı adı zorunlu
        public string UserName { get; set; }

        [Required] // Şifre alanı zorunlu
        public string Password { get; set; }
    }

    // Kullanıcı giriş işlemi için veri transfer nesnesi
    public class LoginDTO
    {
        [Required] // Email alanı zorunlu
        public string Email { get; set; }

        [Required] // Şifre alanı zorunlu
        public string Password { get; set; }
    }

    // Kimlik doğrulama başarılı olduğunda döndürülen veri transfer nesnesi
    public class AuthResponseDTO
    {
        public string Token { get; set; }        // JWT token
        public string UserName { get; set; }     // Kullanıcı adı
        public string Email { get; set; }        // Email adresi
        public string FirstName { get; set; }    // Ad
        public string LastName { get; set; }     // Soyad
        public DateTime Expiration { get; set; } // Token bitiş tarihi
    }
}