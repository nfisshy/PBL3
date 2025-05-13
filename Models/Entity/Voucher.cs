using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{ 
    public class Voucher
    {
        private string voucherId;
        private decimal percentDiscount;
        private decimal maxDiscount;
        private string? description;
        private int voucherQuantity;
        private DateTime startDate;
        private DateTime endDate;
        private bool isActive;
        private int sellerId;

        [Key]
        public string VoucherId
        {
            get { return voucherId; }
            set { voucherId = value; }           
        } 
        public decimal PercentDiscount
        {
            get { return percentDiscount; }
            set { percentDiscount = value; }
        }
        public decimal MaxDiscount
        {
            get { return maxDiscount; }
            set { maxDiscount = value; }
        }
        public string? Description
        {
            get { return description; }
            set { description = value; }
        }
        public int VoucherQuantity
        {
            get { return voucherQuantity; }
            set { voucherQuantity = value; }
        }
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        [ForeignKey("Seller")]
        public int SellerId
        {
            get { return sellerId; }
            set { sellerId = value; }
        }

        public Seller Seller { get; set; }
        public ICollection<Voucher_Buyer> Voucher_Buyers { get; set; }
    }
}