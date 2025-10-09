using AutoMapper;

using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, CreateTransaction>()
            .ReverseMap();

        CreateMap<Transaction, UpdateTransaction>()
            .ReverseMap();

        CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.Installments,
                           opt => opt.MapFrom(src => src.InstallmentsList));

        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.InstallmentsList))
            .ForMember(dest => dest.Reimbursement, opt => opt.MapFrom(src => src.Reimbursement));
    }
}
