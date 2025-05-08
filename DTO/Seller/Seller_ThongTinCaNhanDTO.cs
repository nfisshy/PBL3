﻿using System;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_ThongTinCaNhanDTO
    {
        public string UserName { get; set; }
        public Gender Sex { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressSeller { get; set; }
        public byte[] Avatar { get; set; }
    }
}
