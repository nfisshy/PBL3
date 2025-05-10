using System.Collections.Generic;
using System.Linq;
using PBL3.DTO.Buyer;
using PBL3.Repositories;
using PBL3.Entity;

namespace PBL3.Services
{
    public class CartService
    {
        private readonly ICartItemRepositories _cartRepo;
        private readonly IProductRepositories _productRepo;

        public CartService(ICartItemRepositories cartRepo, IProductRepositories productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public Buyer_CartDTO GetCart(int buyerId)
        {
            var items = _cartRepo.GetByBuyerId(buyerId)
                .Select(ci => {
                    var product = _productRepo.GetById(ci.ProductId);
                    return new Buyer_CartItemDTO
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.ProductName,
                        Price = product != null ? product.Price : 0,
                        Image = ci.ProductImage,
                        Quantity = ci.Quantity
                    };
                }).ToList();

            return new Buyer_CartDTO { CartItems = items };
        }

        public void AddToCart(int buyerId, int productId, int quantity)
        {
            var existing = _cartRepo.Get(buyerId, productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                _cartRepo.Update(existing);
            }
            else
            {
                var product = _productRepo.GetById(productId);
                if (product == null) return;
                _cartRepo.Add(new CartItem
                {
                    BuyerId = buyerId,
                    ProductId = productId,
                    Quantity = quantity,
                    ProductName = product.ProductName,
                    ProductImage = product.ProductImage
                });
            }
        }

        public void RemoveFromCart(int buyerId, int productId)
        {
            _cartRepo.Remove(buyerId, productId);
        }

        public void UpdateQuantity(int buyerId, int productId, int quantity)
        {
            var item = _cartRepo.Get(buyerId, productId);
            if (item != null)
            {
                item.Quantity = quantity;
                _cartRepo.Update(item);
            }
        }
    }
} 