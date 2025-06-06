using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;
using PBL3.DTO.Seller;

namespace PBL3.Repositories 
{
    public class ReviewRepositories : IReviewRepositories
    {
        private readonly AppDbContext _context;

        public ReviewRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void Delete(int reviewId)
        {
            var review = _context.Reviews.Find(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

        public Review GetById(int reviewId)
        {
            return _context.Reviews.Find(reviewId);
        }
        public IEnumerable<Review> GetByProductId(int productId)
        {
            return _context.Reviews.Where(r => r.ProductId == productId).ToList();
        }

        public IEnumerable<Seller_TopSanPhamTheoDanhGiaDTO> GetTopRatedProducts(int sellerId, int limit)
        {

            return _context.Reviews
                .Where(r => r.Product.SellerId == sellerId && r.IsActive)
                .GroupBy(r => new { r.Product.ProductId, r.Product.ProductName })
                .Select(g => new Seller_TopSanPhamTheoDanhGiaDTO
                {
                    ProductName = g.Key.ProductName,
                    TotalReview = g.Count(), // Số lượng đánh giá
                    AverageRating = (decimal)g.Average(r => r.Rating) // Chuyển đổi double sang decimal
                })
                .OrderByDescending(p => p.AverageRating)
                .Take(limit)
                .ToList();
        }
        public List<Review> GetByBuyerId(int buyerId)
        {
            return _context.Reviews
                .Where(r => r.BuyerId == buyerId && r.IsActive)
                .ToList();
        }
    }
}
