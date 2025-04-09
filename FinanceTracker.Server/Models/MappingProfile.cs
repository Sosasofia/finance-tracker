using AutoMapper;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Models.Response;

namespace FinanceTracker.Server.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionCreateDTO>()
                .ReverseMap();

            CreateMap<Reimburstment, ReimburstmentDTO>()
                .ReverseMap();

            CreateMap<Installment, InstallmentDTO>()
                .ReverseMap();

            CreateMap<Installment, InstallmentResponse>();

            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.Installments,
                           opt => opt.MapFrom(src => src.InstallmentsList));

            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.InstallmentsList));
        }
    }
}
