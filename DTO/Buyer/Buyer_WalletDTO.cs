using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_WalletDTO
    {
        public int WalletId { get; set; }
        public decimal WalletBalance { get; set; }
        public int UserId { get; set; }
        public int Pin { get; set; }
        public string BuyerName { get; set; }
        public List<BankDTO> Banks { get; set; }
    }
    public class BankDTO
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; } // Foreign key to Wallet
    }
}