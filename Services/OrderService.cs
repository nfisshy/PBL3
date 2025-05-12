using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.Repositories;
using PBL3.DTO;
using PBL3.Enums;
using PBL3.Dbcontext;

namespace PBL3.Services
{
    public class OrderService
    {
        private readonly IOrderRepositories _orderRepository;
        private readonly IOrderDetailRepositories _orderDetailRepository;
        private readonly AppDbContext _context;

        public OrderService(IOrderRepositories orderRepository, 
                          IOrderDetailRepositories orderDetailRepository,
                          AppDbContext context)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _context = context;
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