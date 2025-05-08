using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_ThanhToanDTO
    {
        public List<Buyer_DonHangThanhToanDTO> Items { get; set; }

        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);

        public string VoucherID { get; set; }
        public int PercentDiscount { get; set; }
        public int MaxDiscount { get; set; }
        public decimal TotalPriceAfterDiscount => TotalPrice - Math.Min(TotalPrice * PercentDiscount / 100, MaxDiscount);
        public decimal CommissionRate { get; set; } = 0.05m; // ti le hoa hong
        public decimal CommissionAmount => TotalPriceAfterDiscount * CommissionRate;
        public string Address { get; set; }
    }

    public class Buyer_DonHangThanhToanDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}