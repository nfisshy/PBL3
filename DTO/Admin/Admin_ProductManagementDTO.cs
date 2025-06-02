using System;
using System.ComponentModel.DataAnnotations;

namespace PBL3.DTO.Admin
{
    public class Admin_ProductManagementDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public int TotalSold { get; set; }
        public byte[]? Image { get; set; }
    }
}
