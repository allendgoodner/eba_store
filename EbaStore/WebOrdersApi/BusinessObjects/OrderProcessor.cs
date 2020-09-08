using EbaLibrary;
using EbaLibrary.Events;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebOrdersApi.DTOs;

namespace WebOrdersApi.BusinessObjects
{
    public interface IOrderProcessor
    {
        Task<Guid> InitializeOrder();
        Task<int> AddItemToOrder(OrderUpdate update);
        Task<int> RemoveItemFromOrder(OrderUpdate update);
        Task<string> SubmitOrder(Guid orderId);
        Task PayAndConfirmOrder(Guid invoiceId, Guid paymentId);
        Task CompleteOrder(Guid orderId);
    }

    public class OrderProcessor : IOrderProcessor
    {
        private EventHub _eventHub;

        public OrderProcessor(EventHub hub)
        {
            _eventHub = hub;
            _eventHub.Subscribe(@event =>
            {
                switch (@event)
                {
                    case InvoicePaid ip:
                        PayAndConfirmOrder(ip.InvoiceId, ip.PaymentDetail.PaymentId);
                        break;
                    case OrderPrepared op:
                        CompleteOrder(op.OrderId);
                        break;
                    default: break;
                }
            });
        }

        public Task<Guid> InitializeOrder()
        {
            var aggregateId = Guid.NewGuid();
            var order = _eventHub.GetOrInitialize(aggregateId);

            var orderStarted = new OrderStarted
            {
                OrderId = aggregateId,
                ServerId = "WEB",
                DateTimeStamp_utc = DateTime.UtcNow
            };

            _eventHub.AddEvent(aggregateId, orderStarted);
            return Task.FromResult(aggregateId);
        }

        public Task<int> AddItemToOrder(OrderUpdate update)
        {
            var order = _eventHub.GetOrInitialize(update.OrderId);
            var next = NextOrderIndex(order);

            var orderItem = new SubOrderItemAdded()
            {
                OrderId = update.OrderId,
                OrderIndex = next,
                Item = update.Item,
                DateTimeStamp_utc = DateTime.UtcNow,
            };

            _eventHub.AddEvent(orderItem.OrderId, orderItem);
            return Task.FromResult(next);
        }

        public Task<int> RemoveItemFromOrder(OrderUpdate update)
        {
            var order = _eventHub.GetOrInitialize(update.OrderId);

            var found = order.OfType<SubOrderItemAdded>()
                              .Where(ia => ia.Item.Name == update.Item.Name &&
                              ia.Item.Modifiers.CollectionsEqual(update.Item.Modifiers))
                              .FirstOrDefault();

            if (found != null)
            {
                var itemRemoved = new SubOrderItemRemoved()
                {
                    DateTimeStamp_utc = DateTime.UtcNow,
                    Name = found.Item.Name,
                    OrderId = found.OrderId,
                    OrderIndex = found.OrderIndex
                };

                _eventHub.AddEvent(itemRemoved.OrderId, itemRemoved);
            }

            return Task.FromResult(NextOrderIndex(order));
        }

        public Task<string> SubmitOrder(Guid orderId)
        {
            var order = _eventHub.GetOrInitialize(orderId);
            var items = order.OfType<SubOrderItemAdded>();
            var removed = order.OfType<SubOrderItemRemoved>()?.Select(rem => rem.OrderIndex);

            var remaining = items.Except(items.Where(i => removed.Contains(i.OrderIndex)));
            var total = items.Sum(i => i.Item.Price + i.Item.Modifiers.Sum(m => m.Price));
            var submit = new OrderSubmitted() { OrderId = orderId, DateTimeStamp_utc = DateTime.UtcNow, InvoiceId = orderId, Total = total };

            _eventHub.AddEvent(orderId, submit);

            return Task.FromResult($"{orderId}:{total}");
        }

        public Task PayAndConfirmOrder(Guid invoiceId, Guid paymentId)
        {
            var order = _eventHub.GetOrInitialize(invoiceId);

            if (!order.Any()) { throw new InvalidOperationException("Cannot pay and confirm order which has not been started."); }

            var paid = new OrderPayed
            {
                OrderId = invoiceId,
                PaymentId = paymentId,
                DateTimeStamp_utc = DateTime.UtcNow
            };

            var confirmed = new OrderConfirmed
            {
                OrderId = invoiceId,
                DateTimeStamp_utc = DateTime.UtcNow
            };

            _eventHub.AddEvent(invoiceId, paid);
            _eventHub.AddEvent(invoiceId, confirmed);

            return Task.CompletedTask;
        }

        public Task CompleteOrder(Guid orderId)
        {
            var order = _eventHub.GetOrInitialize(orderId);

            var completed = new OrderCompleted
            {
                OrderId = orderId,
                DateTimeStamp_utc = DateTime.UtcNow
            };

            _eventHub.AddEvent(orderId, completed);

            return Task.CompletedTask;
        }

        private int NextOrderIndex(IEnumerable<object> order)
        {
            var last = order.OfType<SubOrderItemAdded>()?.OrderByDescending(ia => ia.OrderIndex).FirstOrDefault()?.OrderIndex;

            var next = last.HasValue ? last + 1 : 0;

            return next.Value;
        }
    }
}
