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
        public DateTime RequestDate { get; set; }
        public ExchangeStatus Status { get; set; }
    }
    public class Seller_ReturnExchangeDetailDTO
    {
        public int ReturnExchangeId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public byte[]? Image { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; } // Có thể null nếu chưa phản hồi
        public ExchangeStatus Status { get; set; }
        public int Quantity { get; set; }

    }
}