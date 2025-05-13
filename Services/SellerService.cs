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
        private readonly IBuyerRepositories _buyerRepository;

        public SellerService(
            ISellerRepositories sellerRepository,
            IProductRepositories productRepository,
            IOrderRepositories orderRepository,
            IReviewRepositories reviewRepository,
            IBuyerRepositories buyerRepository)
        {
            _sellerRepository = sellerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _buyerRepository = buyerRepository;
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

                // Lấy danh sách đánh giá
                var reviews = _reviewRepository.GetByProductId(productId);
                var reviewDTOs = reviews?.Select(r => {
                    var buyer = _buyerRepository.GetById(r.BuyerId);
                    return new Seller_DanhGiaDTO
                    {
                        ProductId = r.ProductId,
                        ProductName = product.ProductName,
                        ReviewId = r.ReviewId,
                        Comment = r.Comment,
                        BuyerName = buyer != null ? buyer.Name : $"Người dùng {r.BuyerId}", // nếu người dùng không tồn tại thì sẽ tạo 1 chuỗi tên mặc định 
                        Rating = r.Rating,
                        DateReview = r.DateReview
                    };
                }).ToList() ?? new List<Seller_DanhGiaDTO>();

                // Tính điểm đánh giá trung bình
                double averageRating = reviews?.Any() == true ? reviews.Average(r => r.Rating) : 0;

                // Sử dụng ProductQuantity thay vì GetSoldQuantity
                int soldQuantity = product.ProductQuantity;

                return new Seller_ChiTietSanPhamDTO
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ProductType = product.ProductType,
                    Description = product.ProductDescription,
                    Image = product.ProductImage,
                    Rating = averageRating,
                    SoldQuantity = soldQuantity,
                    Comments = reviewDTOs
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

        // Add notification methods
        public List<Seller_ThongBaoDTO> GetNewOrders(int sellerId)
        {
            var orders = _orderRepository.GetBySellerId(sellerId)
                .Where(o => o.OrderStatus == OrdStatus.WaitConfirm)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new Seller_ThongBaoDTO
                {
                    OrderId = o.OrderId,
                    BuyerName = o.Buyer != null ? o.Buyer.Name : "Khách hàng",
                    TotalProductTypes = o.OrderDetails.Count, // Số lượng loại sản phẩm
                    TotalPrice = o.OrderPrice - o.OriginalPrice * (decimal)0.05, // Giá sau khi trừ phí
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus // Thêm trạng thái đơn hàng
                })
                .ToList();

            return orders;
        }

        public int GetNewOrdersCount(int sellerId)
        {
            return _orderRepository.GetBySellerId(sellerId)
                .Count(o => o.OrderStatus == OrdStatus.WaitConfirm);
        }

        public Seller_ThongKeDTO GetStatistics(int sellerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get all orders for the seller in the date range
                var orders = _orderRepository.GetBySellerId(sellerId)
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.OrderStatus == OrdStatus.Completed)
                    .ToList();

                // Calculate total revenue and orders
                var totalRevenue = orders.Sum(o => o.OrderPrice - o.OriginalPrice * (decimal)0.05); // Trừ 5% phí platform
                var totalOrders = orders.Count;
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                // Get top products by quantity
                var topProductsByQuantity = orders
                    .SelectMany(o => o.OrderDetails)
                    .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                    .Select(g => new Seller_TopSanPhamDTO
                    {
                        ProductName = g.Key.ProductName,
                        TotalSold = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.TotalNetProfit)
                    })
                    .OrderByDescending(p => p.TotalSold)
                    .Take(5)
                    .ToList();

                // Get top products by revenue
                var topProductsByRevenue = orders
                    .SelectMany(o => o.OrderDetails)
                    .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                    .Select(g => new Seller_TopSanPhamDTO
                    {
                        ProductName = g.Key.ProductName,
                        TotalSold = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.TotalNetProfit)
                    })
                    .OrderByDescending(p => p.TotalRevenue)
                    .Take(5)
                    .ToList();

                return new Seller_ThongKeDTO
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalRevenue = totalRevenue,
                    TotalOrders = totalOrders,
                    AverageOrderValue = averageOrderValue,
                    TopProductsByQuantity = topProductsByQuantity,
                    TopProductsByRevenue = topProductsByRevenue
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu thống kê: " + ex.Message, ex);
            }
        }

        public List<Seller_DanhGiaDTO> GetProductReviews(int sellerId, int? productId = null)
        {
            try
            {
                // Lấy tất cả sản phẩm của seller
                var products = _productRepository.GetBySellerId(sellerId);
                if (productId.HasValue)
                {
                    products = products.Where(p => p.ProductId == productId.Value);
                }

                // Lấy tất cả đánh giá của các sản phẩm
                var reviews = new List<Seller_DanhGiaDTO>();
                foreach (var product in products)
                {
                    var productReviews = _reviewRepository.GetByProductId(product.ProductId)
                        .Select(r => {
                            var buyer = _buyerRepository.GetById(r.BuyerId);
                            return new Seller_DanhGiaDTO
                            {
                                ProductId = product.ProductId,
                                ProductName = product.ProductName,
                                ReviewId = r.ReviewId,
                                Comment = r.Comment,
                                BuyerName = buyer != null ? buyer.Name : $"Người dùng {r.BuyerId}",
                                Rating = r.Rating,
                                DateReview = r.DateReview,
                                ProductImage = product.ProductImage
                            };
                        });
                    reviews.AddRange(productReviews);
                }

                // Sắp xếp theo ngày đánh giá mới nhất
                return reviews.OrderByDescending(r => r.DateReview).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách đánh giá: " + ex.Message, ex);
            }
        }
    }
} 