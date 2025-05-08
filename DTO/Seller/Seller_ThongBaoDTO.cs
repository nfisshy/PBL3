using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ThongBaoDTO // chỉ hiện danh sách đơn hàng mới
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string BuyerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}
