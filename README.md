#  TaskManager API

TaskManager, kullanıcıların günlük, haftalık ve aylık görevlerini yönetebileceği RESTful API tabanlı bir görev yönetim sistemidir. Modern .NET teknolojileri kullanılarak geliştirilmiş, güvenli ve ölçeklenebilir bir mimariye sahiptir.

---
##  Teknoloji ve Mimariler

**Backend & Veritabanı**
- **ASP.NET Core 8**: Web API çatısı
- **Entity Framework Core**: ORM (Object-Relational Mapping)
- **MSSQL Server**: Veritabanı

**Kimlik Doğrulama & Güvenlik**
- **ASP.NET Core Identity**: Kullanıcı yönetimi
- **JWT (JSON Web Token)**: Token tabanlı kimlik doğrulama

**Yardımcı Kütüphaneler**
- **FluentValidation**: Gelişmiş veri doğrulama
- **AutoMapper**: Nesne eşleme (DTO ↔ Entity)
- **Swagger/OpenAPI**: API dokümantasyonu ve test

**Mimari Desenler**
- Repository Pattern
- Service Pattern
- Dependency Injection (DI)
- DTO Pattern

---
##  Temel Özellikler

- ** Güvenli Kimlik Doğrulama**: Kullanıcı kaydı, girişi ve JWT tabanlı yetkilendirme.
- ** Görev Yönetimi**: Görev oluşturma, listeleme ve durum takibi (Günlük, Haftalık, Aylık).


---
##  API Endpoint'leri

### Authentication
- `POST /api/auth/register` : Yeni bir kullanıcı kaydı oluşturur.
- `POST /api/auth/login` : Kullanıcı girişi yaparak JWT token alır.

### Tasks
- `POST /api/task/create` : (Yetki Gerekli) Yeni bir görev oluşturur.
- `POST /api/task/list` : (Yetki Gerekli) Tüm görevleri ve raporları listeler.

---
##  Projeyi Çalıştırma

### Gereksinimler
- .NET 8 SDK
- MSSQL Server (veya LocalDB)

### Kurulum Adımları
1.  Repository'yi klonlayın: `git clone <repository-url>`
2.  `TaskManager/appsettings.json` dosyasında `ConnectionStrings` bölümünü kendi veritabanı bilgilerinize göre güncelleyin.
3.  Package Manager Console'da `Update-Database` komutunu çalıştırarak veritabanını oluşturun.
4.  Projeyi çalıştırın: `dotnet run`
5.  API'yi test etmek için `https://localhost:PORT/swagger` adresine gidin.

---
## 🔧 Yapılandırma (Configuration)

