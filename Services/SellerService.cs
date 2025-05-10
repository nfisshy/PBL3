using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.DTO.Seller;
using PBL3.Repositories;
using PBL3.Enums;
using Microsoft.AspNetCore.Http;

namespace PBL3.Services
{
    public class SellerService
    {
        private readonly ISellerRepositories _sellerRepository;
        private readonly IProductRepositories _productRepository;
        private readonly IOrderRepositories _orderRepository;
        private readonly IReviewRepositories _reviewRepository;

        public SellerService(
            ISellerRepositories sellerRepository,
            IProductRepositories productRepository,
            IOrderRepositories orderRepository,
            IReviewRepositories reviewRepository)
        {
            _sellerRepository = sellerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
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
    }
} 