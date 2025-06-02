using System;
using System.Collections.Generic;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ViewShopDTO
    {
        public string StoreName { get; set; }
        public string EmailGeneral { get; set; }
        public string AddressSeller { get; set; }
        public byte[] Avatar { get; set; }
        public int TotalProducts { get; set; }
        public List<Seller_ViewShopProductDTO> Products { get; set; }
        public List<Seller_ViewShopVoucherDTO> Vouchers { get; set; }
    }

    public class Seller_ViewShopProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public double Rating { get; set; }
        public int SoldQuantity { get; set; }
    }

    public class Seller_ViewShopVoucherDTO
    {
        public string VoucherId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercentage { get; set; } // Phần trăm giảm giá
        public decimal MaxDiscount { get; set; }
        public decimal DiscountAmount { get; set; } // Số tiền giảm
        public bool IsActive { get; set; } // Trạng thái hoạt động của voucher
        public int SellerId { get; set; } // ID của người bán sở hữu voucher
    }
} 