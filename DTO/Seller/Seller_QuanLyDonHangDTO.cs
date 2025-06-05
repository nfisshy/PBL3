using System;
using PBL3.Entity;
using PBL3.Enums;
using System.Collections.Generic;

namespace PBL3.DTO.Seller
{
    public class Seller_QuanLyDonHangDTO
    {
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Seller_DanhSachDonHangDTO> ListOrder { get; set; }

    }
    public class Seller_DanhSachDonHangDTO
    {
        public int OrderId { get; set; }
        public string BuyerName { get; set; }
        public OrdStatus OrderStatus { get; set; }
    }
    public class Seller_ChiTietDonHangDTO
    {
        public int OrderId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice => OrderPrice - Discount - (OrderPrice * 0.05m);
        public OrdStatus OrderStatus { get; set; }
        public PayMethod PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; }
        public bool CanUpdateToPending { get; set; }
        public bool CanUpdateToDelivering { get; set; }
        public List<Seller_ChiTietDonHangItemDTO> OrderItems { get; set; }
    }

    public class Seller_ChiTietDonHangItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public byte[]? Image { get; set; }
    }
}
