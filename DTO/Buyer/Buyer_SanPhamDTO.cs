using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_SanPhamDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public double Rating { get; set; } // danh gia may sao
        public ProductStatus Status { get; set; } // trang thai san pham
    }
    public class Buyer_SanPhamTheoDanhMucDTO
    {
        public List<Buyer_SanPhamDTO> ListProduct { get; set; }
        public TypeProduct TypeChosen { get; set; }
    }
    public class Buyer_ChiTietSanPhamDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int Quantity { get; set; }  

        public int AddQuantity {get; set; }
        public double Rating { get; set; } 
        public int sellerId {get; set;}
        public string StoreName { get; set; }
        public byte[] StoreAvatar { get; set; }

        public List<Buyer_DanhGiaDTO> Comments { get; set; }
    }
}
