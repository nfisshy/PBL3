using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_SignUpDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên cửa hàng")]
        [Display(Name = "Tên cửa hàng")]
        public string StoreName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email liên hệ")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email liên hệ")]
        public string EmailGeneral { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string AddressSeller { get; set; }
    }
    public class Seller_SignUpAdjustDTO
    {
        public string Provine { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        public string DetailAddress { get; set; }
    }
}
