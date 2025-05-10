using System;
using System.Collections.Generic;
using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;
using PBL3.DTO.Seller;

namespace PBL3.Repositories 
{
    public interface IReviewRepositories
    {
        void Add(Review review);
        void Update(Review review);
        void Delete(int reviewId);
        Review GetById(int reviewId);
        IEnumerable<Review> GetByProductId(int productId);
        IEnumerable<Seller_TopSanPhamDTO> GetTopRatedProducts(int sellerId, int limit);
    }
}
