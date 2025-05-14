using System;
using PBL3.Entity;
using PBL3.Enums;

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
        public int OTP { get; set; }
    }
    
    public class Seller_LienKetNganHangDTO
    {

        public int BankAccountId { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public int OTP { get; set; }
    }
}
