using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.DTO.Seller;
using PBL3.Repositories;
using PBL3.Enums;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;

namespace PBL3.Services
{
    public class SellerService
    {
        private readonly ISellerRepositories _sellerRepository;
        private readonly IProductRepositories _productRepository;
        private readonly IOrderRepositories _orderRepository;
        private readonly IReviewRepositories _reviewRepository;
        private readonly IBuyerRepositories _buyerRepository;
        private readonly IVoucherRepositories _voucherRepository;
        private readonly IPlatformWalletRepositories _walletRepository;
        private readonly IBankRepositories _bankRepository;
        private readonly ILogger<SellerService> _logger;

        public SellerService(
            ISellerRepositories sellerRepository,
            IProductRepositories productRepository,
            IOrderRepositories orderRepository,
            IReviewRepositories reviewRepository,
            IBuyerRepositories buyerRepository,
            IVoucherRepositories voucherRepository,
            IPlatformWalletRepositories walletRepository,
            IBankRepositories bankRepository,
            ILogger<SellerService> logger)
        {
            _sellerRepository = sellerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _buyerRepository = buyerRepository;
            _voucherRepository = voucherRepository;
            _walletRepository = walletRepository;
            _bankRepository = bankRepository;
            _logger = logger;
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

                // Lấy thông tin người bán để lấy địa chỉ
                var seller = _sellerRepository.GetById(sellerId);
                if (seller == null)
                {
                    throw new Exception("Không tìm thấy thông tin người bán");
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
                        BuyerName = buyer != null ? buyer.Name : $"Người dùng {r.BuyerId}",
                        Rating = r.Rating,
                        DateReview = r.DateReview
                    };
                }).ToList() ?? new List<Seller_DanhGiaDTO>();

                // Tính điểm đánh giá trung bình
                double averageRating = reviews?.Any() == true ? reviews.Average(r => r.Rating) : 0;

                // Sử dụng ProductQuantity thay vì GetSoldQuantity
                int soldQuantity = product.SoldProduct;

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
                    InitialQuantity = product.ProductQuantity,  // Thêm số lượng ban đầu
                    AddressSeller = seller.AddressSeller,  // Thêm địa chỉ lấy hàng
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
                    ProductQuantity = model.ProductQuantity // Sử dụng số lượng từ model
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
            try 
            {
                var orders = _orderRepository.GetBySellerId(sellerId)
                    .Where(o => o != null && o.OrderStatus == OrdStatus.WaitConfirm)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new Seller_ThongBaoDTO
                    {
                        OrderId = o.OrderId,
                        BuyerName = o.Buyer?.Name ?? "Khách hàng",
                        TotalProductTypes = o.OrderDetails?.Count ?? 0,
                        TotalPrice = o.OrderPrice - (o.OrderPrice * (decimal)0.05) - o.Discount,
                        OrderDate = o.OrderDate,
                        OrderStatus = o.OrderStatus
                    })
                    .ToList();

