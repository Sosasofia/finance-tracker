using AutoMapper;

using FinanceTracker.Application.Features.Reimbursements;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class ReimbursementProfile : Profile
{
    public ReimbursementProfile()
    {
        CreateMap<Reimbursement, ReimbursementDto>()
            .ReverseMap();
    }
}
