using AutoMapper;
using FinanceTracker.Application.Features.PaymentMethods;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings
{
    public class PaymentMethodProfile : Profile
    {
        public PaymentMethodProfile()
        {
            CreateMap<PaymentMethod, PaymentMethodDto>();
            CreateMap<CreatePaymentMethodDto, PaymentMethod>();
        }
    }
}
