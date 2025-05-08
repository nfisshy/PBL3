using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class CartItemRepositories : ICartItemRepositories
    {
        private readonly AppDbContext _context;

        public CartItemRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(CartItem item)
        {
            _context.CartItems.Add(item);
            _context.SaveChanges();
        }

        public void Update(CartItem item)
        {
            _context.CartItems.Update(item);
            _context.SaveChanges();
        }

        public void Remove(int buyerId, int productId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.BuyerId == buyerId && ci.ProductId == productId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public CartItem Get(int buyerId, int productId)
        {
            return _context.CartItems.FirstOrDefault(ci => ci.BuyerId == buyerId && ci.ProductId == productId);
        }

        public IEnumerable<CartItem> GetByBuyerId(int buyerId)
        {
            return _context.CartItems.Where(ci => ci.BuyerId == buyerId).ToList();
        }

        public void ClearCart(int buyerId)
        {
            var cartItems = _context.CartItems.Where(ci => ci.BuyerId == buyerId).ToList();
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();
        }
    }
}
