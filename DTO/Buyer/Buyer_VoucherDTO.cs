using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_VoucherDTO
    {
        public string VoucherId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercentage { get; set; } // Phần trăm giảm giá
        public decimal MaxDiscount { get; set; } 
        public decimal DiscountAmount { get; set; } // Số tiền giảm
        public bool IsActive { get; set; } // Trạng thái hoạt động của voucher
        public int BuyerId { get; set; } // ID của người mua sở hữu voucher
    }
}