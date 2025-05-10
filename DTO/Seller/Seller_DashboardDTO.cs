using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_DashboardDTO
    {
        public List<Seller_TopSanPhamDTO> TopSellingProducts { get; set; }
        public List<Seller_TopSanPhamDTO> TopRatedProducts { get; set; }
        public BusinessMetricsDTO BusinessMetrics { get; set; }
    }

    public class BusinessMetricsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
} 