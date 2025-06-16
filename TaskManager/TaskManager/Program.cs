
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Profiles;
using TaskManager.Repositories;
using TaskManager.Services.Jwt;
using TaskManager.Services.Tasks;
using TaskManager.Services.Users;

internal class Program
{
    private static void Main(string[] args)
    {
        // Uygulama yapýlandýrýcýsýný oluþturur
        var builder = WebApplication.CreateBuilder(args);

        // Global hata yakalama
        try
        {
            Console.WriteLine("=== APPLICATION STARTING ===");

            // Veritabaný baðlamýný yapýlandýrýr (SQL Server)
            builder.Services.AddDbContext<AppDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity yapýlandýrmasý - int tipinde ID kullanýr
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                // Þifre gereksinimleri
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // Kullanýcý ayarlarý
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Giriþ ayarlarý
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AppDataContext>()
            .AddDefaultTokenProviders();
            // JWT kimlik doðrulama yapýlandýrmasý
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,        // Ýmza anahtarýný doðrula
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,                  // Token yayýnlayýcýsýný doðrula
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,                // Token hedef kitlesini doðrula
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,                // Token geçerlilik süresini doðrula
                    ClockSkew = TimeSpan.Zero
                };
            });
            // AutoMapper yapýlandýrmasý (MappingProfile yoksa yorum satýrý)
            // builder.Services.AddAutoMapper(typeof(MappingProfile));
            // Özel servisler için dependency injection
            builder.Services.AddTransient<IJwtService, JwtService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ITaskService, TaskService>();
            builder.Services.AddTransient<ITaskRepository, TaskRepository>();

            // FluentValidation yapýlandýrmasý - assembly'den validator'larý otomatik yükler
            builder.Services.AddValidatorsFromAssemblyContaining<TaskManager.Validations.RegisterValidator>();

            // Controller'lar ve JSON yapýlandýrmasý
            builder.Services.AddControllers()
             .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;    // camelCase property isimleri
                 options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;     // camelCase dictionary anahtarlarý
                 options.JsonSerializerOptions.WriteIndented = true;                                 // Okunabilir JSON formatý
             });

            // FluentValidation entegrasyonu (tekrar eklenmiþ - duplikasyon)
            builder.Services.AddValidatorsFromAssemblyContaining<TaskManager.Validations.RegisterValidator>();
            builder.Services.AddFluentValidationAutoValidation();      // Otomatik validasyon
            builder.Services.AddFluentValidationClientsideAdapters();  // Client-side validasyon desteði

            // Swagger/OpenAPI yapýlandýrmasý
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task Manager API",
                    Version = "v1",
                    Description = "Görev Yöneticisi API - Kullanýcýlarýn günlük, haftalýk ve aylýk görevlerini yönetebileceði sistem"
                });

                // JWT Authorization için Swagger konfigürasyonu
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Swagger'da JWT token kullanýmý için güvenlik gereksinimleri
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                           }
                       },
                       new string[] {}
                   }
               });
            });
            // CORS politikasý - tüm originlere izin verir
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)  // Tüm originlere izin ver
                          .AllowAnyMethod()                     // Tüm HTTP methodlarýna izin ver
                          .AllowAnyHeader()                     // Tüm header'lara izin ver
                          .AllowCredentials();                  // Credentials'a izin ver
                });
            });
            // Uygulamayý oluþturur

            var app = builder.Build();
            // HTTP request pipeline yapýlandýrmasý
            if (app.Environment.IsDevelopment())
            {
                // Development ortamýnda Swagger'ý aktif eder
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API V1");
                    c.RoutePrefix = "swagger";                                          // swagger/index.html adresinden eriþim
                    c.DefaultModelsExpandDepth(-1);                                     // Model þemalarýný gizle
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Endpoint'leri kapalý göster
                });
            }

            app.UseCors("AllowAll");        // CORS middleware'ini en baþa ekler

            app.UseHttpsRedirection();      // HTTPS yönlendirmesi

            app.UseAuthentication();        // Kimlik doðrulama middleware'i
            app.UseAuthorization();         // Yetkilendirme middleware'i

            app.MapControllers();           // Controller endpoint'lerini haritalandýrýr

            // Veritabaný migration ve seed data iþlemleri
            try
            {
                Console.WriteLine("=== DATABASE OPERATIONS STARTING ===");
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDataContext>();

                    // Veritabaný migration'larýný çalýþtýrýr
                    Console.WriteLine("Running migrations...");
                    context.Database.Migrate();
                    Console.WriteLine("Migrations completed successfully!");
                }
                Console.WriteLine("=== DATABASE OPERATIONS COMPLETED ===");
            }
            catch (Exception ex)
            {
                // Veritabaný hata yönetimi
                Console.WriteLine("=== DATABASE ERROR ===");
                Console.WriteLine($"Database Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error occuring when database creating.");
            }
            Console.WriteLine("=== STARTING WEB APPLICATION ===");
            app.Run(); // Uygulamayý baþlatýr
        }
        catch (Exception ex)
        {
            // Global hata yakalama ve loglama
            Console.WriteLine($"=== APPLICATION CRASH ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            throw;
        }
    }
}