using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class PlatformWallet
    {
        private int walletId;
        private decimal walletBalance;
        private int userId;
        private int otp;

        [Key]
        public int WalletId
        {
            get { return walletId; }
            set { walletId = value; }
        }
        public decimal WalletBalance
        {
            get { return walletBalance; }
            set { walletBalance = value; }
        }
        [ForeignKey("User")]
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public int Otp
        {
            get { return otp; }
            set { otp = value; }
        }
        public User User { get; set; }
        public ICollection<Bank> Banks { get; set; }
    }
}