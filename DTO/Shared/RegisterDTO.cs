using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Shared
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string Account { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string RePassword {  get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public Gender Sex { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public Roles RoleName { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(RegisterDTO), nameof(ValidateBirthDate))]
        public DateTime? Date { get; set; }

        public static ValidationResult ValidateBirthDate(DateTime? date, ValidationContext context)
        {
            if (!date.HasValue)
                return new ValidationResult("Vui lòng chọn ngày sinh");
            if (date.Value > DateTime.Now.Date)
                return new ValidationResult("Ngày sinh không hợp lệ");
            return ValidationResult.Success;
        }
    }
}
