using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Shared
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string Account { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
    }
}
