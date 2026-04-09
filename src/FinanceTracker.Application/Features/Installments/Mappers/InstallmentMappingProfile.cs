using AutoMapper;
using FinanceTracker.Application.Features.Installments.Models;
using FinanceTracker.Application.Features.Installments.Queries;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Installments.Mappers;

public class InstallmentMappingProfile : Profile
{
    public InstallmentMappingProfile()
    {
        CreateMap<Installment, InstallmentDto>()
                .ReverseMap();

        CreateMap<Installment, InstallmentResponse>();
    }
}
