using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PBL3.Enums;
using System.Data;

namespace PBL3.Entity
{
    public class Order
    {
        private int orderId;
        private int buyerId;
        private int sellerId;
        private DateTime orderDate;
        private decimal orderPrice; // giá gốc
        private OrdStatus orderStatus;  // trang thai : chua giao , da giao , bi huy , giao hang thanh cong
        private PayMethod paymentMethod; // thanh toan khi nhan hang , thanh toan qua vi
        private bool paymentStatus; //da thanh toan : 1 , chua thanh toan : 0 
        private string address;
        private decimal discount;
        private int quantityTypeOfProduct;
        private DateTime orderReceivedDate; // Ngày nhận hàng, nếu có

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


        public decimal Discount{
            get {return discount; }
            set {discount = value; }
        }

        public int QuantityTypeOfProduct
        {
            get { return quantityTypeOfProduct; }
            set { quantityTypeOfProduct = value; }

        }
        public DateTime OrderReceivedDate
        {
            get { return orderReceivedDate; }
            set { orderReceivedDate = value; }
        }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Buyer Buyer {  get; set; }
        public Seller Seller { get; set; }
    }
}