using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface ICartItemRepositories
    {
        void Add(CartItem item);   // thêm sản phẩm vào giỏ
        void Update(CartItem item); // cập nhật số lượng sản phẩm trong giỏ
        void Remove(int buyerId, int productId); // xóa sản phẩm khỏi giỏ
        CartItem Get(int buyerId, int productId); // lấy sản phẩm trong giỏ
        IEnumerable<CartItem> GetByBuyerId(int buyerId); // lấy tất cả sản phẩm trong giỏ của người mua
        void ClearCart(int buyerId); // xóa tất cả sản phẩm trong giỏ của người mua
    }
}
