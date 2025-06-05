using System;
using PBL3.Enums;

namespace PBL3.DTO.Admin
{    public class Admin_OrderDetailDTO
    {
        public required int OrderId { get; set; }
        public required string BuyerName { get; set; }
        public required string BuyerPhone { get; set; }
        public required string SellerName { get; set; }
        public required string SellerPhone { get; set; }
        public required decimal TotalAmount { get; set; }
        public required DateTime OrderDate { get; set; }
        public required OrdStatus OrderStatus { get; set; }
        public required PayMethod PaymentMethod { get; set; }
        public required bool PaymentStatus { get; set; }
        public required string ShippingAddress { get; set; }
        public required List<OrderItemDetail> OrderItems { get; set; }
    }

    public class OrderItemDetail
    {
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductImage { get; set; }
        public required decimal Price { get; set; }
        public required int Quantity { get; set; }
        public required decimal Subtotal { get; set; }
    }
}