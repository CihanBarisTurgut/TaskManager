// Profiles/MappingProfile.cs
using AutoMapper;
using TaskManager.DTOs;
using TaskManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManager.Profiles
{
    // AutoMapper için nesne eşleme profilini tanımlar
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Görev eşlemeleri

            // CreateTaskDTO'dan TaskItem'a eşleme
            CreateMap<CreateTaskDTO, TaskItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())           // ID otomatik oluşturulur, ignore et
                .ForMember(dest => dest.UserId, opt => opt.Ignore())       // UserId ayrıca set edilir
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())    // CreatedAt otomatik set edilir
                .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())  // IsCompleted varsayılan değer alır
                .ForMember(dest => dest.User, opt => opt.Ignore());        // Navigation property ignore et
            // UpdateTaskDTO'dan TaskItem'a eşleme
            CreateMap<UpdateTaskDTO, TaskItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())           // ID değişmez
                .ForMember(dest => dest.UserId, opt => opt.Ignore())       // UserId değişmez
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())    // CreatedAt değişmez
                .ForMember(dest => dest.User, opt => opt.Ignore());        // Navigation property ignore et
            // TaskItem'dan TaskResponseDTO'ya eşleme (tüm propertyler otomatik eşlenir)
            CreateMap<TaskItem, TaskResponseDTO>();
            // Kullanıcı eşlemeleri
            // User'dan AuthResponseDTO'ya eşleme
            CreateMap<User, AuthResponseDTO>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())        // Token ayrıca oluşturulur
                .ForMember(dest => dest.Expiration, opt => opt.Ignore());  // Expiration ayrıca set edilir
        }
    }
}