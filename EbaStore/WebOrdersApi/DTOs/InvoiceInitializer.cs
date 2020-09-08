using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOrdersApi.DTOs
{
    public class InvoiceInitializer
    {
        public Guid AggregateId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public string PayOrReceive { get; set; }
    }
}
