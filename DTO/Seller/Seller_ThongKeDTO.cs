using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ThongKeDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }

        public List<Seller_TopSanPhamDTO> TopProductsByQuantity { get; set; }
        public List<Seller_TopSanPhamDTO> TopProductsByRevenue { get; set; }

    }
    public class Seller_TopSanPhamDTO
    {
        public string ProductName { get; set; }
        public int TotalSold { get; set; } // gắn vào Quantity của Product
        public decimal TotalRevenue { get; set; } // gắn vào Price của Product

    }

    public class Seller_TopSanPhamTheoDanhGiaDTO
    {
        public string ProductName { get; set; }
        public int TotalReview { get; set; } // gắn vào Quantity của Product
        public decimal AverageRating { get; set; } // gắn vào Price của Product

    }
}
