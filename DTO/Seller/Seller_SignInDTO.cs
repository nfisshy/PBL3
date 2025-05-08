using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_SignInDTO
    {
        public int SellerID { get; set; }
        public string StoreName { get; set; }
        public string AddressSeller { get; set; }
        public string EmailGeneral { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class Seller_SignInAdjustDTO
    {
        public int SellerID { get; set; }
        public string Username { get; set; }
        public string Provine { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        public string DetailAddress { get; set; }
    }
}
