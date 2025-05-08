using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class Voucher_Buyer
    {
        private string voucherId;
        private int buyerId;
        private bool isUsed;

        [Key,Column(Order = 0)]
        public string VoucherId
        {
            get { return voucherId; }
            set { voucherId = value; }
        }
        [Key,Column(Order = 1)]
        public int BuyerId
        {
            get { return buyerId; }
            set { buyerId = value; }
        }
        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }

        public Voucher Voucher {  get; set; }
        public Buyer Buyer { get; set; }
    }
}