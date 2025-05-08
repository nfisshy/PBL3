using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class CartItem
    {
        private int buyerId;
        private int productId;
        private int quantity;
        private byte[]? productImage;
        private string productName;
        
        [Key,Column(Order = 0)]
        public int BuyerId
        {
            get { return buyerId; }
            set { buyerId = value; }
        }

        [Key,Column(Order = 1)]
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public string ProductName 
        {
            get { return productName; }
            set { productName = value; }
        }
        public byte[]? ProductImage
        {
            get { return productImage; }
            set { productImage = value; }
        }

        public Buyer Buyer { get; set; }
        public Product Product { get; set; }
    }
}