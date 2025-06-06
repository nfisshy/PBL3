using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.Repositories;
using PBL3.DTO;
using PBL3.Enums;
using PBL3.Dbcontext;
using PBL3.DTO.Buyer;

namespace PBL3.Services
{
    public class OrderService
    {
        private readonly IOrderRepositories _orderRepository;
        private readonly IOrderDetailRepositories _orderDetailRepository;
        private readonly ISellerRepositories _sellerRepository;
        private readonly IBuyerRepositories _buyerRepository;
        private readonly IPlatformWalletRepositories _walletRepository;
        private readonly IProductRepositories _productRepositories;

        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepositories orderRepository,
                          IOrderDetailRepositories orderDetailRepository,
                          IBuyerRepositories buyerRepository,
                          ISellerRepositories sellerRepository,
                          ILogger<OrderService> logger,
                          IPlatformWalletRepositories walletRepository,
                          IProductRepositories productRepositories)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _buyerRepository = buyerRepository;
            _sellerRepository = sellerRepository;
            _logger = logger;
            _walletRepository = walletRepository;
            _productRepositories = productRepositories;
        }
        public PurchaseDTO PreviewOrder(int buyerID, List<Buyer_CartDTO> selectedItem)
        {
            var result = new PurchaseDTO();
            // Get buyer information
            var buyer = _buyerRepository.GetById(buyerID);
            if (buyer == null)
                throw new Exception("Buyer not found");

            foreach (var sellerCart in selectedItem)
            {
                var sellerId = sellerCart.sellerID;
                var seller = _sellerRepository.GetById(sellerId);
                if (seller == null)
                    throw new Exception($"Seller with ID {sellerId} not found");

                var orderDTO = new OrderDTO
                {
                    BuyerId = buyerID,
                    BuyerName = buyer.Name,
                    BuyerPhone = buyer.PhoneNumber,
                    Address = buyer.Location,
                    SellerId = sellerId,
                    SellerStoreName = seller.StoreName,
                    //OrderDate = DateTime.Now,
                    //OrderStatus = OrdStatus.WaitConfirm,
                    PaymentStatus = false,
                    OrderDetails = new List<OrderDetailDTO>()
                };

                decimal totalPrice = 0;
                foreach (var cartItem in sellerCart.CartItems)
                {
                    var orderDetail = new OrderDetailDTO
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        Quantity = cartItem.Quantity,
                        TotalPrice = cartItem.Price * cartItem.Quantity,
                        Image = cartItem.Image,
                        ImageData = cartItem.ImageData
                    };
                    totalPrice += orderDetail.TotalPrice;
                    _logger.LogInformation("djfldfhsdahfkldhlfkj");
                    _logger.LogInformation("${totalPrice}", totalPrice);
                    orderDTO.OrderDetails.Add(orderDetail);
                    _logger.LogInformation("${totalPrice}", totalPrice);
                }
                orderDTO.OrderPrice = totalPrice;
                result.Orders.Add(orderDTO);
                result.purchasePrice += orderDTO.OrderPrice;
            }
            return result;
        }
        public void CreateOrder(OrderDTO orderDTO)
{
    // Create the main order
    var order = new Order
    {
        BuyerId = orderDTO.BuyerId,
        SellerId = orderDTO.SellerId,
        OrderDate = DateTime.Now,
        OrderPrice = orderDTO.OrderPrice + 22000,
        OrderStatus = OrdStatus.WaitConfirm,
        PaymentMethod = orderDTO.PaymentMethod,
        PaymentStatus = orderDTO.PaymentStatus,
        Address = orderDTO.Address,
        QuantityTypeOfProduct = orderDTO.OrderDetails.Count,
        Discount = orderDTO.Discount
    };

    _orderRepository.Add(order);

    // Create order details
    foreach (var detail in orderDTO.OrderDetails)
    {
        byte[] imageBytes = null;

        if (!string.IsNullOrEmpty(detail.ImageData))
        {
            try
            {
                // Nếu chuỗi base64 có tiền tố data:image/jpeg;base64,... thì cần cắt bỏ
                var base64Data = detail.ImageData.Contains(",")
                    ? detail.ImageData.Split(',')[1]
                    : detail.ImageData;

                imageBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                // Log lỗi base64 không hợp lệ nếu cần
                imageBytes = null;
            }
        }

        var orderDetail = new OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = detail.ProductId,
            Quantity = detail.Quantity,
            Price = detail.TotalPrice,
            Productname = detail.ProductName,
            Image = imageBytes // gán vào đây
        };

        _orderDetailRepository.Add(orderDetail);
    }
}


