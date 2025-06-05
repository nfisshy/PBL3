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

        private readonly ISellerRepositories _sellerRepo;

        public CartService(ICartItemRepositories cartRepo, IProductRepositories productRepo, ISellerRepositories sellerRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _sellerRepo = sellerRepo;
        }
        //đã sửa xong
        public List<Buyer_CartDTO> GetCart(int buyerId)
        {
            var cartItems = _cartRepo.GetByBuyerId(buyerId).ToList();
            var result = cartItems
                .GroupBy(ci => _productRepo.GetById(ci.ProductId)?.SellerId ?? 0)
                .Select(g => {
                    var sellerId = g.Key;
                    var seller = _sellerRepo.GetById(sellerId);
                    var sellerName = seller?.StoreName ?? "Không rõ";
                    var items = g.Select(ci => {
                        var product = _productRepo.GetById(ci.ProductId);
                        return new Buyer_CartItemDTO
                        {
                            ProductId = ci.ProductId,
                            ProductName = product?.ProductName ?? ci.ProductName,
                            Price = product?.Price ?? 0,
                            Image = ci.ProductImage,
                            Quantity = ci.Quantity,
                            IsSelected = false,
                            currentQuantity = product?.ProductQuantity ?? 0,
                            IsActive = product.ProductStatus == PBL3.Enums.ProductStatus.Selling
                        };
                    }).ToList();
                    return new Buyer_CartDTO
                    {
                        sellerID = sellerId,
                        sellerName = sellerName,
                        CartItems = items
                    };
                }).ToList();
            return result;
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

        //đã sửa xong
        public void RemoveFromCart(int buyerId, int productId)
        {
            _cartRepo.Remove(buyerId, productId);
        }

        //đã sửa xong
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