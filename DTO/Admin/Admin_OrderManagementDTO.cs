using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Enums;

namespace PBL3.DTO.Admin
{
    public class Admin_OrderManagementDTO
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string BuyerName { get; set; }
        public string SellerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public bool PaymentStatus { get; set; }
        public OrdStatus OrderStatus { get; set; }
        public PayMethod PaymentMethod { get; set; }
        public string Address { get; set; }
        public int QuantityTypeOfProduct { get; set; }
    }
}
