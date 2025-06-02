using System;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Enums
{
    public enum ExchangeStatus
    {
        [Display(Name = "Chờ xác nhận")]
        WaitConfirm,

        [Display(Name = "Đã từ chối")]
        Rejected,

        [Display(Name = "Đã chấp nhận")]
        Approved,
    }
}