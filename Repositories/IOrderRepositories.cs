using System;
using System.Collections.Generic;
using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;
using PBL3.DTO.Seller;

namespace PBL3.Repositories 
{
    public interface IOrderRepositories
    {
        void Add(Order order);
        void Update(Order order);
        void Delete(int orderId);
        Order GetById(int orderId);
        IEnumerable<Order> GetByBuyerId(int buyerId);
        IEnumerable<Order> GetBySellerId(int sellerId);
        IEnumerable<Order> GetByStatus(OrdStatus status); // lọc theo trạng thái
        IEnumerable<Seller_TopSanPhamDTO> GetTopSellingProducts(int sellerId, DateTime startDate, DateTime endDate, int limit);
        IEnumerable<Seller_TopSanPhamDTO> GetTopRevenueProducts(int sellerId, DateTime startDate, DateTime endDate, int limit);
        decimal GetTotalRevenue(int sellerId, DateTime startDate, DateTime endDate);
        int GetTotalOrders(int sellerId, DateTime startDate, DateTime endDate);
        public IEnumerable<Order> GetByBuyer_Status(int buyerId, OrdStatus status);
    }
}
