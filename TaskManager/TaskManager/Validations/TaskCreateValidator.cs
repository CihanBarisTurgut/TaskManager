using FluentValidation;
using TaskManager.DTOs;

namespace TaskManager.Validations
{
    // CreateTaskDTO için FluentValidation doğrulama sınıfı
    public class TaskCreateValidator : AbstractValidator<CreateTaskDTO>
    {
        public TaskCreateValidator()
        {
            // Title alanı için doğrulama kuralları
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Görev başlığı zorunludur.")
            .Length(1, 200).WithMessage("Görev başlığı 1-200 karakter arasında olmalıdır.");

            // Description alanı için doğrulama kuralları (isteğe bağlı)
            RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Görev açıklaması en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description)); // Sadece dolu ise kontrol et

            // UserId alanı için doğrulama kuralları
            RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID'si gereklidir.");

            // DueDate alanı için doğrulama kuralları (isteğe bağlı)
            RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Bitiş tarihi bugünden önce olamaz.")
            .When(x => x.DueDate.HasValue); // Sadece değer varsa kontrol et
            // Type alanı için doğrulama kuralları
            RuleFor(x => x.Type)
            .InclusiveBetween(1, 3).WithMessage("Görev tipi 1 (Günlük), 2 (Haftalık) veya 3 (Aylık) olmalıdır.");
        }
    }
}