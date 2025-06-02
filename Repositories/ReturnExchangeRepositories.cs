using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories
{
    public interface IReturnExchangeRepositories
    {
        void Add(ReturnExchange returnExchange);
        void Update(ReturnExchange returnExchange);
        void Delete(int returnExchangeId);
        ReturnExchange GetById(int returnExchangeId);
        IEnumerable<ReturnExchange> GetAll();
        IEnumerable<ReturnExchange> GetByOrderId(int orderId);
        IEnumerable<ReturnExchange> GetByProductId(int productId);
        IEnumerable<ReturnExchange> GetByStatus(ExchangeStatus status);
    }
    public class ReturnExchangeRepositories : IReturnExchangeRepositories
    {

        private readonly AppDbContext _context;

        public ReturnExchangeRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(ReturnExchange returnExchange)
        {
            _context.ReturnExchanges.Add(returnExchange);
            _context.SaveChanges();
        }

        public void Update(ReturnExchange returnExchange)
        {
            _context.ReturnExchanges.Update(returnExchange);
            _context.SaveChanges();
        }

        public void Delete(int returnExchangeId)
        {
            var returnExchange = _context.ReturnExchanges.Find(returnExchangeId);
            if (returnExchange != null)
            {
                _context.ReturnExchanges.Remove(returnExchange);
                _context.SaveChanges();
            }
        }

        public ReturnExchange GetById(int returnExchangeId)
        {
            return _context.ReturnExchanges.Find(returnExchangeId);
        }

        public IEnumerable<ReturnExchange> GetByOrderId(int orderId)
        {
            return _context.ReturnExchanges.Where(re => re.OrderId == orderId).ToList();
        }

        public IEnumerable<ReturnExchange> GetByProductId(int productId)
        {
            return _context.ReturnExchanges.Where(re => re.ProductId == productId).ToList();
        }

        public IEnumerable<ReturnExchange> GetByStatus(ExchangeStatus status)
        {
            return _context.ReturnExchanges.Where(re => re.Status == status).ToList();
        }

        public IEnumerable<ReturnExchange> GetAll()
        {
            return _context.ReturnExchanges.ToList();
        }
    }
}
