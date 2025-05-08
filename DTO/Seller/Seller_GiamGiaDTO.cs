using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_DanhSachGiamGiaDTO
    {
        public string VoucherId { get; set; }
        public int PercentDiscount { get; set; }
        public int MaxDiscount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
    public class Seller_TaoGiamGiaDTO
    {
        public string VoucherId { get; set; }
        public int PercentDiscount { get; set; }
        public int MaxDiscount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
    }
}
