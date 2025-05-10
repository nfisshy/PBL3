using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;
using PBL3.DTO.Seller;

namespace PBL3.Repositories 
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly AppDbContext _context;

        public OrderRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void Delete(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public Order GetById(int orderId)
        {
            return _context.Orders.Find(orderId);
        }

        public IEnumerable<Order> GetByBuyerId(int buyerId)
        {
            return _context.Orders.Where(o => o.BuyerId == buyerId).ToList();
        }

        public IEnumerable<Order> GetBySellerId(int sellerId)
        {
            return _context.Orders.Where(o => o.SellerId == sellerId).ToList();
        }

        public IEnumerable<Order> GetByStatus(OrdStatus status)
        {
            return _context.Orders.Where(o => o.OrderStatus == status).ToList();
        }

        public IEnumerable<Seller_TopSanPhamDTO> GetTopSellingProducts(int sellerId, DateTime startDate, DateTime endDate, int limit)
        {
            return _context.OrderDetails
                .Where(od => od.Order.SellerId == sellerId &&
                            od.Order.OrderDate >= startDate &&
                            od.Order.OrderDate <= endDate &&
                            od.Order.OrderStatus == OrdStatus.Completed)
                .GroupBy(od => new { od.Product.ProductId, od.Product.ProductName, od.Product.Price })
                .Select(g => new Seller_TopSanPhamDTO
                {
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * g.Key.Price)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(limit)
                .ToList();
        }

        public decimal GetTotalRevenue(int sellerId, DateTime startDate, DateTime endDate)
        {
            return _context.OrderDetails
                .Where(od => od.Order.SellerId == sellerId &&
                            od.Order.OrderDate >= startDate &&
                            od.Order.OrderDate <= endDate &&
                            od.Order.OrderStatus == OrdStatus.Completed)
                .Sum(od => od.Quantity * od.Product.Price);
        }

        public int GetTotalOrders(int sellerId, DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Count(o => o.SellerId == sellerId &&
                           o.OrderDate >= startDate &&
                           o.OrderDate <= endDate &&
                           o.OrderStatus == OrdStatus.Completed);
        }
    }
}
