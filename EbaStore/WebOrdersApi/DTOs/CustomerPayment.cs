using EbaLibrary.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOrdersApi.DTOs
{
    public class CustomerPayment
    {
        public Guid AggregateId { get; set; }
        public CreditCardPayment Payment { get; set; }
    }
}
