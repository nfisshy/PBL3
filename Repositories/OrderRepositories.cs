using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly AppDbContext _context;

        public OrderRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void Delete(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public Order GetById(int orderId)
        {
            return _context.Orders.Find(orderId);
        }

        public IEnumerable<Order> GetByBuyerId(int buyerId)
        {
            return _context.Orders.Where(o => o.BuyerId == buyerId).ToList();
        }

        public IEnumerable<Order> GetBySellerId(int sellerId)
        {
            return _context.Orders.Where(o => o.SellerId == sellerId).ToList();
        }

        public IEnumerable<Order> GetByStatus(OrdStatus status)
        {
            return _context.Orders.Where(o => o.OrderStatus == status).ToList();
        }
    }
}
