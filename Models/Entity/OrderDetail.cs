using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class OrderDetail
    {
        private int orderId;
        private int productId;

        private string productName;
        private int quantity;
        private decimal price;

        private byte[]? image;

        [Key,Column(Order =0)]
        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
        [Key,Column(Order =1)]
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
        public string Productname
        {
            get { return productName; }
            set { productName = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        public byte[]? Image{
            get {return image; }
            set {image = value; }
        }
        public Order Order { get; set; }
        public Product Product { get;set; }

    }
}