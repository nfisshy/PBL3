using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IOrderRepositories
    {
        void Add(Order order);
        void Update(Order order);
        void Delete(int orderId);
        Order GetById(int orderId);
        IEnumerable<Order> GetByBuyerId(int buyerId);
        IEnumerable<Order> GetBySellerId(int sellerId);
        IEnumerable<Order> GetByStatus(OrdStatus status); // lọc theo trạng thái
    }
}
