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
        public int Quantity { get; set; }  // so luong mua hoac them vao gio hang
        public double Rating { get; set; } 
        public string StoreName { get; set; }
        public byte[] ImageStore { get; set; }
    }
}
