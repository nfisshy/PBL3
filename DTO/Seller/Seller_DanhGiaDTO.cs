using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_DanhGiaDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public string BuyerName { get; set; }
        public int Rating { get; set; }
        public DateTime DateReview { get; set; }

    }
}
