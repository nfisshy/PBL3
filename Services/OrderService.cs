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

        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepositories orderRepository,
                          IOrderDetailRepositories orderDetailRepository,
                          IBuyerRepositories buyerRepository,
                          ISellerRepositories sellerRepository,
                          ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _buyerRepository = buyerRepository;
            _sellerRepository = sellerRepository;
            _logger = logger;
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
                        Image = cartItem.Image
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
                PaymentStatus = false,
                Address = orderDTO.Address,
                QuantityTypeOfProduct = orderDTO.OrderDetails.Count,
                Discount = orderDTO.Discount
            };

            _orderRepository.Add(order);

            // Create order details
            foreach (var detail in orderDTO.OrderDetails)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    Price = detail.TotalPrice,
                    Productname = detail.ProductName,
                    Image = detail.Image
                };
                _orderDetailRepository.Add(orderDetail);
            }
        }

        public void UpdateOrderStatus(int orderId, OrdStatus newStatus)
        {
            var order = _orderRepository.GetById(orderId);
            if (order != null)
            {
                order.OrderStatus = newStatus;
                if( newStatus == OrdStatus.Completed)
                {
                    order.OrderReceivedDate = DateTime.Now; // Cập nhật ngày giao hàng nếu trạng thái là Completed
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

            return orderDTOs;
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