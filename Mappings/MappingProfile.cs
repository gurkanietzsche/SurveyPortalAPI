// Mappings/MappingProfile.cs
using AutoMapper;
using SurveyPortalAPI.DTOs;
using SurveyPortalAPI.Models;

namespace SurveyPortalAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<ApplicationUser, UserDTO>();
            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Survey mappings
            CreateMap<Survey, SurveyDTO>()
                .ForMember(dest => dest.CreatedByName,
                    opt => opt.MapFrom(src => $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}"));

            CreateMap<CreateSurveyDTO, Survey>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Questions, opt => opt.Ignore());

            CreateMap<UpdateSurveyDTO, Survey>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore());

            // Question mappings
            CreateMap<Question, QuestionDTO>();
            CreateMap<CreateQuestionDTO, Question>()
                .ForMember(dest => dest.Options, opt => opt.Ignore());

            // Question Option mappings
            CreateMap<QuestionOption, QuestionOptionDTO>();
            CreateMap<CreateQuestionOptionDTO, QuestionOption>();

            // Survey Response mappings
            CreateMap<SurveyResponse, SurveyResponseDTO>()
                .ForMember(dest => dest.SurveyTitle, opt => opt.MapFrom(src => src.Survey.Title))
                .ForMember(dest => dest.UserFullName,
                    opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<CreateSurveyResponseDTO, SurveyResponse>()
                .ForMember(dest => dest.SubmittedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Answers, opt => opt.Ignore());

            // Answer mappings
            CreateMap<Answer, AnswerDTO>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.Text))
                .ForMember(dest => dest.SelectedOptionText, opt => opt.MapFrom(src => src.SelectedOption.Text));

            CreateMap<CreateAnswerDTO, Answer>();
        }
    }
}