using EbaLibrary.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary.Events
{
    public abstract class OrderEvent
    {
        public Guid OrderId { get; set; }
        public DateTime DateTimeStamp_utc { get; set; }
    }

    public class OrderStarted : OrderEvent
    { 
        public string ServerId { get; set; }
    }

    public class SubOrderItemAdded : OrderEvent
    {
        public int OrderIndex { get; set; }
        public OrderItem Item { get; set; }
    }

    public class SubOrderItemRemoved : OrderEvent
    {
        public int OrderIndex { get; set; }
        public string Name { get; set; }
    }

    public class OrderSubmitted : OrderEvent {
        public Guid InvoiceId { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderConfirmed : OrderEvent { }

    public class OrderPrepared : OrderEvent { }

    public class OrderCancelled : OrderEvent
    {
        public string AuthorizationId { get; set; }
    }

    public class OrderPayed : OrderEvent
    {
        public Guid PaymentId { get; set; }
    }

    public class OrderCompleted : OrderEvent { }
}
