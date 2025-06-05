using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_CartItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public string ImageData {get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public bool IsSelected { get; set; }

        public int currentQuantity { get; set; }
        public bool IsActive { get; set; } // Trạng thái sản phẩm có còn bán hay không
    }
    public class Buyer_CartDTO
    {
        public int sellerID {get; set; }
        public string sellerName {get; set; }
        public List<Buyer_CartItemDTO> CartItems { get; set; }
        public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);
    }
}
