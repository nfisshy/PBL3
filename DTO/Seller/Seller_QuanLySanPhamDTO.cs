using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_QuanLySanPhamDTO
    {
        public ProductStatus ProductStatus { get; set; }
        public TypeProduct TypeProduct { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }

    }
    
    public class CreateProductDTO
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public TypeProduct TypeProduct { get; set; }
        public string Description { get; set; }
        public byte[] ProductImage { get; set; }
    }

    public class Seller_ChiTietSanPhamDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public TypeProduct ProductType { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
    }
}
