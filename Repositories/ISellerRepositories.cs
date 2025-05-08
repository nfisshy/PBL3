using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface ISellerRepositories
    {
        Seller GetById(int id);
        Seller GetByUsername(string username);
        void Add(Seller seller);
        void Update(Seller seller);
        void Delete(int id);
    }
}
