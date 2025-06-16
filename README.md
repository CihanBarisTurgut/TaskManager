# 🚀 Proje Yapısı ve Dosya Açıklamaları

Bu doküman, projenin mimarisini ve içerisindeki temel sınıfların görevlerini açıklamaktadır.

## 🏁 Başlangıç ve Yapılandırma (Configuration & Startup)
---
**`Program.cs`**
> ASP.NET Core uygulamasının başlangıç noktasıdır. **Dependency Injection**, veritabanı bağlamı, **Identity**, **JWT** kimlik doğrulama, **FluentValidation**, **Swagger** ve **CORS** yapılandırmalarını içerir. Uygulama başlatma, middleware pipeline ve hata yönetimi işlemlerini gerçekleştirir.

**`AppDataContext.cs`**
> Entity Framework Core ve Identity kullanarak veritabanı `context`'ini oluşturur. Kullanıcı ve görev tabloları için yapılandırmaları, veri kısıtlamaları, ilişkiler ve performans `index`'lerini tanımlar.

**`MappingProfile.cs`**
> **AutoMapper** kütüphanesi için nesne eşleme kurallarını tanımlar. DTO'lar ile Model sınıfları arasında veri transferi yaparken hangi alanların otomatik eşleneceğini, hangilerinin göz ardı edileceğini belirtir. Bu sayede manuel `mapping` yapmak yerine otomatik dönüşüm sağlar.

**`ClaimConstants.cs`**
> JWT token'larda veya kimlik doğrulama işlemlerinde kullanılacak `claim` anahtarlarını **sabit değerler** olarak tutar.

## 📁 Modeller ve Numaralandırmalar (Models & Enums)
---
**`User.cs`**
> ASP.NET Core Identity'nin `IdentityUser` sınıfını genişletir. Temel kimlik doğrulama özelliklerine ek olarak **ad, soyad** ve **görev koleksiyonu** ekler. `Id`, `UserName`, `Email` gibi özellikler Identity'den otomatik olarak miras alınır.

**`TaskItem.cs`**
> Veritabanındaki `Görevler (Tasks)` tablosunu temsil eden model sınıfıdır. Her görevin başlık, açıklama, bitiş tarihi, tamamlanma durumu ve hangi kullanıcıya ait olduğu bilgilerini tutar. `User` ile **one-to-many** ilişki kurar.

**`TaskTypes.cs`**
> Görev türlerini kategorize etmek için kullanılan bir `enum` yapısıdır. Görevlerin periyodikliğini belirtir (günlük, haftalık veya aylık) ve veritabanında `integer` değerler olarak saklanır.

## 📦 DTO'lar (Data Transfer Objects)
---
**`AuthDTOs.cs`**
> Kullanıcı kimlik doğrulama işlemleri için veri transferi yapar. `RegisterDTO` kayıt için, `LoginDTO` giriş için gerekli alanları içerir.

**`LoginResponseDTOs.cs`**
> Kullanıcı giriş işlemi başarılı olduğunda client'a döndürülen yanıt verilerini taşır. **JWT token**, kullanıcı bilgileri ve token'ın ne zaman sona ereceği bilgilerini içerir.

**`TaskDTO.cs`**
> Görev yönetimi işlemleri için farklı veri transfer nesnelerini tanımlar. `CreateTaskDTO` yeni görev oluşturmak, `UpdateTaskDTO` güncelleme, `TaskResponseDTO` görev detayları ve `TaskReportDTO` ise görev istatistikleri ve raporları için kullanılır.

## 🛡️ Doğrulayıcılar (Validators)
---
**`RegisterValidator.cs`**
> **FluentValidation** kütüphanesi kullanarak `RegisterDTO` için detaylı doğrulama kuralları tanımlar. Her alan için zorunluluk, uzunluk ve format kontrollerini yapar. Şifre için **güçlü parola politikası** uygular ve kullanıcı dostu hata mesajları sağlar.

**`TaskCreateValidator.cs`**
> **FluentValidation** kütüphanesi kullanarak `CreateTaskDTO` için kapsamlı doğrulama kuralları tanımlar. Başlık zorunluluğu, karakter sınırları, kullanıcı ID geçerliliği ve tarih kontrolü yapar. Veri bütünlüğünü korur ve anlaşılır Türkçe hata mesajları sağlar.

## 🎮 Denetleyiciler (Controllers)
---
**`AuthController.cs`**
> Kimlik doğrulama (authentication) işlemlerini yönetir. Kullanıcı **kaydı (register)** ve **giriş (login)** işlemlerini API endpoint'leri olarak sunar. İş mantığını `Service` katmanına devredip sadece HTTP isteklerini yönetir.

**`TaskController.cs`**
> Görev (task) yönetimi işlemlerini yönetir. Yeni görev oluşturma ve mevcut görevleri listeleme işlemlerini API endpoint'leri olarak sunar. İş mantığını `Service` katmanına devredip sadece HTTP isteklerini yönetir.

## ⚙️ Servisler (Business Logic)
---
**`IUserService.cs` / `UserService.cs`**
> **Arayüz (Interface):** Kullanıcı kaydı ve giriş işlemlerinin imzalarını belirtir.
> **Sınıf (Class):** Kullanıcı kimlik doğrulama işlemlerini yönetir. Kullanıcı kaydı ve giriş işlemlerini `Identity` framework ile gerçekleştirir, validasyon kontrolü yapar, JWT token oluşturur ve hata yönetimi içerir.

**`ITaskService.cs` / `TaskService.cs`**
> **Arayüz (Interface):** Görev oluşturma ve raporlama işlemlerinin imzalarını belirtir.
> **Sınıf (Class):** Görev işlemleri için **business logic**'i yönetir. Yeni görev oluşturma ve görev raporlama işlemlerini gerçekleştirir. `Repository` katmanını kullanarak görev tiplerine göre gruplayarak detaylı istatistiksel raporlar oluşturur.

**`IJwtService.cs` / `JwtService.cs`**
> **Arayüz (Interface):** JWT token oluşturma işleminin imzasını belirtir.
> **Sınıf (Class):** Kullanıcı bilgilerini alıp güvenli bir **JWT token** üretir. Token içerisine kullanıcı `claim`'lerini ekler, geçerlilik süresi belirler ve güvenli imzalama yapar. Kimlik doğrulama sisteminin ana bileşenidir.

## 🗄️ Repositories (Data Access Layer)
---
**`ITaskRepository.cs` / `TaskRepository.cs`**
> **Arayüz (Interface):** **Repository Pattern**'in bir parçası olarak, görevlerle ilgili veritabanı işlemlerinin imzalarını belirtir. Bu, Dependency Injection ve Unit Testing için kritik öneme sahiptir.
> **Sınıf (Class):** Görevlerle ilgili veritabanı işlemlerini (`CRUD`) yönetir. `Service` katmanından gelen istekleri Entity Framework Core kullanarak veritabanı sorgularına çevirir.
