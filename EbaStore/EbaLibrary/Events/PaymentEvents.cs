using EbaLibrary.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary.Events
{
    

    public abstract class PaymentEvent 
    {
        public Guid InvoiceId { get; set; }
        public DateTime DateTimeStamp_utc { get; set; }
    }

    public class InvoiceCreated : PaymentEvent
    {
        public ReceivableInvoice Invoice { get; set; }
    }

    public class InvoicePaid : PaymentEvent
    {
        public Payment PaymentDetail { get; set; }
    }
}
