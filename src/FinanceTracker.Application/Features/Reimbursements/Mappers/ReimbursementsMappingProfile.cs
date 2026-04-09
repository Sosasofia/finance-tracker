using AutoMapper;
using FinanceTracker.Application.Features.Reimbursements.Queries;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Reimbursements.Mappers;

public class ReimbursementsMappingProfile : Profile
{
    public ReimbursementsMappingProfile()
    {
        CreateMap<Reimbursement, ReimbursementDto>()
            .ReverseMap();
    }
}
