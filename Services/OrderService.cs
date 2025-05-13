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

        // public OrderDTO GetOrderById(int orderId)
        // {
        //     var order = _orderRepository.GetById(orderId);
        //     if (order == null) return null;

        //     return MapToOrderDTO(order);
        // }

        // public IEnumerable<OrderDTO> GetOrdersByBuyerId(int buyerId)
        // {
        //     var orders = _orderRepository.GetByBuyerId(buyerId);
        //     return orders.Select(MapToOrderDTO);
        // }

        // public IEnumerable<OrderDTO> GetOrdersBySellerId(int sellerId)
        // {
        //     var orders = _orderRepository.GetBySellerId(sellerId);
        //     return orders.Select(MapToOrderDTO);
        // }

        // public IEnumerable<OrderDTO> GetOrdersByStatus(OrdStatus status)
        // {
        //     var orders = _orderRepository.GetByStatus(status);
        //     return orders.Select(MapToOrderDTO);
        // }
        public List<OrderDTO> PreviewOrder(int buyerID, List<Buyer_CartDTO> selectedItem){
            var result = new List<OrderDTO>();
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
                    SellerName = seller.Name,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrdStatus.WaitConfirm,
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
                    _logger.LogInformation("${totalPrice}",totalPrice);
                    orderDTO.OrderDetails.Add(orderDetail);
                    _logger.LogInformation("${totalPrice}",totalPrice);
                }
                orderDTO.OrderPrice = totalPrice;
                result.Add(orderDTO);
            }
            
            return result;
        }
        public void CreateOrder(OrderDTO orderDTO)
        {
            var order = new Order
            {
                BuyerId = orderDTO.BuyerId,
                SellerId = orderDTO.SellerId,
                OrderDate = DateTime.Now,
                OrderPrice = orderDTO.OrderPrice,
                OrderStatus = OrdStatus.WaitConfirm,
                PaymentMethod = orderDTO.PaymentMethod,
                PaymentStatus = false
            };

            _orderRepository.Add(order);

            foreach (var detail in orderDTO.OrderDetails)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity
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
    }
} 