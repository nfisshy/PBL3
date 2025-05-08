using PBL3.DTO.Buyer;
using PBL3.Repositories;
using PBL3.Entity;
using System;

namespace PBL3.Services
{
    public class BuyerService
    {
        private readonly IBuyerRepositories _buyerRepositories;

        public BuyerService(IBuyerRepositories buyerRepositories)
        {
            _buyerRepositories = buyerRepositories;
        }

        public Buyer_ThongTinCaNhanDTO GetThongTinCaNhan(int buyerId)
        {
            if (buyerId <= 0)
                throw new ArgumentException("ID người mua không hợp lệ", nameof(buyerId));
            try
            {
                var buyer = _buyerRepositories.GetById(buyerId);
                if (buyer == null)
                    throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");
                return new Buyer_ThongTinCaNhanDTO
                {
                    UserName = buyer.Username,
                    Sex = buyer.Sex,
                    Date = buyer.Date,
                    PhoneNumber = buyer.PhoneNumber,
                    AddressBuyer = buyer.Location,
                    Avatar = buyer.Avatar
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
                throw new Exception($"Lỗi khi lấy thông tin cá nhân buyer ID {buyerId}: " + ex.Message, ex);
            }
        }
    }
} 