using PBL3.Enums;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Helpers
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this TypeProduct type)
        {
            return type switch
            {
                TypeProduct.Thoitrangnu => "Thời trang nữ",
                TypeProduct.Thoitrangnam => "Thời trang nam",
                TypeProduct.Giaydep => "Giày dép",
                TypeProduct.Tuithoitrangnu => "Túi thời trang nữ",
                TypeProduct.Tuithoitrangnam => "Túi thời trang nam",
                TypeProduct.BaloVali => "Balo & Vali",
                TypeProduct.Donghotrangsuc => "Đồng hồ & Trang sức",
                TypeProduct.Dochoime_be => "Đồ chơi mẹ & bé",
                TypeProduct.Dienthoai_maytinhbang => "Điện thoại & Máy tính bảng",
                TypeProduct.Lamdep_suckhoe => "Làm đẹp & Sức khỏe",
                TypeProduct.Diengiadung => "Điện gia dụng",
                TypeProduct.Laptop_mayvitinh_linhkien => "Laptop, Máy vi tính & Linh kiện",
                TypeProduct.Oto_xemay_xedap => "Ô tô, Xe máy & Xe đạp",
                TypeProduct.Sachvo => "Sách & Vở",
                TypeProduct.Dientu_dienlanh => "Điện tử & Điện lạnh",
                TypeProduct.Mayanh_mayquayphim => "Máy ảnh & Máy quay phim",
                TypeProduct.Thethao => "Thể thao",
                _ => type.ToString()
            };
        }
    }
} 