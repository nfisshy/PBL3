using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories
{
    public class AddressBuyerRepositories : IAddressBuyerRepositories
    {
        private readonly AppDbContext _context;

        public AddressBuyerRepositories(AppDbContext context)
        {
            _context = context;
        }

        public AddressBuyer GetById(int id)
        {
            return _context.AddressBuyers.Find(id);
        }

        public void Add(AddressBuyer addressBuyer)
        {
            _context.AddressBuyers.Add(addressBuyer);
            _context.SaveChanges();
        }

        public void Update(AddressBuyer addressBuyer)
        {
            _context.AddressBuyers.Update(addressBuyer);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var addressBuyer = _context.AddressBuyers.Find(id);
            if (addressBuyer != null)
            {
                _context.AddressBuyers.Remove(addressBuyer);
                _context.SaveChanges();
            }
        }
    }
}
