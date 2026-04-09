using AutoMapper;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.PaymentMethods.Mappers;

public class PaymentMethodsMappingProfile : Profile
{
    public PaymentMethodsMappingProfile()
    {
        CreateMap<PaymentMethod, PaymentMethodDto>();
    }
}
