namespace TaskManager.DTOs
{
    // Kullanıcı giriş işlemi başarılı olduğunda döndürülen yanıt nesnesi
    public class LoginResponseDto
    {
        public string Token { get; set; }        // JWT erişim token'ı
        public string UserName { get; set; }     // Kullanıcının kullanıcı adı
        public string Email { get; set; }        // Kullanıcının email adresi
        public string FirstName { get; set; }    // Kullanıcının adı
        public string LastName { get; set; }      // Kullanıcının soyadı
        public DateTime Expiration { get; set; }  // Token'ın geçerlilik süresi
    }
}