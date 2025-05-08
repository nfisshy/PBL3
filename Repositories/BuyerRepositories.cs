using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class BuyerRepositories : IBuyerRepositories
    {
        private readonly AppDbContext _context;

        public BuyerRepositories(AppDbContext context)
        {
            _context = context;
        }

        public Buyer GetById(int id)
        {
            return _context.Buyers.Find(id);
        }

        public Buyer GetByUsername(string username)
        {
            return _context.Buyers.FirstOrDefault(b => b.Username == username);
        }

        public void Add(Buyer buyer)
        {
            _context.Buyers.Add(buyer);
            _context.SaveChanges();
        }

        public void Update(Buyer buyer)
        {
            _context.Buyers.Update(buyer);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var buyer = _context.Buyers.Find(id);
            if (buyer != null)
            {
                _context.Buyers.Remove(buyer);
                _context.SaveChanges();
            }
        }
    }
}
