
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
        // Uygulama yap�land�r�c�s�n� olu�turur
        var builder = WebApplication.CreateBuilder(args);

        // Global hata yakalama
        try
        {
            Console.WriteLine("=== APPLICATION STARTING ===");

            // Veritaban� ba�lam�n� yap�land�r�r (SQL Server)
            builder.Services.AddDbContext<AppDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity yap�land�rmas� - int tipinde ID kullan�r
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                // �ifre gereksinimleri
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // Kullan�c� ayarlar�
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Giri� ayarlar�
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AppDataContext>()
            .AddDefaultTokenProviders();
            // JWT kimlik do�rulama yap�land�rmas�
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
                    ValidateIssuerSigningKey = true,        // �mza anahtar�n� do�rula
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,                  // Token yay�nlay�c�s�n� do�rula
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,                // Token hedef kitlesini do�rula
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,                // Token ge�erlilik s�resini do�rula
                    ClockSkew = TimeSpan.Zero
                };
            });
            // AutoMapper yap�land�rmas� (MappingProfile yoksa yorum sat�r�)
            // builder.Services.AddAutoMapper(typeof(MappingProfile));
            // �zel servisler i�in dependency injection
            builder.Services.AddTransient<IJwtService, JwtService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ITaskService, TaskService>();
            builder.Services.AddTransient<ITaskRepository, TaskRepository>();

            // FluentValidation yap�land�rmas� - assembly'den validator'lar� otomatik y�kler
            builder.Services.AddValidatorsFromAssemblyContaining<TaskManager.Validations.RegisterValidator>();

            // Controller'lar ve JSON yap�land�rmas�
            builder.Services.AddControllers()
             .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;    // camelCase property isimleri
                 options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;     // camelCase dictionary anahtarlar�
                 options.JsonSerializerOptions.WriteIndented = true;                                 // Okunabilir JSON format�
             });

            // FluentValidation entegrasyonu (tekrar eklenmi� - duplikasyon)
            builder.Services.AddValidatorsFromAssemblyContaining<TaskManager.Validations.RegisterValidator>();
            builder.Services.AddFluentValidationAutoValidation();      // Otomatik validasyon
            builder.Services.AddFluentValidationClientsideAdapters();  // Client-side validasyon deste�i

            // Swagger/OpenAPI yap�land�rmas�
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task Manager API",
                    Version = "v1",
                    Description = "G�rev Y�neticisi API - Kullan�c�lar�n g�nl�k, haftal�k ve ayl�k g�revlerini y�netebilece�i sistem"
                });

                // JWT Authorization i�in Swagger konfig�rasyonu
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Swagger'da JWT token kullan�m� i�in g�venlik gereksinimleri
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
            // CORS politikas� - t�m originlere izin verir
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)  // T�m originlere izin ver
                          .AllowAnyMethod()                     // T�m HTTP methodlar�na izin ver
                          .AllowAnyHeader()                     // T�m header'lara izin ver
                          .AllowCredentials();                  // Credentials'a izin ver
                });
            });
            // Uygulamay� olu�turur

            var app = builder.Build();
            // HTTP request pipeline yap�land�rmas�
            if (app.Environment.IsDevelopment())
            {
                // Development ortam�nda Swagger'� aktif eder
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API V1");
                    c.RoutePrefix = "swagger";                                          // swagger/index.html adresinden eri�im
                    c.DefaultModelsExpandDepth(-1);                                     // Model �emalar�n� gizle
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Endpoint'leri kapal� g�ster
                });
            }

            app.UseCors("AllowAll");        // CORS middleware'ini en ba�a ekler

            app.UseHttpsRedirection();      // HTTPS y�nlendirmesi

            app.UseAuthentication();        // Kimlik do�rulama middleware'i
            app.UseAuthorization();         // Yetkilendirme middleware'i

            app.MapControllers();           // Controller endpoint'lerini haritaland�r�r

            // Veritaban� migration ve seed data i�lemleri
            try
            {
                Console.WriteLine("=== DATABASE OPERATIONS STARTING ===");
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDataContext>();

                    // Veritaban� migration'lar�n� �al��t�r�r
                    Console.WriteLine("Running migrations...");
                    context.Database.Migrate();
                    Console.WriteLine("Migrations completed successfully!");
                }
                Console.WriteLine("=== DATABASE OPERATIONS COMPLETED ===");
            }
            catch (Exception ex)
            {
                // Veritaban� hata y�netimi
                Console.WriteLine("=== DATABASE ERROR ===");
                Console.WriteLine($"Database Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error occuring when database creating.");
            }
            Console.WriteLine("=== STARTING WEB APPLICATION ===");
            app.Run(); // Uygulamay� ba�lat�r
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