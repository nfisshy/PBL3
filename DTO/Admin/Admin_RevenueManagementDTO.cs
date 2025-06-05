using System;
using System.Collections.Generic;
using PBL3.Enums;

namespace PBL3.DTO.Admin
{
    public class Admin_RevenueManagementDTO
    {
        public required int Id { get; set; }
        public required string BuyerName { get; set; }
        public required string SellerName { get; set; }
        public required decimal TotalAmount { get; set; }
        public required decimal Discount { get; set; }
        public required decimal Revenue { get; set; }
        public required DateTime OrderDate { get; set; }
        public required OrdStatus OrderStatus { get; set; }
        public required bool PaymentStatus { get; set; }
    }

    public class RevenueStatisticsDTO
    {
        public required decimal TotalRevenue { get; set; }
        public required int TotalOrders { get; set; }
        public required int CompletedOrders { get; set; }
        public required decimal AverageOrderValue { get; set; }
    }
}