**Veritabanı Bağlantısı (`appsettings.json`)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TaskManagerDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**JWT Ayarları (`appsettings.json`)**
```json
{
  "JwtSettings": {
    "SecretKey": "pitonkullanicilariicinsupergizligirisanahtari",
    "Issuer": "TaskManagerAPI",
    "Audience": "TaskManagerUsers"
  }
}
```
**Kayıt olduktan sonra jwt token döner.**
![auth](https://github.com/user-attachments/assets/ddd1dfa4-f2bc-4a65-9f69-1489aa1d5e18)
#
**Fluent validation kullanımı.**
![fluentvalidation](https://github.com/user-attachments/assets/e378ed41-1de5-4cf4-98ce-de8d2fcf253e)
#
**Tasklar'ı listeler.**
![listtask](https://github.com/user-attachments/assets/884bffb4-8c7e-4523-b94d-b57c1a747c17)



##  Başlangıç ve Yapılandırma (Configuration & Startup)
---
**`Program.cs`**
> ASP.NET Core uygulamasının başlangıç noktasıdır. **Dependency Injection**, veritabanı bağlamı, **Identity**, **JWT** kimlik doğrulama, **FluentValidation**, **Swagger** ve **CORS** yapılandırmalarını içerir. Uygulama başlatma, middleware pipeline ve hata yönetimi işlemlerini gerçekleştirir.

**`AppDataContext.cs`**
> Entity Framework Core ve Identity kullanarak veritabanı `context`'ini oluşturur. Kullanıcı ve görev tabloları için yapılandırmaları, veri kısıtlamaları, ilişkiler ve performans `index`'lerini tanımlar.

**`MappingProfile.cs`**
> **AutoMapper** kütüphanesi için nesne eşleme kurallarını tanımlar. DTO'lar ile Model sınıfları arasında veri transferi yaparken hangi alanların otomatik eşleneceğini, hangilerinin göz ardı edileceğini belirtir. Bu sayede manuel `mapping` yapmak yerine otomatik dönüşüm sağlar.

**`ClaimConstants.cs`**
> JWT token'larda veya kimlik doğrulama işlemlerinde kullanılacak `claim` anahtarlarını **sabit değerler** olarak tutar.

##  Modeller ve Numaralandırmalar (Models & Enums)
---
**`User.cs`**
> ASP.NET Core Identity'nin `IdentityUser` sınıfını genişletir. Temel kimlik doğrulama özelliklerine ek olarak **ad, soyad** ve **görev koleksiyonu** ekler. `Id`, `UserName`, `Email` gibi özellikler Identity'den otomatik olarak miras alınır.

**`TaskItem.cs`**
> Veritabanındaki `Görevler (Tasks)` tablosunu temsil eden model sınıfıdır. Her görevin başlık, açıklama, bitiş tarihi, tamamlanma durumu ve hangi kullanıcıya ait olduğu bilgilerini tutar. `User` ile **one-to-many** ilişki kurar.

**`TaskTypes.cs`**
> Görev türlerini kategorize etmek için kullanılan bir `enum` yapısıdır. Görevlerin periyodikliğini belirtir (günlük, haftalık veya aylık) ve veritabanında `integer` değerler olarak saklanır.

##  DTO'lar (Data Transfer Objects)
---
**`AuthDTOs.cs`**
> Kullanıcı kimlik doğrulama işlemleri için veri transferi yapar. `RegisterDTO` kayıt için, `LoginDTO` giriş için gerekli alanları içerir.

**`LoginResponseDTOs.cs`**
> Kullanıcı giriş işlemi başarılı olduğunda client'a döndürülen yanıt verilerini taşır. **JWT token**, kullanıcı bilgileri ve token'ın ne zaman sona ereceği bilgilerini içerir.

**`TaskDTO.cs`**
> Görev yönetimi işlemleri için farklı veri transfer nesnelerini tanımlar. `CreateTaskDTO` yeni görev oluşturmak, `UpdateTaskDTO` güncelleme, `TaskResponseDTO` görev detayları ve `TaskReportDTO` ise görev istatistikleri ve raporları için kullanılır.

##  Doğrulayıcılar (Validators)
---
**`RegisterValidator.cs`**
> **FluentValidation** kütüphanesi kullanarak `RegisterDTO` için detaylı doğrulama kuralları tanımlar. Her alan için zorunluluk, uzunluk ve format kontrollerini yapar. Şifre için **güçlü parola politikası** uygular ve kullanıcı dostu hata mesajları sağlar.

**`TaskCreateValidator.cs`**
> **FluentValidation** kütüphanesi kullanarak `CreateTaskDTO` için kapsamlı doğrulama kuralları tanımlar. Başlık zorunluluğu, karakter sınırları, kullanıcı ID geçerliliği ve tarih kontrolü yapar. Veri bütünlüğünü korur ve anlaşılır Türkçe hata mesajları sağlar.

##  Denetleyiciler (Controllers)
---
**`AuthController.cs`**
> Kimlik doğrulama (authentication) işlemlerini yönetir. Kullanıcı **kaydı (register)** ve **giriş (login)** işlemlerini API endpoint'leri olarak sunar. İş mantığını `Service` katmanına devredip sadece HTTP isteklerini yönetir.

**`TaskController.cs`**
> Görev (task) yönetimi işlemlerini yönetir. Yeni görev oluşturma ve mevcut görevleri listeleme işlemlerini API endpoint'leri olarak sunar. İş mantığını `Service` katmanına devredip sadece HTTP isteklerini yönetir.

##  Servisler (Business Logic)
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

##  Repositories (Data Access Layer)
---
**`ITaskRepository.cs` / `TaskRepository.cs`**
> **Arayüz (Interface):** **Repository Pattern**'in bir parçası olarak, görevlerle ilgili veritabanı işlemlerinin imzalarını belirtir. Bu, Dependency Injection ve Unit Testing için kritik öneme sahiptir.
> **Sınıf (Class):** Görevlerle ilgili veritabanı işlemlerini (`CRUD`) yönetir. `Service` katmanından gelen istekleri Entity Framework Core kullanarak veritabanı sorgularına çevirir.
