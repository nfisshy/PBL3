using System;
using System.CodeDom;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3.Entity
{
    public class AddressBuyer
    {
        private int addressId;
        private string location;
        private int buyerId;

        private bool isDefault;

        [Key]
        public int AddressId
        {
            get { return addressId; }
            set { addressId = value; }
        }
        public string Location
        {
            get { return location; }
            set { location = value; }
        }
        
        public bool IsDefault{
            get {return isDefault;}
            set {isDefault = value;}
        }
        
        [ForeignKey("Buyer")]
        public int BuyerId
        {
            get { return buyerId; }
            set { buyerId = value; }
        }

        public Buyer Buyer { get; set; }  // dai dien cho mqh nhieu addressBuyer thuoc ve 1 buyer
    }
}