using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOrdersApi.DTOs;

namespace WebOrdersApi.BusinessObjects
{
    public class OrderUpdateValidator : AbstractValidator<OrderUpdate>
    {
        public OrderUpdateValidator()
        {
            RuleFor(o => o.OrderId).NotEmpty();
            RuleFor(o => o.Item).NotEmpty();
        }
    }
}
