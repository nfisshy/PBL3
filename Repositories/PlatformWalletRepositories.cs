using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class PlatformWalletRepositories : IPlatformWalletRepositories
    {
        private readonly AppDbContext _context;

        public PlatformWalletRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(PlatformWallet wallet)
        {
            _context.PlatformWallets.Add(wallet);
            _context.SaveChanges();
        }

        public void Update(PlatformWallet wallet)
        {
            _context.PlatformWallets.Update(wallet);
            _context.SaveChanges();
        }

        public void Delete(int walletId)
        {
            var wallet = _context.PlatformWallets.Find(walletId);
            if (wallet != null)
            {
                _context.PlatformWallets.Remove(wallet);
                _context.SaveChanges();
            }
        }

        public PlatformWallet GetById(int walletId)
        {
            return _context.PlatformWallets.Find(walletId);
        }

        public PlatformWallet GetByUserId(int userId)
        {
            return _context.PlatformWallets.FirstOrDefault(w => w.UserId == userId);
        }
    }
}
