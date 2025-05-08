using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IBuyerRepositories
    {
        Buyer GetById(int id);
        Buyer GetByUsername(string username);
        void Add(Buyer buyer);
        void Update(Buyer buyer);
        void Delete(int id);
    }
}
