using FluentValidation;
using System.Data;
using TaskManager.DTOs;

namespace TaskManager.Validations
{
    // RegisterDTO için FluentValidation doğrulama sınıfı
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            // FirstName alanı için doğrulama kuralları
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.") // Boş olamaz
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters."); // 2-50 karakter arası
            // LastName alanı için doğrulama kuralları
            RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")// Boş olamaz
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters."); // 2-50 karakter arası
            // Email alanı için doğrulama kuralları
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")  // Boş olamaz
            .EmailAddress().WithMessage("Invalid email format."); // Geçerli email formatı
            // UserName alanı için doğrulama kuralları
            RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")  // Boş olamaz
            .Length(3, 20).WithMessage("Username must be between 3 and 20 characters."); // 3-20 karakter arası
            // Password alanı için doğrulama kuralları
            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.") // Boş olamaz
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.") // En az 6 karakter
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter (A-Z)."); // En az bir büyük harf
        }
    }
}