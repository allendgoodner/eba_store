using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EbaLibrary.Orders
{
    public class OrderDetails
    {
        public Dictionary<int, IList<OrderItem>> SubOrders { get; }

        public OrderDetails()
        {
            SubOrders = new Dictionary<int, IList<OrderItem>>();
        }

        public void AddItem(int idx, OrderItem item)
        {
            IList<OrderItem> items = new List<OrderItem>();
            if (SubOrders.TryGetValue(idx, out items))
            {
                AddToExistingSubOrder(idx, item);
                return;
            }
            AddNewSubOrder(idx, item);
        }

        public void RemoveItem(int idx, string itemName)
        {
            if (!SubOrders.ContainsKey(idx)) return;
            var item = SubOrders[idx].Where(x => x.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
                                     .Last();

            SubOrders[idx].Remove(item);
        }

        private void AddNewSubOrder(int idx, OrderItem item)
        {
            IList<OrderItem> items = new List<OrderItem>();
            items.Add(item);
            SubOrders.Add(idx, items);
        }

        private void AddToExistingSubOrder(int idx, OrderItem item)
        {
            SubOrders[idx].Add(item);
        }
    }
}
