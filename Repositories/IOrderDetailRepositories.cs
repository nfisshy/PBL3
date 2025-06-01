using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IOrderDetailRepositories
    {
        void Add(OrderDetail orderDetail);
        void Update(OrderDetail orderDetail);
        void Delete(int orderId, int productId);
        OrderDetail GetById(int orderId, int productId);
        public List<OrderDetail> GetByOrderId(int orderId);
    }
}