public void UpdateOrderStatus(int orderId, int buyerId, OrdStatus newStatus)
{
    var order = _orderRepository.GetById(orderId);
    if (order != null)
    {
        order.OrderStatus = newStatus;
        

        if (newStatus == OrdStatus.Completed)
        {
            order.OrderReceivedDate = DateTime.Now;
            order.PaymentStatus = true;
            var wallet = _walletRepository.GetByUserId(order.SellerId);
            if (wallet != null)
            {
                wallet.WalletBalance += (order.OrderPrice - 22000)*0.95M;
                _walletRepository.Update(wallet);
            }

            // Lấy danh sách chi tiết đơn hàng
                    var orderDetails = _orderDetailRepository.GetByOrderId(orderId);

                    foreach (var detail in orderDetails)
                    {
                        var product = _productRepositories.GetById(detail.ProductId);
                        if (product != null)
                        {
                            product.SoldProduct += detail.Quantity;
                            _productRepositories.Update(product);
                        }
                    }
        }
        else if (newStatus == OrdStatus.Canceled)
        {
            var wallet = _walletRepository.GetByUserId(buyerId);
            if (wallet != null)
            {
                wallet.WalletBalance += order.OrderPrice;
                _walletRepository.Update(wallet);
            }
            order.PaymentStatus = false;
        }

        _orderRepository.Update(order);
    }
}

        public void UpdatePaymentStatus(int orderId, bool paymentStatus)
        {
            var order = _orderRepository.GetById(orderId);
            if (order != null)
            {
                order.PaymentStatus = paymentStatus;
                _orderRepository.Update(order);
            }
        }

        public List<OrderDTO> GetOrdersByStatus(int buyerId, OrdStatus? status = null)
        {
            var orders = status.HasValue
                ? _orderRepository.GetByBuyer_Status(buyerId, status.Value)
                : _orderRepository.GetByBuyerId(buyerId);

            var orderDTOs = new List<OrderDTO>();

            foreach (var order in orders)
            {
                var buyer = _buyerRepository.GetById(order.BuyerId);
                var seller = _sellerRepository.GetById(order.SellerId);

                // Lấy order details từ repository
                var orderDetails = _orderDetailRepository.GetByOrderId(order.OrderId)
                    .Select(od => new OrderDetailDTO
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Productname ?? "Unknown",
                        Quantity = od.Quantity,
                        TotalPrice = od.Price,
                        Image = od?.Image
                    }).ToList();

                orderDTOs.Add(new OrderDTO
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    OrderPrice = order.OrderPrice,
                    OrderStatus = order.OrderStatus,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    SellerStoreName = seller?.StoreName ?? "N/A",
                    OrderDetails = orderDetails
                });
            }

            return orderDTOs.OrderByDescending(o => o.OrderDate).ToList();
        }
        
        public OrderDTO GetOrderById(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                return null;

            var buyer = _buyerRepository.GetById(order.BuyerId);
            var seller = _sellerRepository.GetById(order.SellerId);

            // Lấy order details từ repository
            var orderDetails = _orderDetailRepository.GetByOrderId(order.OrderId)
                .Select(od => new OrderDetailDTO
                {
                    ProductId = od.ProductId,
                    ProductName = od.Productname ?? "Unknown",
                    Quantity = od.Quantity,
                    TotalPrice = od.Price,
                    Image = od?.Image
                }).ToList();

            var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                BuyerId = order.BuyerId,
                OrderDate = order.OrderDate,
                DeliveryDate = order.OrderReceivedDate,
                OrderPrice = order.OrderPrice,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                SellerStoreName = seller?.StoreName ?? "N/A",
                BuyerName = buyer?.Username ?? "N/A",
                BuyerPhone = buyer?.PhoneNumber ?? "N/A",
                Address = order.Address,
                Discount = order.Discount,
                OrderDetails = orderDetails
            };

            return orderDTO;
        }
    }
} 