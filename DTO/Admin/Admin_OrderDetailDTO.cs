using System;
using PBL3.Enums;

namespace PBL3.DTO.Admin
{
    public class Admin_OrderDetailDTO
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string SellerName { get; set; }
        public string SellerPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public OrdStatus OrderStatus { get; set; }
        public PayMethod PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDetail> OrderItems { get; set; }
    }

    public class OrderItemDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
} 