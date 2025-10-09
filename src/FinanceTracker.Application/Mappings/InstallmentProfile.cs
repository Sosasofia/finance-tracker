using AutoMapper;

using FinanceTracker.Application.Features.Installments;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class InstallmentProfile : Profile
{
    public InstallmentProfile()
    {
        CreateMap<Installment, InstallmentDto>()
                .ReverseMap();

        CreateMap<Installment, InstallmentResponse>();
    }
}
