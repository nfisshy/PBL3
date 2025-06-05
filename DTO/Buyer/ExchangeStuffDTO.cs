using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class ExchangeStuffDTO
    {
        public int ReturnExchangeId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public byte[]? ProductImage { get; set; }
        public int OrderId { get; set; }
        public int BuyerId { get; set; }

        public int SellerId { get; set; } // ID người bán
        public string Reason { get; set; }
        public byte[]? Image { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public int Quantity { get; set; } // số lượng sản phẩm yêu cầu đổi trả
        public ExchangeStatus Status { get; set; } // true: đã xử lý, false: chưa xử lý
        public string SellerStoreName { get; set; } // Tên người bán
        public string BuyerName { get; set; } // Tên người mua
        public IFormFile? ImageFile { get; set; }
        public string SellerEmail { get; set; } // Email người bán
        public string SellerPhone { get; set; } // Số điện thoại người bán

        }
}