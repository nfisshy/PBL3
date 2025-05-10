using System;
using System.Collections.Generic;
using System.Linq;
using PBL3.Entity;
using PBL3.DTO.Buyer;
using PBL3.Repositories;
using PBL3.Enums;

namespace PBL3.Services
{
    public class ProductService
    {
        private readonly IProductRepositories _productRepository;
        private readonly IReviewRepositories _reviewRepository;
        private readonly ISellerRepositories _sellerRepository;
        private readonly IBuyerRepositories _buyerRepository;

        public ProductService(
            IProductRepositories productRepository,
            IReviewRepositories reviewRepository,
            ISellerRepositories sellerRepository,
            IBuyerRepositories buyerRepository)
        {
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
            _sellerRepository = sellerRepository;
            _buyerRepository = buyerRepository;
        }

        // Hiển thị toàn bộ sản phẩm của hệ thống
        public List<Buyer_SanPhamDTO> GetAllProducts()
        {
            try
            {
                var products = _productRepository.GetAll();
                if (products == null || !products.Any())
                {
                    return new List<Buyer_SanPhamDTO>();
                }

                return products.Select(p => new Buyer_SanPhamDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Image = p.ProductImage,
                    Rating = CalculateAverageRating(p.ProductId)
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sản phẩm: " + ex.Message, ex);
            }
        }

        // Lọc sản phẩm theo danh mục
        public Buyer_SanPhamTheoDanhMucDTO GetProductsByCategory(TypeProduct category)
        {
            try
            {
                if (!Enum.IsDefined(typeof(TypeProduct), category))
                {
                    throw new ArgumentException("Danh mục sản phẩm không hợp lệ", nameof(category));
                }

                var products = _productRepository.GetByType(category);
                if (products == null || !products.Any())
                {
                    return new Buyer_SanPhamTheoDanhMucDTO
                    {
                        ListProduct = new List<Buyer_SanPhamDTO>(),
                        TypeChosen = category
                    };
                }

                var productDTOs = products.Select(p => new Buyer_SanPhamDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Image = p.ProductImage,
                    Rating = CalculateAverageRating(p.ProductId)
                }).ToList();

                return new Buyer_SanPhamTheoDanhMucDTO
                {
                    ListProduct = productDTOs,
                    TypeChosen = category
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sản phẩm theo danh mục {category}: " + ex.Message, ex);
            }
        }

        // Hiển thị chi tiết sản phẩm
        public Buyer_ChiTietSanPhamDTO GetProductDetails(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    throw new ArgumentException("ID sản phẩm không hợp lệ", nameof(productId));
                }

                var product = _productRepository.GetById(productId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID: {productId}");
                }
                var seller = _sellerRepository.GetById(product.SellerId);
                if (seller == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy thông tin người bán cho sản phẩm ID: {productId}");
                }

                // Lấy danh sách đánh giá
                var reviews = _reviewRepository.GetByProductId(productId);
                var reviewDTOs = reviews?.Select(r => {
                    var buyer = _buyerRepository.GetById(r.BuyerId);
                    return new Buyer_DanhGiaDTO
                    {
                        ProductId = r.ProductId,
                        BuyerId = r.BuyerId,
                        BuyerName = buyer != null ? buyer.Name : $"Người dùng {r.BuyerId}",
                        Content = r.Comment,
                        Rating = r.Rating,
                        DateReview = r.DateReview
                    };
                }).ToList() ?? new List<Buyer_DanhGiaDTO>();

                return new Buyer_ChiTietSanPhamDTO
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Description = product.ProductDescription,
                    Image = product.ProductImage,
                    Quantity = product.ProductQuantity,
                    Rating = CalculateAverageRating(productId),
                    StoreName = seller.StoreName,
                    Comments = reviewDTOs
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết sản phẩm ID {productId}: " + ex.Message, ex);
            }
        }

        // Tính điểm đánh giá trung bình của sản phẩm
        private double CalculateAverageRating(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    throw new ArgumentException("ID sản phẩm không hợp lệ", nameof(productId));
                }

                var reviews = _reviewRepository.GetByProductId(productId);
                if (reviews == null || !reviews.Any())
                {
                    return 0;
                }

                return reviews.Average(r => r.Rating);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính điểm đánh giá cho sản phẩm ID {productId}: " + ex.Message, ex);
            }
        }
    }
} 