using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IUserRepositories
    {
        User GetById(int id);
        User GetByUsername(string username);

        User GetByPhone(string phone);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
    }
}
