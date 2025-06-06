using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ReturnExchangeDTO
    {
        public int ReturnExchangeId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public byte[]? Image { get; set; }
        public DateTime RequestDate { get; set; }
        public ExchangeStatus Status { get; set; }
    }

}