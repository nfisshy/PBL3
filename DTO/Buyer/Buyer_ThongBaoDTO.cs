using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_TrangThaiDonHangDTO
    {
        public int OrderId { get; set; }
        public OrdStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class Buyer_ThongBaoVoucherDTO
    {
        public string VoucherId { get; set; }
        public string VoucherName { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}