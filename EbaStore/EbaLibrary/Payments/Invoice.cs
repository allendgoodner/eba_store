using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary.Payments
{
    public class InvoiceType
    {
        public static DiscriminatedUnion TypeSelector = new DiscriminatedUnion(new Dictionary<string, Type>
            {
                { "Receivable",  typeof(ReceivableInvoice) },
                { "Payable", typeof(PayableInvoice) }
            });
    }
    
    public class ReceivableInvoice
    {
        public Guid InvoiceId { get; set; }
        public DateTime OrderDate_utc { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime? PaidDate_utc { get; set; }

        public ReceivableInvoice(Guid aggregateId, DateTime orderDate, decimal amountDue)
        {
            InvoiceId = aggregateId;
            OrderDate_utc = orderDate;
            AmountDue = amountDue;
            PaidDate_utc = null;
        }
    }

    public class PayableInvoice
    {
        public Guid InvoiceId { get; set; }
        public string VendorInvoiceId { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime InvoiceReceivedDate { get; set; }
        public DateTime? InvoicePaidDate { get; set; }
        public DateTime? DisputedDate { get; set; }
        public string OriginalVendorInvoice { get; set; }
        public string AuthorizationId { get; set; }

        public PayableInvoice(string vendorId, decimal amount, DateTime receivedDate, string originalInvoice)
        {
            InvoiceId = Guid.NewGuid();
            VendorInvoiceId = vendorId;
            AmountDue = amount;
            InvoiceReceivedDate = receivedDate;
            OriginalVendorInvoice = originalInvoice;
        }
    }
}
