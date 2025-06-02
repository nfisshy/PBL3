using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
        public class Buyer_DanhGiaDTO
        {
            public int ReviewId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public byte[]? ProductImage { get; set; }
            public int BuyerId { get; set; }
            public string BuyerName { get; set; }
            public string Content { get; set; }
            public int Rating { get; set; } 
            public DateTime DateReview { get; set; }
        }
}
