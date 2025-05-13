using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ThongBaoDTO
    {
        public int OrderId { get; set; }
        public string BuyerName { get; set; }
        public int TotalProductTypes { get; set; } // Số lượng loại sản phẩm
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public OrdStatus OrderStatus { get; set; } // Thêm trạng thái đơn hàng
    }
}
