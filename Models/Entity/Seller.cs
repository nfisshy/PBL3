using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class Seller : User
    {
        private byte[]? avatar;
        private string storeName;
        private DateTime joinedDate;
        private string emailGeneral;
        private string addressSeller;

        public byte[]? Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }
        public string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }
        public DateTime JoinedDate
        {
            get { return joinedDate; }
            set { joinedDate = value; }
        }
        public string EmailGeneral
        {
            get { return emailGeneral; }
            set { emailGeneral = value; }
        }
        public string AddressSeller
        {
            get { return addressSeller; }
            set { addressSeller = value; }
        }

        public ICollection<Product> Products { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Voucher> Vouchers { get; set; }
    }
}