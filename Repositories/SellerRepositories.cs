using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class SellerRepositories : ISellerRepositories
    {
        private readonly AppDbContext _context;

        public SellerRepositories(AppDbContext context)
        {
            _context = context;
        }

        public Seller GetById(int id)
        {
            return _context.Sellers.Find(id);
        }

        public Seller GetByUsername(string username)
        {
            return _context.Sellers.FirstOrDefault(s => s.Username == username);
        }

        public void Add(Seller seller)
        {
            _context.Sellers.Add(seller);
            _context.SaveChanges();
        }

        public void Update(Seller seller)
        {
            _context.Sellers.Update(seller);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var seller = _context.Sellers.Find(id);
            if (seller != null)
            {
                _context.Sellers.Remove(seller);
                _context.SaveChanges();
            }
        }
    }
}
