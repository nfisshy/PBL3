using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PBL3.Enums;

namespace PBL3.Entity
{
    public class Order
    {
        private int orderId;
        private int buyerId;
        private int sellerId;
        private int quantityTypeOfProduct;
        private DateTime orderDate;
        private decimal orderPrice;
        private OrdStatus orderStatus;  // trang thai : chua giao , da giao , bi huy , giao hang thanh cong
        private PayMethod paymentMethod; // thanh toan khi nhan hang , thanh toan qua vi
        private bool paymentStatus; //da thanh toan : 1 , chua thanh toan : 0 
        private string address;

        [Key]
        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
        [ForeignKey("Buyer")]
        public int BuyerId
        {
            get { return buyerId; }
            set { buyerId = value; }
        }
        [ForeignKey("Seller")]
        public int SellerId
        {
            get { return  sellerId; }
            set { sellerId = value; }
        }
        public int QuantityTypeOfProduct
        {
            get { return quantityTypeOfProduct; }
            set { quantityTypeOfProduct = value; }
        }
        public DateTime OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }
        public decimal OrderPrice
        {
            get { return orderPrice; }
            set { orderPrice = value; }
        }
        public OrdStatus OrderStatus
        {
            get { return orderStatus; }
            set { orderStatus = value; }
        }
        public PayMethod PaymentMethod
        {
            get { return paymentMethod; }
            set { paymentMethod = value; }
        }
        public bool PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value; }
        }

        public string Address{
            get { return address; }
            set { address = value; } 
        }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Buyer Buyer {  get; set; }
        public Seller Seller { get; set; }
    }
}