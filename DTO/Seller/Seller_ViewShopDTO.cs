using System;
using System.Collections.Generic;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ViewShopDTO
    {
        public string StoreName { get; set; }
        public string EmailGeneral { get; set; }
        public string AddressSeller { get; set; }
        public byte[] Avatar { get; set; }
        public int TotalProducts { get; set; }
        public List<Seller_ViewShopProductDTO> Products { get; set; }
    }

    public class Seller_ViewShopProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public double Rating { get; set; }
        public int SoldQuantity { get; set; }
    }
} 