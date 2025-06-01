using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class OrderDetailRepositories : IOrderDetailRepositories
    {
        private readonly AppDbContext _context;

        public OrderDetailRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            _context.SaveChanges();
        }

        public void Delete(int orderId, int productId)
        {
            var orderDetail = _context.OrderDetails.Find(orderId, productId);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                _context.SaveChanges();
            }
        }

        public OrderDetail GetById(int orderId, int productId)
        {
            return _context.OrderDetails.Find(orderId, productId);
        }
        public List<OrderDetail> GetByOrderId(int orderId)
        {
            return _context.OrderDetails
                        .Where(od => od.OrderId == orderId)
                        .ToList();
        }
    }
}
