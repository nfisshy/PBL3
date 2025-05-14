using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;
using System.Collections.Generic;

namespace PBL3.Repositories 
{
    public interface IBankRepositories
    {
        Bank GetById(int bankAccountId);                     
        void Add(Bank bank);                              
        void Update(Bank bank);                           
        void Delete(int bankAccountId);
        IEnumerable<Bank> GetByWalletId(int walletId);
    }
}
