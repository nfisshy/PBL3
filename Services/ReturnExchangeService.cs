using System.Collections.Generic;
using System.Linq;
using PBL3.DTO.Buyer;
using PBL3.Entity;
using PBL3.Enums;
using PBL3.Repositories;

namespace PBL3.Services
{
    public class ReturnExchangeService
    {
        private readonly IReturnExchangeRepositories _returnExchangeRepo;
        private readonly IProductRepositories _productRepo;
        private readonly ISellerRepositories _sellerRepo;
        private readonly IBuyerRepositories _buyerRepo;
        private readonly IOrderRepositories _orderRepo;

        public ReturnExchangeService(
            IReturnExchangeRepositories returnExchangeRepo,
            IProductRepositories productRepo,
            ISellerRepositories sellerRepo,
            IBuyerRepositories buyerRepo,
            IOrderRepositories orderRepo)
        {
            _returnExchangeRepo = returnExchangeRepo;
            _productRepo = productRepo;
            _sellerRepo = sellerRepo;
            _buyerRepo = buyerRepo;
            _orderRepo = orderRepo;
        }

        // 1. Lấy toàn bộ bản ghi
        public List<ExchangeStuffDTO> GetAll(int buyerId, ExchangeStatus? status = null)
        {
            var result = new List<ExchangeStuffDTO>();

            // Lấy tất cả các yêu cầu đổi trả, lọc theo status nếu có
            var exchanges = _returnExchangeRepo.GetAll()
                .Where(e => status == null || e.Status == status)
                .ToList();

            foreach (var e in exchanges)
            {
                var order = _orderRepo.GetById(e.OrderId);
                if (order == null || order.BuyerId != buyerId)
                    continue;

                var product = _productRepo.GetById(e.ProductId);
                var seller = _sellerRepo.GetById(product?.SellerId ?? 0);

                var dto = new ExchangeStuffDTO
                {
                    ReturnExchangeId = e.ReturnExchangeId,
                    ProductId = e.ProductId,
                    ProductName = product?.ProductName ?? "N/A",
                    ProductImage = product?.ProductImage,
                    OrderId = e.OrderId,
                    Reason = e.Reason,
                    Image = e.Image,
                    RequestDate = e.RequestDate,
                    ResponseDate = e.ResponseDate,
                    Quantity = e.Quantity,
                    Status = e.Status,
                    SellerStoreName = seller?.StoreName ?? "N/A",
                    BuyerId = buyerId,
                    SellerEmail = seller?.EmailGeneral ?? "N/A",
                    SellerPhone = seller?.PhoneNumber ?? "N/A",
                };

                result.Add(dto);
            }

            return result.OrderByDescending(x => x.RequestDate).ToList();
        }


        // 2. Xoá bản ghi theo id
        public void Delete(int id)
        {
            _returnExchangeRepo.Delete(id);
        }

        // 3. Tạo bản ghi mới
        public void Add(ExchangeStuffDTO dto)
        {
            var entity = new ReturnExchange
            {
                ProductId = dto.ProductId,
                OrderId = dto.OrderId,
                Reason = dto.Reason,
                Image = dto.Image,
                RequestDate = dto.RequestDate,
                ResponseDate = dto.ResponseDate,
                Quantity = dto.Quantity,
                Status = dto.Status
            };

            _returnExchangeRepo.Add(entity);
        }

        // 4. Lấy một bản ghi theo id (trả về DTO)
        public ExchangeStuffDTO GetById(int id, int buyerId)
        {
            var e = _returnExchangeRepo.GetById(id);
            if (e == null) return null;

            var product = _productRepo.GetById(e.ProductId);
            var seller = _sellerRepo.GetById(product?.SellerId ?? 0);
            var buyer = _buyerRepo.GetById(buyerId);

            return new ExchangeStuffDTO
            {
                ReturnExchangeId = e.ReturnExchangeId,
                ProductId = e.ProductId,
                ProductName = product?.ProductName ?? "N/A",
                ProductImage = product?.ProductImage,
                OrderId = e.OrderId,
                Reason = e.Reason,
                Image = e.Image,
                RequestDate = e.RequestDate,
                ResponseDate = e.ResponseDate,
                Quantity = e.Quantity,
                Status = e.Status,
                SellerStoreName = seller?.StoreName ?? "N/A",
                BuyerName = buyer?.Username ?? "N/A",
                SellerEmail = seller?.EmailGeneral ?? "N/A",
                SellerPhone = seller?.PhoneNumber ?? "N/A",
            };
        }
    }
}
