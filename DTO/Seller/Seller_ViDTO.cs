using System;
using PBL3.Entity;
using PBL3.Enums;
using System.ComponentModel.DataAnnotations;


namespace PBL3.DTO.Seller
{
    public class Seller_ViDTO
    {
        public decimal WalletBalance { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }

    }

    public class Seller_RutNapTienDTO
    {
        public decimal AmountMoney { get; set; }
        public decimal WalletBalance { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public int Pin { get; set; }
    }

    public class Seller_LienKetNganHangDTO
    {

        public int BankAccountId { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public int Pin { get; set; }
    }
    
    public class Seller_TaoPinDTO
    {
        public bool HasPin { get; set; }
        public int CurrentPin { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã PIN mới")]
        [Range(100000, 999999, ErrorMessage = "Mã PIN phải gồm đúng 6 chữ số")]
        public int NewPin { get; set; }
    }
}
