using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Buyer
{
    public class Buyer_SoDiaChiDTO
    {
        public int AddressId { get; set; }
        public string Street { get; set; }    // Số nhà, tên đường
        public string Ward { get; set; }       // Phường/Xã
        public string District { get; set; }   // Quận/Huyện
        public string City { get; set; }        // Tỉnh/Thành phố
        public bool IsDefault { get; set; }

    }
}
