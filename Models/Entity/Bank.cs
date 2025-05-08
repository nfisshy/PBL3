using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class Bank
    {
        private int bankAccountId;
        private string bankNumber;
        private string bankName;
        private int walletId;

        [Key]
        public int BankAccountId
        {
            get { return bankAccountId; }
            set { bankAccountId = value; }
        }
        public string BankNumber
        {
            get { return bankNumber; }
            set { bankNumber = value; }
        }
        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }
        [ForeignKey("Wallet")]
        public int WalletId
        {
            get { return walletId; }
            set { walletId = value; }
        }
        public PlatformWallet Wallet {  get; set; }
    }
}