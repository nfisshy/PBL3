using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PBL3.Services;
using PBL3.DTO.Admin;
using PBL3.Dbcontext;
using PBL3.Entity;
using PBL3.Enums;
using PBL3.Models.Pagination;
using PBL3.Helpers;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    public class AdminController : Controller
    {
        private readonly AccountService _accountService;
        private readonly AppDbContext _context;

        public AdminController(AccountService accountService, AppDbContext context)
        {
            _accountService = accountService;
            _context = context;
        }

        public IActionResult Index()
        {
            // Chuyển về Dashboard.cshtml
            var dashboardData = new Admin_DashboardDTO
            {
                TotalUsers = _context.Users.Count(),
                TotalProducts = _context.Products.Count(),
                TotalOrders = _context.Orders.Count(),
                TotalRevenue = 0
            };
            return View("Dashboard", dashboardData);
        }

        public IActionResult Dashboard()
        {
            var dashboardData = new Admin_DashboardDTO
            {
                TotalUsers = _context.Users.Count(),
                TotalProducts = _context.Products.Count(),
                TotalOrders = _context.Orders.Count(),
                TotalRevenue = 0
            };
            return View(dashboardData);
        }

        public IActionResult UserManagement(string search, string role, string status, int? page)
        {            
            var sellers = _context.Set<Seller>()
                .Select(s => new Admin_UserManagementDTO
                {
                    Id = s.Id,
                    Username = s.Username,
                    Name = s.Name,
                    Gender = s.Sex == Gender.Male ? "Nam" : s.Sex == Gender.Female ? "Nữ" : "Khác",
                    Email = s.EmailGeneral,
                    Role = "Seller",
                    CreatedAt = s.JoinedDate,
                    IsActive = s.IsActive,
                    Status = s.IsActive ? "Hoạt động" : "Bị cấm",
                    PhoneNumber = s.PhoneNumber
                });            
            
            var buyers = _context.Set<Buyer>()
                .Select(b => new Admin_UserManagementDTO
                {
                    Id = b.Id,
                    Username = b.Username,
                    Name = b.Name,
                    Gender = b.Sex == Gender.Male ? "Nam" : b.Sex == Gender.Female ? "Nữ" : "Khác",
                    Email = "",
                    Role = "Buyer",
                    CreatedAt = b.Date,
                    IsActive = b.IsActive,
                    Status = b.IsActive ? "Hoạt động" : "Bị cấm",
                    PhoneNumber = b.PhoneNumber
                });            

            // Kết hợp danh sách Seller và Buyer
            var users = sellers.Union(buyers).ToList();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                users = users.Where(u => 
                    u.Username.ToLower().Contains(search) ||
                    u.Name.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    u.PhoneNumber.ToLower().Contains(search)
                ).ToList();
            }

            // Lọc theo vai trò
            if (!string.IsNullOrEmpty(role) && role != "")
            {
                users = users.Where(u => u.Role == role).ToList();
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status) && status != "")
            {
                users = users.Where(u => u.Status == status).ToList();
            }

            // Phân trang
            int pageSize = 10; // Số lượng item trên mỗi trang
            int pageNumber = page ?? 1; // Nếu không có tham số page, mặc định là trang 1
            var paginatedList = PaginatedList<Admin_UserManagementDTO>.Create(users, pageNumber, pageSize);

            // Lưu các tham số lọc vào ViewBag để giữ lại trạng thái khi chuyển trang
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentRole = role;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = pageNumber;
            
            return View(paginatedList);
        }

        public IActionResult ProductManagement(string search, string category, string status, int? page)
        {
            var products = _context.Products
                .Include(p => p.Seller)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                products = products.Where(p => 
                    p.ProductName.ToLower().Contains(search) || 
                    p.ProductId.ToString().Contains(search) ||
                    p.Seller.StoreName.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(category) && Enum.TryParse<TypeProduct>(category, out var categoryEnum))
            {
                products = products.Where(p => p.ProductType == categoryEnum);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProductStatus>(status, out var statusEnum))
            {
                products = products.Where(p => p.ProductStatus == statusEnum);
            }

            // Select and map to DTO
            var productsQuery = products.Select(p => new Admin_ProductManagementDTO
            {
                Id = p.ProductId,
                ProductName = p.ProductName,
                SellerName = p.Seller.StoreName,
                Price = p.Price,
                StockQuantity = p.ProductQuantity,
                Category = p.ProductType.GetDisplayName(),
                Status = p.ProductStatus.ToString(),
                Image = p.ProductImage,
                CreatedAt = DateTime.Now
            });

            // Pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var paginatedList = PaginatedList<Admin_ProductManagementDTO>.Create(
                productsQuery, 
                pageNumber, 
                pageSize
            );

            // Store filter values in ViewBag
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = pageNumber;

            // Add product categories to ViewBag
            ViewBag.Categories = Enum.GetValues(typeof(TypeProduct))
                .Cast<TypeProduct>()
                .Select(t => new { Value = t.ToString(), Text = t.GetDisplayName() })
                .ToList();

            // Add product statuses to ViewBag with Vietnamese translations
            var statusTranslations = new Dictionary<ProductStatus, string>
            {
                { ProductStatus.Selling, "Đang bán" },
                { ProductStatus.StopSelling, "Ngừng bán" },
                { ProductStatus.Violation, "Vi phạm" },
                { ProductStatus.WaitConfirm, "Chờ xác nhận" }
            };

            ViewBag.Statuses = Enum.GetValues(typeof(ProductStatus))
                .Cast<ProductStatus>()
                .Select(s => new { Value = s.ToString(), Text = statusTranslations[s] })
                .ToList();

            return View(paginatedList);
        }

        public IActionResult OrderManagement(string search, OrdStatus? status, DateTime? fromDate, DateTime? toDate, int? page)
        {
            // Validate date range
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            {
                TempData["Error"] = "Ngày bắt đầu không thể lớn hơn ngày kết thúc";
                return RedirectToAction("OrderManagement");
            }

            var orders = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                orders = orders.Where(o => 
                    o.OrderId.ToString().Contains(search) || 
                    o.Buyer.Username.ToLower().Contains(search) || 
                    o.Seller.Username.ToLower().Contains(search));
            }

            if (status.HasValue)
            {
                orders = orders.Where(o => o.OrderStatus == status.Value);
            }

            if (fromDate.HasValue)
            {
                var startDate = fromDate.Value.Date;
                orders = orders.Where(o => o.OrderDate.Date >= startDate);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                orders = orders.Where(o => o.OrderDate.Date <= endDate);
            }

            // Get total filtered count before pagination
            var totalFilteredOrders = orders.Count();

            // Order by most recent first
            orders = orders.OrderByDescending(o => o.OrderDate);

            // Select and project to DTO
            var ordersQuery = orders.Select(o => new Admin_OrderManagementDTO
            {
                Id = o.OrderId,
                OrderCode = o.OrderId.ToString("D6"),
                BuyerName = o.Buyer.Username,
                SellerName = o.Seller.Username,
                TotalAmount = o.OrderPrice,
                OrderDate = o.OrderDate,
                PaymentStatus = o.PaymentStatus,
                OrderStatus = o.OrderStatus,
                PaymentMethod = o.PaymentMethod,
                Address = o.Address,
                QuantityTypeOfProduct = o.QuantityTypeOfProduct
            });

            // Pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var paginatedList = PaginatedList<Admin_OrderManagementDTO>.Create(
                ordersQuery.ToList(), 
                pageNumber, 
                pageSize
            );

            // Add ViewBag data for filters
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentFromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalFilteredOrders = totalFilteredOrders;

            // Add order statuses to ViewBag with Vietnamese translations
            var statusTranslations = new Dictionary<OrdStatus, string>
            {
                { OrdStatus.WaitConfirm, "Chờ xác nhận" },
                { OrdStatus.Pending, "Đang xử lý" },
                { OrdStatus.Delivering, "Đang giao hàng" },
                { OrdStatus.Completed, "Hoàn thành" },
                { OrdStatus.Canceled, "Đã hủy" }
            };

            ViewBag.Statuses = Enum.GetValues(typeof(OrdStatus))
                .Cast<OrdStatus>()
                .Select(s => new { Value = s, Text = statusTranslations[s] })
                .ToList();

            // Add statistics
            var allOrders = orders.ToList();
            ViewBag.TotalOrders = allOrders.Count;
            ViewBag.NewOrders = allOrders.Count(o => o.OrderStatus == OrdStatus.WaitConfirm);
            ViewBag.ProcessingOrders = allOrders.Count(o => o.OrderStatus == OrdStatus.Pending);
            ViewBag.ShippedOrders = allOrders.Count(o => o.OrderStatus == OrdStatus.Delivering);
            ViewBag.CompletedOrders = allOrders.Count(o => o.OrderStatus == OrdStatus.Completed);
            ViewBag.CanceledOrders = allOrders.Count(o => o.OrderStatus == OrdStatus.Canceled);

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            var orderDetails = new Admin_OrderDetailDTO
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderId.ToString("D6"),
                BuyerName = order.Buyer.Name,
                BuyerPhone = order.Buyer.PhoneNumber,
                SellerName = order.Seller.Name,
                SellerPhone = order.Seller.PhoneNumber,
                TotalAmount = order.OrderPrice,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                ShippingAddress = order.Address,
                OrderItems = order.OrderDetails.Select(od => new OrderItemDetail
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    ProductImage = od.Product.ProductImage != null ? 
                        $"data:image/jpeg;base64,{Convert.ToBase64String(od.Product.ProductImage)}" : 
                        "/images/default-product.jpg",
                    Price = od.Product.Price,
                    Quantity = od.Quantity,
                    Subtotal = od.Product.Price * od.Quantity
                }).ToList()
            };

            return Json(new { success = true, data = orderDetails });
        }

        
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id, string role)
        {
            try
            {
                if (role == "Buyer")
                {
                    var buyer = await _context.Set<Buyer>().FindAsync(id);
                    if (buyer == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy người dùng." });
                    }

                    buyer.IsActive = !buyer.IsActive;
                    await _context.SaveChangesAsync();

                    var newStatus = buyer.IsActive ? "Active" : "Banned";
                    return Json(new { success = true, newStatus = newStatus, isActive = buyer.IsActive });
                }
                else if (role == "Seller")
                {
                    var seller = await _context.Set<Seller>().FindAsync(id);
                    if (seller == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy người dùng." });
                    }

                    seller.IsActive = !seller.IsActive;
                    await _context.SaveChangesAsync();

                    var newStatus = seller.IsActive ? "Active" : "Banned";
                    return Json(new { success = true, newStatus = newStatus, isActive = seller.IsActive });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể thay đổi trạng thái của tài khoản admin." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi thực hiện thao tác." });
            }
        }

        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products
                .Include(p => p.Seller)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("ProductManagement");
            }

            var productDetails = new Admin_ProductManagementDTO
            {
                Id = product.ProductId,
                ProductName = product.ProductName,
                SellerName = product.Seller.StoreName,
                Price = product.Price,
                StockQuantity = product.ProductQuantity,
                Category = product.ProductType.GetDisplayName(),
                Status = product.ProductStatus.ToString(),
                Image = product.ProductImage,
                CreatedAt = DateTime.Now
            };

            return View(productDetails);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleProductViolation(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            // Toggle the product status based on current status
            if (product.ProductStatus == ProductStatus.Violation)
            {
                // If currently banned, change to StopSelling
                product.ProductStatus = ProductStatus.StopSelling;
            }
            else if (product.ProductStatus == ProductStatus.Selling || product.ProductStatus == ProductStatus.StopSelling)
            {
                // If currently Selling or StopSelling, change to Violation
                product.ProductStatus = ProductStatus.Violation;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { 
                    success = true, 
                    message = product.ProductStatus == ProductStatus.Violation ? "Sản phẩm đã bị cấm" : "Đã bỏ cấm sản phẩm",
                    newStatus = product.ProductStatus.ToString()
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái sản phẩm" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                if (product.ProductStatus != ProductStatus.WaitConfirm)
                {
                    return Json(new { success = false, message = "Sản phẩm không trong trạng thái chờ xác nhận" });
                }

                product.ProductStatus = ProductStatus.StopSelling;
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Xác nhận sản phẩm thành công",
                    newStatus = product.ProductStatus.ToString()
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xác nhận sản phẩm" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                if (product.ProductStatus != ProductStatus.WaitConfirm)
                {
                    return Json(new { success = false, message = "Sản phẩm không trong trạng thái chờ xác nhận" });
                }

                product.ProductStatus = ProductStatus.Violation;
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Từ chối sản phẩm thành công",
                    newStatus = product.ProductStatus.ToString()
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi từ chối sản phẩm" });
            }
        }
    }
}