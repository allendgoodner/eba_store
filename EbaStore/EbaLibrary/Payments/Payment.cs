using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary.Payments
{
    public static class PaymentType
    {
        public static DiscriminatedUnion PaymentTypes =
            new DiscriminatedUnion(new Dictionary<string, Type>
            {
                { "Cash", typeof(CashPayment) },
                { "Credit", typeof(CreditCardPayment) }
            });
    }


    public abstract class Payment
    {
        public DateTime PaymentDate_Utc { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentId { get; set; }
    }

    public class CashPayment : Payment { }

    public class CreditCardPayment : Payment
    {
        CardDetails CardDetails { get; set; }
    }
}
