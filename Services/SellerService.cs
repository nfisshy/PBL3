using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.DTO.Seller;
using PBL3.Repositories;
using PBL3.Enums;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PBL3.Services
{
    public class SellerService
    {
        private readonly ISellerRepositories _sellerRepository;
        private readonly IProductRepositories _productRepository;
        private readonly IOrderRepositories _orderRepository;
        private readonly IReviewRepositories _reviewRepository;

        public SellerService(
            ISellerRepositories sellerRepository,
            IProductRepositories productRepository,
            IOrderRepositories orderRepository,
            IReviewRepositories reviewRepository)
        {
            _sellerRepository = sellerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
        }

        public bool IsSellerProfileComplete(int sellerId)
        {
            var seller = _sellerRepository.GetById(sellerId);
            return seller != null && 
                   !string.IsNullOrEmpty(seller.StoreName) && 
                   !string.IsNullOrEmpty(seller.EmailGeneral) && 
                   !string.IsNullOrEmpty(seller.AddressSeller);
        }

        public Seller_SignUpDTO GetSellerProfile(int sellerId)
        {
            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                return null;

            return new Seller_SignUpDTO
            {
                StoreName = seller.StoreName,
                EmailGeneral = seller.EmailGeneral,
                AddressSeller = seller.AddressSeller
            };
        }

        public Seller_SignUpAdjustDTO GetSellerAddress(int sellerId, string tempAddress = null)
        {
            // If temporary address exists, use it
            if (!string.IsNullOrEmpty(tempAddress))
            {
                var address = tempAddress.Split(',', StringSplitOptions.RemoveEmptyEntries);
                return new Seller_SignUpAdjustDTO
                {
                    DetailAddress = address.Length > 0 ? address[0].Trim() : "",
                    Commune = address.Length > 1 ? address[1].Trim() : "",
                    District = address.Length > 2 ? address[2].Trim() : "",
                    Provine = address.Length > 3 ? address[3].Trim() : ""
                };
            }

            // Otherwise get from database
            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                return null;

            // Tách địa chỉ thành các thành phần
            var addressParts = seller.AddressSeller?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return new Seller_SignUpAdjustDTO
            {
                DetailAddress = addressParts?.Length > 0 ? addressParts[0].Trim() : "",
                Commune = addressParts?.Length > 1 ? addressParts[1].Trim() : "",
                District = addressParts?.Length > 2 ? addressParts[2].Trim() : "",
                Provine = addressParts?.Length > 3 ? addressParts[3].Trim() : ""
            };
        }

        public void UpdateSellerAddress(int sellerId, Seller_SignUpAdjustDTO model)
        {
            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new Exception("Không tìm thấy thông tin người bán");

            // Gộp các thành phần địa chỉ thành một chuỗi
            var fullAddress = $"{model.DetailAddress}, {model.Commune}, {model.District}, {model.Provine}";
            seller.AddressSeller = fullAddress;

            _sellerRepository.Update(seller);
        }

        public void UpdateSellerProfile(int sellerId, Seller_SignUpDTO model)
        {
            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new Exception("Không tìm thấy thông tin người bán");

            // Cập nhật thông tin cơ bản
            seller.StoreName = model.StoreName;
            seller.EmailGeneral = model.EmailGeneral;
            seller.AddressSeller = model.AddressSeller;
            seller.JoinedDate = DateTime.Now;

            _sellerRepository.Update(seller);
        }

        public Seller_DashboardDTO GetDashboardData(int sellerId)
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddDays(-7);

                // Get top selling products
                var topSellingProducts = _orderRepository.GetTopSellingProducts(sellerId, startDate, endDate, 3)
                    .Select(p => new Seller_TopSanPhamDTO
                    {
                        ProductName = p.ProductName,
                        TotalSold = p.TotalSold,
                        TotalRevenue = p.TotalRevenue
                    }).ToList();

                // Get top rated products
                var topRatedProducts = _reviewRepository.GetTopRatedProducts(sellerId, 3)
                    .Select(p => new Seller_TopSanPhamDTO
                    {
                        ProductName = p.ProductName,
                        TotalSold = p.TotalSold,
                        TotalRevenue = p.TotalRevenue
                    }).ToList();

                // Get business metrics
                var businessMetrics = new BusinessMetricsDTO
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalRevenue = _orderRepository.GetTotalRevenue(sellerId, startDate, endDate),
                    TotalOrders = _orderRepository.GetTotalOrders(sellerId, startDate, endDate)
                };

                return new Seller_DashboardDTO
                {
                    TopSellingProducts = topSellingProducts,
                    TopRatedProducts = topRatedProducts,
                    BusinessMetrics = businessMetrics
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu dashboard: " + ex.Message, ex);
            }
        }

        public Seller_QuanLyDonHangDTO GetOrderManagement(int sellerId, DateTime? startDate, DateTime? endDate, OrdStatus? status, int? orderId)
        {
            var orders = _orderRepository.GetBySellerId(sellerId);
            if (startDate.HasValue)
                orders = orders.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                orders = orders.Where(o => o.OrderDate <= endDate.Value);
            if (status.HasValue)
                orders = orders.Where(o => o.OrderStatus == status.Value);
            if (orderId.HasValue)
                orders = orders.Where(o => o.OrderId == orderId.Value);

            var listOrder = orders.Select(o => new Seller_DanhSachDonHangDTO
            {
                OrderId = o.OrderId,
                BuyerName = o.Buyer != null ? o.Buyer.Name : o.BuyerId.ToString(),
                OrderStatus = o.OrderStatus
            }).ToList();

            return new Seller_QuanLyDonHangDTO
            {
                StartDate = startDate ?? DateTime.Now.AddDays(-30),
                EndDate = endDate ?? DateTime.Now,
                ListOrder = listOrder
            };
        }

        public List<Seller_QuanLySanPhamDTO> GetProductList(int sellerId)
        {
            try
            {
                var products = _productRepository.GetBySellerId(sellerId);
                if (products == null || !products.Any())
                {
                    return new List<Seller_QuanLySanPhamDTO>();
                }

                return products.Select(p => new Seller_QuanLySanPhamDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Profit = p.Price * 0.95m, // Assuming 5% commission
                    TypeProduct = p.ProductType,
                    ProductStatus = p.ProductStatus
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sản phẩm: " + ex.Message, ex);
            }
        }

        public Seller_ChiTietSanPhamDTO GetProductDetail(int sellerId, int productId)
        {
            try
            {
                var product = _productRepository.GetById(productId);
                if (product == null || product.SellerId != sellerId)
                {
                    return null;
                }

                return new Seller_ChiTietSanPhamDTO
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ProductType = product.ProductType,
                    Description = product.ProductDescription,
                    Image = product.ProductImage
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết sản phẩm ID {productId}: " + ex.Message, ex);
            }
        }

        public void CreateProduct(int sellerId, CreateProductDTO model)
        {
            try
            {
                // Kiểm tra người bán tồn tại
                var seller = _sellerRepository.GetById(sellerId);
                if (seller == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy thông tin người bán");
                }

                // Tạo sản phẩm mới
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Price = model.Price,
                    ProductType = model.TypeProduct,
                    ProductDescription = model.Description,
                    ProductImage = model.ProductImage,
                    SellerId = sellerId,
                    ProductStatus = ProductStatus.Selling,
                    ProductQuantity = 0, // Số lượng ban đầu là 0
                };

                _productRepository.Add(product);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo sản phẩm mới: " + ex.Message, ex);
            }
        }
    }
} 