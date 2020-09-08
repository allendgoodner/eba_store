using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary.Orders
{
    public class OrderItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Instructions { get; set; }
        public IEnumerable<ItemModifier> Modifiers { get; set; }
    }

    public class ItemModifier
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ItemAdjustment Adjustment { get; set; }
    }

    public enum ItemAdjustment
    {
        None,
        Lite,
        Regular,
        Extra
    }
}
