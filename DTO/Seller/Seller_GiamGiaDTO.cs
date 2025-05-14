using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_DanhSachGiamGiaDTO
    {
        public string VoucherId { get; set; }
        public int PercentDiscount { get; set; }
        public int MaxDiscount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
    public class Seller_TaoGiamGiaDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập mã voucher")]
        [Display(Name = "Mã voucher")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Mã voucher phải từ 3-20 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã voucher chỉ được chứa chữ cái và số")]
        public string VoucherId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm giá")]
        [Display(Name = "Phần trăm giảm giá")]
        [Range(1, 100, ErrorMessage = "Phần trăm giảm giá phải từ 1% đến 100%")]
        public int PercentDiscount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm tối đa")]
        [Display(Name = "Giá trị giảm tối đa")]
        [Range(1000, 1000000000, ErrorMessage = "Giá trị giảm tối đa phải từ 1.000 đến 1.000.000.000 VNĐ")]
        public int MaxDiscount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng voucher")]
        [Display(Name = "Số lượng voucher")]
        [Range(1, 10000, ErrorMessage = "Số lượng voucher phải từ 1 đến 10.000")]
        public int Quantity { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
