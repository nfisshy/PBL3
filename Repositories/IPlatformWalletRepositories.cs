using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IPlatformWalletRepositories
    {
        void Add(PlatformWallet wallet);
        void Update(PlatformWallet wallet);
        void Delete(int walletId);
        PlatformWallet GetById(int walletId);
        PlatformWallet GetByUserId(int userId);
    }
}
