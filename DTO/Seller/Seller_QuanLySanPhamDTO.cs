using System;
using System.ComponentModel.DataAnnotations;
using PBL3.Entity;
using PBL3.Enums;

namespace PBL3.DTO.Seller
{
    public class Seller_QuanLySanPhamDTO
    {
        public ProductStatus ProductStatus { get; set; }
        public TypeProduct TypeProduct { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

    }
    
    public class CreateProductDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên sản phẩm phải từ 5-100 ký tự")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Display(Name = "Giá sản phẩm")]
        [Range(1000, 1000000000, ErrorMessage = "Giá sản phẩm phải từ 1.000 đến 1.000.000.000 VNĐ")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        [Display(Name = "Loại sản phẩm")]
        public TypeProduct TypeProduct { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm")]
        [Display(Name = "Số lượng sản phẩm")]
        [Range(0, 10000, ErrorMessage = "Số lượng sản phẩm phải từ 0 đến 10.000")]
        public int ProductQuantity { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh sản phẩm")]
        [Display(Name = "Hình ảnh sản phẩm")]
        public byte[] ProductImage { get; set; }
    }

    public class Seller_ChiTietSanPhamDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public TypeProduct ProductType { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        
        public double Rating { get; set; }
        public int SoldQuantity { get; set; }
        public List<Seller_DanhGiaDTO> Comments { get; set; }
    }
}
