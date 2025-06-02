using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.DTO.Buyer;
using PBL3.Entity;
using PBL3.Repositories;

namespace PBL3.Services
{
    public class ReviewService
    {
        private readonly IReviewRepositories _reviewRepository;
        private readonly IBuyerRepositories _buyerRepository;
        private readonly IProductRepositories _productRepository;

        public ReviewService(IReviewRepositories reviewRepository, IBuyerRepositories buyerRepository, IProductRepositories productRepository)
        {
            _reviewRepository = reviewRepository;
            _buyerRepository = buyerRepository;
            _productRepository = productRepository;
        }

        // 1. Lấy toàn bộ đánh giá của một người mua
        public List<Buyer_DanhGiaDTO> GetAllReviewsByBuyer(int buyerId)
        {
            var buyer = _buyerRepository.GetById(buyerId);
            if (buyer == null) throw new Exception("Không tìm thấy người mua.");

            var reviews = _reviewRepository.GetByBuyerId(buyerId);
            var result = new List<Buyer_DanhGiaDTO>();

            foreach (var review in reviews)
            {
                var product = _productRepository.GetById(review.ProductId);
                result.Add(new Buyer_DanhGiaDTO
                {
                    ReviewId = review.ReviewId,
                    ProductId = review.ProductId,
                    ProductName = product?.ProductName ?? "Không xác định",
                    ProductImage = product?.ProductImage,
                    BuyerId = review.BuyerId,
                    BuyerName = buyer.Name,
                    Content = review.Comment,
                    Rating = review.Rating,
                    DateReview = review.DateReview
                });
            }

            return result;
        }

        // 2. Lấy một đánh giá theo reviewId
        public Buyer_DanhGiaDTO GetReviewById(int reviewId)
        {
            var review = _reviewRepository.GetById(reviewId);
            if (review == null || !review.IsActive) return null;

            var buyer = _buyerRepository.GetById(review.BuyerId);
            var product = _productRepository.GetById(review.ProductId);

            if (buyer == null) throw new Exception("Không tìm thấy người mua.");

            return new Buyer_DanhGiaDTO
            {
                ReviewId = review.ReviewId,
                ProductId = review.ProductId,
                ProductName = product?.ProductName ?? "Không xác định",
                ProductImage = product?.ProductImage,
                BuyerId = review.BuyerId,
                BuyerName = buyer.Name,
                Content = review.Comment,
                Rating = review.Rating,
                DateReview = review.DateReview
            };
        }

        // 3. Sửa đánh giá
        public void UpdateReview(int reviewId, string content, int rating)
        {
            var review = _reviewRepository.GetById(reviewId);
            if (review == null || !review.IsActive) throw new Exception("Không tìm thấy đánh giá.");

            review.Comment = content;
            review.Rating = rating;
            review.DateReview = DateTime.Now;

            _reviewRepository.Update(review);
        }

        // 4. Thêm đánh giá mới
        public void AddReview(Buyer_DanhGiaDTO dto)
        {
            var buyer = _buyerRepository.GetById(dto.BuyerId);
            if (buyer == null) throw new Exception("Người mua không tồn tại.");

            var review = new Review
            {
                ProductId = dto.ProductId,
                BuyerId = dto.BuyerId,
                Rating = dto.Rating,
                Comment = dto.Content,
                DateReview = DateTime.Now,
                IsActive = true
            };

            _reviewRepository.Add(review);
        }
    }
}
