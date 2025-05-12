using System;
using PBL3.Entity;
using PBL3.Enums;

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
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        
    }
}