                return orders;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách đơn hàng mới: " + ex.Message, ex);
            }
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
                var totalRevenue = orders.Sum(o => o.OrderPrice - o.OrderPrice * (decimal)0.05-o.Discount); // Trừ 5% phí platform
                var totalOrders = orders.Count;
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                // Get top products by quantity
                var topProductsByQuantity = orders
                    .SelectMany(o => o.OrderDetails) // trải phẳng các nhóm orderdetail theo tất cả các order
                    .GroupBy(od => new { od.ProductId, od.Product.ProductName }) // nhóm các orderdetail theo productid và productname
                    .Select(g => new Seller_TopSanPhamDTO  // mỗi g đại diện cho 1 group 
                    {
                        ProductName = g.Key.ProductName,  // key này là key của group ở trên 
                        TotalSold = g.Sum(od => od.Quantity), // nhóm tất cả các orderdetail trong group và tính tổng số lượng
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

        public List<Seller_DanhSachGiamGiaDTO> GetVoucherList(int sellerId)
        {
            try
            {
                var vouchers = _voucherRepository.GetBySellerId(sellerId);
                return vouchers.Select(v => new Seller_DanhSachGiamGiaDTO
                {
                    VoucherId = v.VoucherId,
                    PercentDiscount = (int)v.PercentDiscount,
                    MaxDiscount = (int)v.MaxDiscount,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Quantity = v.VoucherQuantity,
                    IsActive = v.IsActive && v.EndDate > DateTime.Now
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách voucher: " + ex.Message, ex);
            }
        }

        public void CreateVoucher(int sellerId, Seller_TaoGiamGiaDTO model)
        {
            try
            {
                // Validate input
                if (model.StartDate >= model.EndDate)
                    throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc");

                if (model.PercentDiscount <= 0 || model.PercentDiscount > 100)
                    throw new ArgumentException("Phần trăm giảm giá phải từ 1% đến 100%");

                if (model.MaxDiscount <= 0)
                    throw new ArgumentException("Giá trị giảm tối đa phải lớn hơn 0");

                if (model.Quantity <= 0)
                    throw new ArgumentException("Số lượng voucher phải lớn hơn 0");

                // Check if voucher ID already exists
                var existingVoucher = _voucherRepository.GetById(model.VoucherId);
                if (existingVoucher != null)
                    throw new ArgumentException("Mã voucher đã tồn tại");

                var voucher = new Voucher
                {
                    VoucherId = model.VoucherId,
                    PercentDiscount = model.PercentDiscount,
                    MaxDiscount = model.MaxDiscount,
                    Description = model.Description,
                    VoucherQuantity = model.Quantity,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsActive = true,
                    SellerId = sellerId
                };

                _voucherRepository.Add(voucher);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo voucher: " + ex.Message, ex);
            }
        }

        public void UpdateVoucherStatus(int sellerId, string voucherId, bool isActive)
        {
            try
            {
                var voucher = _voucherRepository.GetById(voucherId);
                if (voucher == null || voucher.SellerId != sellerId)
                    throw new ArgumentException("Không tìm thấy voucher");

                voucher.IsActive = isActive;
                _voucherRepository.Update(voucher);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật trạng thái voucher: " + ex.Message, ex);
            }
        }

        public void DeleteVoucher(int sellerId, string voucherId)
        {
            try
            {
                var voucher = _voucherRepository.GetById(voucherId);
                if (voucher == null || voucher.SellerId != sellerId)
                    throw new ArgumentException("Không tìm thấy voucher");

                _voucherRepository.Delete(voucherId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa voucher: " + ex.Message, ex);
            }
        }

        public Seller_ChiTietDonHangDTO GetOrderDetail(int sellerId, int orderId)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null || order.SellerId != sellerId)
                {
                    throw new Exception("Không tìm thấy đơn hàng");
                }

                _logger.LogInformation("Getting order details for order {OrderId}. OrderDetails count: {OrderDetailsCount}", 
                    orderId, order.OrderDetails?.Count ?? 0);

                var buyer = _buyerRepository.GetById(order.BuyerId);
                
                // Kiểm tra khả năng cập nhật trạng thái
                bool canUpdateToPending = order.OrderStatus == OrdStatus.WaitConfirm;
                bool canUpdateToDelivering = order.OrderStatus == OrdStatus.Pending;

                var orderItems = order.OrderDetails?.Select(od => {
                    _logger.LogInformation("Processing order detail: ProductId={ProductId}, ProductName={ProductName}, Quantity={Quantity}", 
                        od.ProductId, od.Product?.ProductName, od.Quantity);
                    return new Seller_ChiTietDonHangItemDTO
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product?.ProductName ?? "Unknown Product",
                        Quantity = od.Quantity,
                        Price = od.Product?.Price ?? 0,
                        Image = od.Product?.ProductImage,
                        TotalPrice = (od.Product?.Price ?? 0) * od.Quantity
                    };
                }).ToList() ?? new List<Seller_ChiTietDonHangItemDTO>();

                _logger.LogInformation("Created {OrderItemsCount} order items", orderItems.Count);

                return new Seller_ChiTietDonHangDTO
                {
                    OrderId = order.OrderId,
                    BuyerName = buyer?.Name ?? "Khách hàng",
                    BuyerPhone = buyer?.PhoneNumber ?? "Chưa cập nhật",
                    Address = order.Address,
                    OrderDate = order.OrderDate,
                    OrderPrice = order.OrderPrice,
                    Discount = order.Discount,
                    OrderStatus = order.OrderStatus,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    CanUpdateToPending = canUpdateToPending,
                    CanUpdateToDelivering = canUpdateToDelivering,
                    OrderItems = orderItems
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order detail for order {orderId}");
                throw new Exception($"Lỗi khi lấy chi tiết đơn hàng: {ex.Message}", ex);
            }
        }

        public void UpdateOrderStatus(int sellerId, int orderId, OrdStatus newStatus)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null || order.SellerId != sellerId)
                {
                    throw new Exception("Không tìm thấy đơn hàng");
                }

                // Kiểm tra tính hợp lệ của việc chuyển trạng thái
                if (!IsValidStatusTransition(order.OrderStatus, newStatus))
                {
                    throw new Exception("Không thể chuyển sang trạng thái này");
                }

                order.OrderStatus = newStatus;
                _orderRepository.Update(order);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}", ex);
            }
        }

        private bool IsValidStatusTransition(OrdStatus currentStatus, OrdStatus newStatus)
        {
            // Chỉ cho phép chuyển từ WaitConfirm sang Pending, và từ Pending sang Delivering
            return (currentStatus == OrdStatus.WaitConfirm && newStatus == OrdStatus.Pending) ||
                   (currentStatus == OrdStatus.Pending && newStatus == OrdStatus.Delivering);
        }

        public Seller_ThongTinCaNhanDTO GetSellerPersonalInfo(int sellerId)
        {
            try
            {
                var seller = _sellerRepository.GetById(sellerId);
                if (seller == null)
                {
                    throw new Exception("Không tìm thấy thông tin người bán");
                }

                return new Seller_ThongTinCaNhanDTO
                {
                    FullName = seller.Name,
                    Sex = seller.Sex,
                    Date = seller.Date,
                    PhoneNumber = seller.PhoneNumber,
                    AddressSeller = seller.AddressSeller,
                    Avatar = seller.Avatar,
                    StoreName = seller.StoreName,
                    EmailGeneral = seller.EmailGeneral
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy thông tin cá nhân: " + ex.Message, ex);
            }
        }

        public void UpdateSellerProfile(int sellerId, Seller_ThongTinCaNhanDTO profile, byte[] newAvatar = null)
        {
            try
            {
                var seller = _sellerRepository.GetById(sellerId);
                if (seller == null)
                {
                    throw new Exception("Không tìm thấy thông tin người bán");
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(profile.StoreName))
                    throw new Exception("Tên cửa hàng không được để trống");
                if (string.IsNullOrWhiteSpace(profile.AddressSeller))
                    throw new Exception("Địa chỉ không được để trống");
                if (string.IsNullOrWhiteSpace(profile.PhoneNumber))
                    throw new Exception("Số điện thoại không được để trống");
                if (string.IsNullOrWhiteSpace(profile.EmailGeneral))
                    throw new Exception("Email không được để trống");

                // Validate phone number format (basic validation)
                if (!System.Text.RegularExpressions.Regex.IsMatch(profile.PhoneNumber, @"^[0-9]{10,11}$"))
                    throw new Exception("Số điện thoại không hợp lệ");

                // Validate email format
                if (!System.Text.RegularExpressions.Regex.IsMatch(profile.EmailGeneral, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Email không hợp lệ");

                // Update seller information
                seller.StoreName = profile.StoreName;
                seller.AddressSeller = profile.AddressSeller;
                seller.PhoneNumber = profile.PhoneNumber;
                seller.EmailGeneral = profile.EmailGeneral;
                if (newAvatar != null)
                {
                    seller.Avatar = newAvatar;
                }

                _sellerRepository.Update(seller);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật thông tin cá nhân: " + ex.Message, ex);
            }
        }
        public void DoiMatKhau(int sellerId, Seller_DoiMatKhauDTO model)
        {
            if (sellerId <= 0)
                throw new ArgumentException("ID người bán không hợp lệ", nameof(sellerId));

            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new KeyNotFoundException($"Không tìm thấy người bán với ID: {sellerId}");

            if (seller.Password != model.OldPassword)
                throw new ArgumentException("Mật khẩu cũ không đúng");

            if (model.NewPassword != model.ConfirmPassword)
                throw new ArgumentException("Mật khẩu mới và xác nhận mật khẩu không khớp");

            seller.Password = model.NewPassword;
            _sellerRepository.Update(seller);
        }

        public Seller_ViDTO GetWalletInfo(int sellerId)
        {
            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new KeyNotFoundException($"Không tìm thấy người bán với ID: {sellerId}");

            var wallet = _walletRepository.GetByUserId(sellerId);
            if (wallet == null)
                throw new KeyNotFoundException("Không tìm thấy ví của người bán");

            var bank = _bankRepository.GetByWalletId(wallet.WalletId)?.FirstOrDefault();

            return new Seller_ViDTO
            {
                WalletBalance = wallet.WalletBalance,
                BankName = bank?.BankName ?? "Chưa liên kết",
                BankNumber = bank?.BankNumber ?? "Chưa liên kết"
            };
        }

        public void LinkBankAccount(int sellerId, Seller_LienKetNganHangDTO model)
        {
            if (sellerId <= 0)
                throw new ArgumentException("ID người bán không hợp lệ", nameof(sellerId));

            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new KeyNotFoundException($"Không tìm thấy người bán với ID: {sellerId}");

            var wallet = _walletRepository.GetByUserId(sellerId);
            if (wallet == null)
                throw new KeyNotFoundException("Không tìm thấy ví của người bán");

            // Kiểm tra OTP
            // if (model.OTP != seller.OTP)
            //     throw new ArgumentException("OTP không đúng"); // sửa ........................................

            // Kiểm tra xem đã có tài khoản ngân hàng chưa
            var existingBank = _bankRepository.GetByWalletId(wallet.WalletId)?.FirstOrDefault();
            if (existingBank != null)
            {
                // Cập nhật thông tin ngân hàng
                existingBank.BankName = model.BankName;
                existingBank.BankNumber = model.BankNumber;
                _bankRepository.Update(existingBank);
            }
            else
            {
                // Thêm mới tài khoản ngân hàng
                var bank = new Bank
                {
                    BankName = model.BankName,
                    BankNumber = model.BankNumber,
                    WalletId = wallet.WalletId
                };
                _bankRepository.Add(bank);
            }
        }

        // public void UpdateOTP(int sellerId)
        // {
        //     var seller = _sellerRepository.GetById(sellerId);
        //     if (seller == null)
        //         throw new KeyNotFoundException($"Không tìm thấy người bán với ID: {sellerId}");

        //     // Tạo OTP ngẫu nhiên 6 chữ số
        //     Random random = new Random();
        //     seller.OTP = random.Next(100000, 999999);
        //     _sellerRepository.Update(seller);
        // }

        public void NapTien(int sellerId, Seller_RutNapTienDTO model)
        {
            if (sellerId <= 0)
                throw new ArgumentException("ID người bán không hợp lệ", nameof(sellerId));

            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new KeyNotFoundException($"Không tìm thấy người bán với ID: {sellerId}");

            var wallet = _walletRepository.GetByUserId(sellerId);
            if (wallet == null)
                throw new KeyNotFoundException("Không tìm thấy ví của người bán");

            // Kiểm tra OTP
            // if (model.OTP != seller.OTP)
            //     throw new ArgumentException("OTP không đúng"); // sửa ........................................

            // Kiểm tra số tiền nạp
            if (model.AmountMoney <= 0)
                throw new ArgumentException("Số tiền nạp phải lớn hơn 0");

            // Cập nhật số dư ví
            wallet.WalletBalance += model.AmountMoney;
            _walletRepository.Update(wallet);
        }

        public void RutTien(int sellerId, Seller_RutNapTienDTO model)
        {
            if (sellerId <= 0)
                throw new ArgumentException("ID người bán không hợp lệ", nameof(sellerId));

            var seller = _sellerRepository.GetById(sellerId);
            if (seller == null)
                throw new KeyNotFoundException($"Không tìm thấy người bán với ID: {sellerId}");

            var wallet = _walletRepository.GetByUserId(sellerId);
            if (wallet == null)
                throw new KeyNotFoundException("Không tìm thấy ví của người bán");

            // Kiểm tra OTP
            // if (model.OTP != seller.OTP)
            //     throw new ArgumentException("OTP không đúng"); // sửa ........................................

            // Kiểm tra số tiền rút
            if (model.AmountMoney <= 0)
                throw new ArgumentException("Số tiền rút phải lớn hơn 0");

            // Kiểm tra số dư
            if (wallet.WalletBalance < model.AmountMoney)
                throw new ArgumentException("Số dư không đủ để thực hiện giao dịch");

            // Cập nhật số dư ví
            wallet.WalletBalance -= model.AmountMoney;
            _walletRepository.Update(wallet);
        }

        public Seller_ViewShopDTO ViewShop(int sellerId)
        {
            try
            {
                var seller = _sellerRepository.GetById(sellerId);
                if (seller == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy cửa hàng với ID: {sellerId}");
                }

                // Lấy danh sách sản phẩm của cửa hàng
                var products = _productRepository.GetBySellerId(sellerId);
                var productDTOs = products?.Select(p =>
                {
                    var reviews = _reviewRepository.GetByProductId(p.ProductId);
                    double averageRating = reviews?.Any() == true ? reviews.Average(r => r.Rating) : 0;

                    return new Seller_ViewShopProductDTO
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        Image = p.ProductImage,
                        Rating = averageRating,
                        SoldQuantity = p.SoldProduct
                    };
                }).ToList() ?? new List<Seller_ViewShopProductDTO>();

                return new Seller_ViewShopDTO
                {
                    StoreName = seller.StoreName,
                    EmailGeneral = seller.EmailGeneral,
                    AddressSeller = seller.AddressSeller,
                    Avatar = seller.Avatar,
                    TotalProducts = products?.Count() ?? 0,
                    Products = productDTOs
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xem thông tin cửa hàng: " + ex.Message, ex);
            }
        }
        public void UpdateProduct(int sellerId, int productId, EditProductDTO model)
        {
            try
            {
                var product = _productRepository.GetById(productId);
                if (product == null || product.SellerId != sellerId)
                {
                    throw new Exception("Không tìm thấy sản phẩm");
                }

                // Cập nhật thông tin sản phẩm
                product.ProductName = model.ProductName;
                product.Price = model.Price;
                product.ProductQuantity = model.ProductQuantity;
                product.ProductDescription = model.Description;

                // Cập nhật hình ảnh nếu có
                if (model.ProductImage != null && model.ProductImage.Length > 0)
                {
                    product.ProductImage = model.ProductImage;
                }

                _productRepository.Update(product);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật sản phẩm: " + ex.Message, ex);
            }
        }
    }
} 