using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class UserRepositories : IUserRepositories
    {
        private readonly AppDbContext _context;

        public UserRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id); // tốc độ tìm tối ưu hơn nhưng chỉ tìm được theo khóa chính
        }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
            // có thể tìm được với bất cứ điều kiện nào
        }

        public User GetByPhone(string phone)
        {
            return _context.Users.FirstOrDefault(u => u.PhoneNumber == phone);
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
