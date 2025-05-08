using System;
using System.Collections.Generic;

namespace PBL3.Entity
{
    public class Buyer : User
    {
        private byte[]? avatar;
        private string location;
        private bool isActive;

        public byte[]? Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }
        public string Location
        {
            get { return location; }
            set { location = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public ICollection<AddressBuyer> Addresses { get; set; } // thiet lap mqh 2 chieu giua address va buyer
        public ICollection<CartItem> CartItems { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Voucher_Buyer> Voucher_Buyers { get; set; }
    }
}