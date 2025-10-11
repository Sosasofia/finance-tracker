using AutoMapper;
using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Application.Features.Installments;
using FinanceTracker.Application.Features.Reimbursements;
using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Server.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Transaction, CreateTransactionDto>()
            .ReverseMap();

        CreateMap<Transaction, UpdateTransactionDto>()
            .ReverseMap();

        CreateMap<Reimbursement, ReimbursementDto>()
            .ReverseMap();

        CreateMap<Installment, InstallmentDto>()
            .ReverseMap();

        CreateMap<Installment, InstallmentResponse>();

        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Installments,
                       opt => opt.MapFrom(src => src.InstallmentsList));

        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.InstallmentsList))
            .ForMember(dest => dest.Reimbursement, opt => opt.MapFrom(src => src.Reimbursement));

        CreateMap<CustomCategory, CustomCategoryDto>()
            .ReverseMap();

        CreateMap<User, UserDto>();
    }
}
