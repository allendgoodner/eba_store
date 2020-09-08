using System;
using System.Collections.Generic;
using System.Text;

namespace EbaLibrary.Payments
{
    public class CardDetails
    {
        public CardVendors Vendor { get; set; }
        public string CardNumber { get; set; }
        public short ExpirationMonth { get; set; }
        public short ExpirationYear { get; set; }
        public string SecurityCode { get; set; }
    }

    public enum CardVendors
    {
        AmericanExpress,
        Discover,
        MasterCard,
        Visa
    }
}
