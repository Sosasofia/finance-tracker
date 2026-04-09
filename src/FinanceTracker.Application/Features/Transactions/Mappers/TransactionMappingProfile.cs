using AutoMapper;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Transactions.Mappers;

public class TransactionMappingProfile : Profile
{
    public TransactionMappingProfile()
    {
        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Money.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Money.Currency))
            .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.Installments))
            .ForMember(dest => dest.Reimbursement, opt => opt.MapFrom(src => src.Reimbursement));

        CreateMap<Transaction, TransactionExportDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Money.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Money.Currency))
            .ForMember(dest => dest.Category, opt =>
                opt.MapFrom(src => src.Category != null ? src.Category.Name : "Sin Categoría"))
            .ForMember(dest => dest.PaymentMethod, opt =>
                opt.MapFrom(src => src.PaymentMethod != null ? src.PaymentMethod.Name : "N/A"))
            .ForMember(dest => dest.HasReimbursement, opt =>
                opt.MapFrom(src => src.ReimbursementId != null));
    }
}
