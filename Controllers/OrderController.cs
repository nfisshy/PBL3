using Microsoft.AspNetCore.Mvc;
using PBL3.Services;
using PBL3.DTO;
using PBL3.DTO.Buyer;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using PBL3.Enums;
using Newtonsoft.Json;
using PBL3.Entity;
namespace PBL3.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly BuyerService _buyerService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderService orderService, BuyerService buyerService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
            _buyerService = buyerService;
        }
        //đã sửa
        [HttpGet]
        public IActionResult Preview()
        {
            var json = TempData["PreviewOrders"] as string;
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Cart", "Cart");

            var previewOrders = JsonConvert.DeserializeObject<PurchaseDTO>(json);
            return View("Order", previewOrders);
        }
        //đã sửa
        [HttpPost]
        public IActionResult Order([FromBody] List<Buyer_CartDTO> selectedItem)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return RedirectToAction("Login", "Account");


            try
            {
                var cartItems = selectedItem;
                if (cartItems == null || !cartItems.Any())
                {
                    TempData["Error"] = "Vui lòng chọn sản phẩm từ giỏ hàng";
                    return RedirectToAction("Cart", "Cart");
                }


                var previewOrders = _orderService.PreviewOrder(buyerId, cartItems);
                //return View("Order",previewOrders);
                TempData["PreviewOrders"] = JsonConvert.SerializeObject(previewOrders);
                return Ok(new { redirectUrl = Url.Action("Preview") });
            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem trước đơn hàng cho buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Có lỗi xảy ra khi xem trước đơn hàng";
                return RedirectToAction("Cart", "Cart");
            }
        }


        //đã sửa 
        [HttpPost]
        public IActionResult CreateOrder([FromBody] List<OrderDTO> orderDTOs)
        {
            _logger.LogInformation("OrderDTO nhận được: {@orderDTO}", orderDTOs);
            _logger.LogInformation("OrderDetails count: {Count}", orderDTOs.Count);
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
                return RedirectToAction("Login", "Account");

            try
            {
                foreach (var orderDTO in orderDTOs)
                {
                    orderDTO.BuyerId = buyerId;
                    _orderService.CreateOrder(orderDTO);
                }

                // HttpContext.Session.Remove("SelectedCartItems"); // Xóa các sản phẩm đã đặt hàng khỏi giỏ
                TempData["Success"] = "Đặt hàng thành công";
                return Json(new { success = true, redirectUrl = Url.Action("Success", "Order") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng cho buyer ID: {BuyerId}", buyerId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi đặt hàng" });
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrdStatus newStatus)
        {
            try
            {
                _orderService.UpdateOrderStatus(orderId, newStatus);
                TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công";
                return RedirectToAction("OrderDetailHome");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng ID: {OrderId}", orderId);
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng";
                return RedirectToAction("OrderDetailHome");
            }
        }


        [HttpPost]
        public IActionResult UpdatePaymentStatus(int orderId, bool paymentStatus)
        {
            try
            {
                _orderService.UpdatePaymentStatus(orderId, paymentStatus);
                return Json(new { success = true, message = "Cập nhật trạng thái thanh toán thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái thanh toán đơn hàng ID: {OrderId}", orderId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái thanh toán" });
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            // Kiểm tra xem có thông báo thành công trong TempData không
            if (TempData["Success"] == null)
            {
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        [HttpGet]
        public IActionResult OrderDetailHome(OrdStatus? status = null)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation("Bắt đầu lấy đơn hàng cho buyerId = {buyerId}, status = {status}", buyerId, status);
                var orders = _orderService.GetOrdersByStatus(buyerId, status);

                _logger.LogInformation("Số đơn hàng lấy được: {count}", orders.Count);
                ViewBag.CurrentStatus = status;
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách đơn hàng cho buyerId = {buyerId}, status = {status}", buyerId, status);
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách đơn hàng";
                return RedirectToAction("Index", "Product");
            }
        }
        [HttpGet]
        public IActionResult OrderDetail(int orderId)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var order = _orderService.GetOrderById(orderId);

                // Kiểm tra nếu đơn hàng không tồn tại hoặc không thuộc về buyer hiện tại
                if (order == null || order.BuyerId != buyerId)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem đơn hàng này.";
                    return RedirectToAction("OrderDetailHome");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết đơn hàng ID: {OrderId} cho buyerId: {BuyerId}", orderId, buyerId);
                TempData["Error"] = "Có lỗi xảy ra khi tải chi tiết đơn hàng.";
                return RedirectToAction("OrderDetailHome");
            }
        }

        [HttpGet]
        public IActionResult AddressChoose()
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var addresses = _buyerService.GetAllAddressByBuyerId(buyerId);
                return PartialView("AddressChoose", addresses); // Tạo view AddressList.cshtml để hiển thị
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách địa chỉ cho Buyer ID: {BuyerId}", buyerId);
                TempData["Error"] = "Không thể tải danh sách địa chỉ.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult VoucherChoose([FromForm] List<int> sellerIds)
        {
            int buyerId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (buyerId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Gọi service với buyerId và danh sách sellerId
                var vouchers = _buyerService.GetVouchersByBuyerIdAndSellers(buyerId, sellerIds);
                // Trả về PartialView để hiển thị trong modal
                return PartialView(vouchers);
            }
            catch (Exception ex)
            {
                // TODO: log lỗi nếu cần
                return PartialView(new List<Buyer_VoucherDTO>());
            }
        }

    }
} 