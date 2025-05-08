using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class ProductRepositories : IProductRepositories
    {
        private readonly AppDbContext _context;

        public ProductRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public Product GetById(int productId)
        {
            return _context.Products.Find(productId);
        }

        public IEnumerable<Product> GetBySellerId(int sellerId)
        {
            return _context.Products.Where(p => p.SellerId == sellerId).ToList();
        }

        public IEnumerable<Product> GetByType(TypeProduct productType)
        {
            return _context.Products.Where(p => p.ProductType == productType).ToList();
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.Where(p => p.ProductStatus == ProductStatus.Selling).ToList();
        }
    }
}
