using System;
using System.Collections.Generic;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderPrice { get; set; }
        public OrdStatus OrderStatus { get; set; }
        public PayMethod PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
        public string BuyerName { get; set; }
        public string SellerName { get; set; }
        public string address {get; set; }
    }

    public class OrderDetailDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 