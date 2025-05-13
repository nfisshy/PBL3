using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using PBL3.Enums;

namespace PBL3.Entity
{
    public class Product
    {
        private int productId;
        private string productName;
        private int productQuantity; // số lượng sản phẩm bán được
        private decimal price;
        private byte[]? productImage;
        private TypeProduct productType;
        private string? productDescription;
        private int sellerId;
        private ProductStatus productStatus;
        private int soldProduct;

        [Key]
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }
        public int ProductQuantity
        {
            get { return productQuantity; }
            set { productQuantity = value; }
        }
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        public byte[]? ProductImage
        {
            get { return productImage; }
            set { productImage = value; }
        }
        public TypeProduct ProductType
        {
            get { return productType; }
            set { productType = value; }
        }
        public string? ProductDescription
        {
            get { return productDescription; }
            set { productDescription = value; }
        }
        [ForeignKey("Seller")]
        public int SellerId
        {
            get { return sellerId; }
            set { sellerId = value; }
        }
        public ProductStatus ProductStatus
        {
            get { return productStatus; }
            set { productStatus = value; }
        }
        public int SoldProduct
        {
            get { return soldProduct; }
            set { soldProduct = value; }
        }
        public Seller Seller { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Review> Reviews { get; set; }  
    }
}