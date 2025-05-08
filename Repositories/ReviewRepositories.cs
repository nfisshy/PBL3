using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

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
    }
}
