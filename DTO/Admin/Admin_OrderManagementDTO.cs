using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Enums;

namespace PBL3.DTO.Admin
{    public class Admin_OrderManagementDTO
    {
        public required int Id { get; set; }
        public required string BuyerName { get; set; }
        public required string SellerName { get; set; }
        public required decimal TotalAmount { get; set; }
        public required DateTime OrderDate { get; set; }
        public required bool PaymentStatus { get; set; }
        public required OrdStatus OrderStatus { get; set; }
        public required PayMethod PaymentMethod { get; set; }
        public required string Address { get; set; }
        public required int QuantityTypeOfProduct { get; set; }
    }
}
