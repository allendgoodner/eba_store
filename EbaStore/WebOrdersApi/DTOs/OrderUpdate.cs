using EbaLibrary.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOrdersApi.DTOs
{
    public class OrderUpdate
    {
        public Guid OrderId { get; set; }
        public OrderItem Item { get; set; }
        public IEnumerable<ItemModifier> Mods { get; set; }
    }
}
