using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class BankRepositories : IBankRepositories
    {
        private readonly AppDbContext _context;

        public BankRepositories(AppDbContext context)
        {
            _context = context;
        }

        public Bank GetById(int bankAccountId)
        {
            return _context.Banks.Find(bankAccountId);
        }

        public void Add(Bank bank)
        {
            _context.Banks.Add(bank);
            _context.SaveChanges();
        }

        public void Update(Bank bank)
        {
            _context.Banks.Update(bank);
            _context.SaveChanges();
        }

        public void Delete(int bankAccountId)
        {
            var bank = _context.Banks.Find(bankAccountId);
            if (bank != null)
            {
                _context.Banks.Remove(bank);
                _context.SaveChanges();
            }
        }
    }
}
