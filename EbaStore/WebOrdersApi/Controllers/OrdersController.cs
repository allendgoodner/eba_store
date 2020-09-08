using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EbaLibrary;
using EbaLibrary.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PubSub;
using WebOrdersApi.BusinessObjects;
using WebOrdersApi.DTOs;

namespace WebOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IOrderProcessor _processor;
        private Hub _hub;

        public OrdersController(IOrderProcessor processor, Hub hub)
        {
            _processor = processor;
            _hub = hub;
        }

        private void EnableSubscriptions()
        {
            
        }

        [HttpPost("StartOrder")]
        public async Task<Guid> InitializeOrder()
        {
            var result = await _processor.InitializeOrder();

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return result;
        }        

        [HttpPost("AddItem")]
        public async Task<int> AddItemToOrder([FromBody]OrderUpdate update)
        {
            var validator = new OrderUpdateValidator();
            var @try = validator.Validate(update);

            if (@try.IsValid)
            {
                var updated = await _processor.AddItemToOrder(update);

                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return updated;
            }

            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await HttpContext.Response.WriteAsync(string.Join(", ", @try.Errors.Select(e => e.ErrorMessage)));

            return -1;
        }

        [HttpPost("RemoveItem")]
        public async Task<int> RemoveItemFromOrder([FromBody]OrderUpdate update)
        {
            var validator = new OrderUpdateValidator();
            var @try = validator.Validate(update);

            if (@try.IsValid)
            {
                var updated = await _processor.RemoveItemFromOrder(update);

                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return updated;
            }

            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await HttpContext.Response.WriteAsync(string.Join(", ", @try.Errors.Select(e => e.ErrorMessage)));

            return -1;
        }

        [HttpPost("SubmitOrder")]
        public async Task<string> SubmitOrder([FromQuery]Guid orderId)
        {
            try
            {
               var result = await _processor.SubmitOrder(orderId);

                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return result;
            }
            catch(Exception e)
            {
                _hub.Publish(e);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return "An error has occurred. Please wait and try your request again. If the error continues, please contact support.";
            }
        }

        public async Task PayAndConfirmOrder(Guid invoiceId, Guid paymentId)
        {
            try
            {
                await _processor.PayAndConfirmOrder(invoiceId, paymentId);

                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                return;
            }
            catch(Exception e)
            {
                _hub.Publish(e);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HttpContext.Response.WriteAsync("An error has occurred. Please wait and try your request again. If the error continues, please contact support.");
            }
        }

        public async Task CompleteOrder(Guid orderId)
        {
            try
            {
                await _processor.CompleteOrder(orderId);

                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                return;
            }
            catch (Exception e)
            {
                _hub.Publish(e);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HttpContext.Response.WriteAsync("An error has occurred. Please wait and try your request again. If the error continues, please contact support.");
            }
        }
    }
}
