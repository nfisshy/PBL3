using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IProductRepositories
    {
        void Add(Product product);
        void Update(Product product);
        void Delete(int productId);

        Product GetById(int productId);
        IEnumerable<Product> GetBySellerId(int sellerId);
        IEnumerable<Product> GetByType(TypeProduct productType);
        IEnumerable<Product> GetAll();
    }
}
