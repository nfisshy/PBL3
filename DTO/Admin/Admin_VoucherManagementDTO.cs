using System;
using System.ComponentModel.DataAnnotations;

namespace PBL3.DTO.Admin
{
    public class Admin_VoucherManagementDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập mã voucher")]
        public string Code { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm")]
        public decimal DiscountAmount { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập giá trị đơn hàng tối thiểu")]
        public decimal MinimumOrderAmount { get; set; }
    }
}
