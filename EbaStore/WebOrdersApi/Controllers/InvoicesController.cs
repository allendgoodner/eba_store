using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbaLibrary;
using EbaLibrary.Events;
using EbaLibrary.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebOrdersApi.DTOs;

namespace WebOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private EventHub _eventHub;

        public InvoicesController(EventHub eventHub)
        {
            _eventHub = eventHub;
        }

        [HttpPost("CreateReceivableInvoice")]
        public Task CreateInvoice([FromBody]InvoiceInitializer invoice)
        {
            var aggregate = _eventHub.GetOrInitialize(invoice.AggregateId);
            var activated = InvoiceType.TypeSelector.Activate("Receivable", new object[] { invoice.AggregateId, invoice.Amount, invoice.OrderDate }) as ReceivableInvoice;

            var created = new InvoiceCreated
            {
                InvoiceId = invoice.AggregateId,
                Invoice = activated,
                DateTimeStamp_utc = DateTime.UtcNow
            };

            _eventHub.AddEvent(invoice.AggregateId, created);

            return Task.CompletedTask;
        }

        [HttpPatch("ProcessPayment")]
        public Task ProcessPayment([FromBody]CustomerPayment payment)
        {
            var invoice = _eventHub.GetOrInitialize(payment.AggregateId);
            var paymentEvent = new InvoicePaid
            {
                InvoiceId = payment.AggregateId,
                PaymentDetail = payment.Payment,
                DateTimeStamp_utc = DateTime.UtcNow
            };

            _eventHub.AddEvent(payment.AggregateId, paymentEvent);

            return Task.CompletedTask;
        }
    }
}
