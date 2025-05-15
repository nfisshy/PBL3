using PBL3.DTO.Buyer;
using PBL3.Repositories;
using PBL3.Entity;
using System;
using System.Linq;
using System.Collections.Generic;
using PBL3.Enums;

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
                    Name = buyer.Name,
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

        public void DoiMatKhau(int buyerId, Buyer_DoiMatKhauDTO model)
        {
            if (buyerId <= 0)
                throw new ArgumentException("ID người mua không hợp lệ", nameof(buyerId));

            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer == null)
                throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");

            if (buyer.Password != model.OldPassword)
                throw new ArgumentException("Mật khẩu cũ không đúng");

            if (model.NewPassword != model.ConfirmPassword)
                throw new ArgumentException("Mật khẩu mới và xác nhận mật khẩu không khớp");

            buyer.Password = model.NewPassword;
            _buyerRepositories.Update(buyer);
        }

        public (List<Buyer_TrangThaiDonHangDTO> DonHang, List<Buyer_ThongBaoVoucherDTO> Voucher) GetThongBao(int buyerId)
        {
            if (buyerId <= 0)
                throw new ArgumentException("ID người mua không hợp lệ", nameof(buyerId));

            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer == null)
                throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");

            try
            {
                // Lấy danh sách đơn hàng
                var donHang = buyer.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new Buyer_TrangThaiDonHangDTO
                    {
                        OrderId = o.OrderId,
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate
                    })
                    .ToList();

                // Lấy danh sách voucher
                var voucher = buyer.Voucher_Buyers
                    .Select(vb => new Buyer_ThongBaoVoucherDTO
                    {
                        VoucherId = vb.VoucherId,
                        EndDate = vb.Voucher.EndDate,
                        IsActive = vb.Voucher.EndDate > DateTime.Now
                    })
                    .OrderByDescending(v => v.EndDate)
                    .ToList();

                return (donHang, voucher);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông báo cho buyer ID {buyerId}: " + ex.Message, ex);
            }
        }

        // Update Name
        public void UpdateName(int buyerId, string newName)
        {
            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer == null) throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");
            buyer.Name = newName;
            _buyerRepositories.Update(buyer);
        }

        // Update Date
        public void UpdateDate(int buyerId, DateTime newDate)
        {
            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer == null) throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");
            buyer.Date = newDate;
            _buyerRepositories.Update(buyer);
        }

        // Update Sex
        public void UpdateSex(int buyerId, Gender newSex)
        {
            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer == null) throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");
            buyer.Sex = newSex;
            _buyerRepositories.Update(buyer);
        }

        // Update PhoneNumber
        public void UpdatePhoneNumber(int buyerId, string newPhoneNumber)
        {
            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer == null) throw new KeyNotFoundException($"Không tìm thấy người mua với ID: {buyerId}");
            buyer.PhoneNumber = newPhoneNumber;
            _buyerRepositories.Update(buyer);
        }
    }
} 