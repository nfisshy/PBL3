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
        private readonly IAddressBuyerRepositories _addressBuyerRepositories;
        private readonly IVoucher_BuyerRepositories _voucherBuyerRepositories;
        private readonly IVoucherRepositories _voucherRepositories;
        public BuyerService(IBuyerRepositories buyerRepositories, IAddressBuyerRepositories addressBuyerRepositories,
            IVoucher_BuyerRepositories voucherBuyerRepositories, IVoucherRepositories voucherRepositories)
        {
            _buyerRepositories = buyerRepositories;
            _addressBuyerRepositories = addressBuyerRepositories;
            _voucherBuyerRepositories = voucherBuyerRepositories;
            _voucherRepositories = voucherRepositories;
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
        public List<Buyer_SoDiaChiDTO> GetAllAddressByBuyerId(int buyerId)
        {
            var addresses = _addressBuyerRepositories.GetAllByBuyerId(buyerId);
            return addresses.Select(addr => new Buyer_SoDiaChiDTO
            {
                AddressId = addr.AddressId,
                BuyerId = addr.BuyerId,
                LocationName = addr.Location, // Dùng nguyên chuỗi Location
                IsDefault = addr.IsDefault
            }).ToList();
        }

        private (string Street, string Ward, string District, string City) TachDiaChi(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return ("", "", "", "");

            var parts = location.Split(',').Select(p => p.Trim()).ToList();
            parts.Reverse(); // Đảo ngược để xử lý từ cuối xâu

            string city = parts.ElementAtOrDefault(0) ?? "";
            string district = parts.ElementAtOrDefault(1) ?? "";
            string ward = parts.ElementAtOrDefault(2) ?? "";
            string street = parts.Count > 3 ? string.Join(", ", parts.Skip(3).Reverse()) : ""; // Ghép lại phần đầu nếu có

            return (street, ward, district, city);
        }

        private string GopDiaChi(string street, string ward, string district, string city)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(street))
                parts.Add(street.Trim());

            if (!string.IsNullOrWhiteSpace(ward))
                parts.Add(ward.Trim());

            if (!string.IsNullOrWhiteSpace(district))
                parts.Add(district.Trim());

            if (!string.IsNullOrWhiteSpace(city))
                parts.Add(city.Trim());

            return string.Join(", ", parts);
        }
        public void AddAddress(Buyer_SoDiaChiDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Ward))
                throw new ArgumentException("Phường/Xã không được để trống.", nameof(dto.Ward));

            if (string.IsNullOrWhiteSpace(dto.District))
                throw new ArgumentException("Quận/Huyện không được để trống.", nameof(dto.District));

            if (string.IsNullOrWhiteSpace(dto.City))
                throw new ArgumentException("Tỉnh/Thành phố không được để trống.", nameof(dto.City));
            var address = new AddressBuyer
            {
                BuyerId = dto.BuyerId,
                Location = GopDiaChi(dto.Street, dto.Ward, dto.District, dto.City),
                IsDefault = dto.IsDefault
            };

            _addressBuyerRepositories.Add(address);

            if (dto.IsDefault)
            {
                SetDefaultAddress(address.AddressId);
            }
        }

        public void UpdateAddress(Buyer_SoDiaChiDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Ward))
                throw new ArgumentException("Phường/Xã không được để trống.", nameof(dto.Ward));

            if (string.IsNullOrWhiteSpace(dto.District))
                throw new ArgumentException("Quận/Huyện không được để trống.", nameof(dto.District));

            if (string.IsNullOrWhiteSpace(dto.City))
                throw new ArgumentException("Tỉnh/Thành phố không được để trống.", nameof(dto.City));

            var existing = _addressBuyerRepositories.GetById(dto.AddressId);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy địa chỉ với ID: {dto.AddressId}");

            existing.Location = GopDiaChi(dto.Street, dto.Ward, dto.District, dto.City);
            existing.IsDefault = dto.IsDefault;

            _addressBuyerRepositories.Update(existing);

            if (dto.IsDefault)
            {
                SetDefaultAddress(dto.AddressId);
            }
        }
        public void DeleteAddress(int addressId)
        {
            var existing = _addressBuyerRepositories.GetById(addressId);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy địa chỉ với ID: {addressId}");

            int buyerId = existing.BuyerId;

            // Lấy tất cả địa chỉ của buyer
            var allAddresses = _addressBuyerRepositories.GetAllByBuyerId(buyerId);

            // Nếu chỉ có 1 địa chỉ (là chính nó), không cho phép xoá
            if (allAddresses.Count <= 1)
            {
                throw new InvalidOperationException("Phải có ít nhất một địa chỉ.");
            }

            bool wasDefault = existing.IsDefault;

            _addressBuyerRepositories.Delete(addressId);

            if (wasDefault)
            {
                // Sau khi xoá, gán địa chỉ đầu tiên còn lại làm mặc định
                var remaining = _addressBuyerRepositories.GetAllByBuyerId(buyerId);
                var newDefault = remaining.FirstOrDefault();
                if (newDefault != null)
                {
                    SetDefaultAddress(newDefault.AddressId);
                }
            }
        }

        public void SetDefaultAddress(int addressId)
        {
            var address = _addressBuyerRepositories.GetById(addressId);
            if (address == null)
                throw new KeyNotFoundException($"Không tìm thấy địa chỉ với ID: {addressId}");

            var buyerId = address.BuyerId;
            var allAddresses = _addressBuyerRepositories.GetAllByBuyerId(buyerId);

            foreach (var addr in allAddresses)
            {
                addr.IsDefault = (addr.AddressId == addressId);
                _addressBuyerRepositories.Update(addr);
            }

            var buyer = _buyerRepositories.GetById(buyerId);
            if (buyer != null)
            {
                buyer.Location = address.Location;
                _buyerRepositories.Update(buyer);
            }
        }

        public Buyer_SoDiaChiDTO GetAddressById(int addressId)
        {
            var address = _addressBuyerRepositories.GetById(addressId);
            if (address == null)
                return null;

            // Tách địa chỉ thành các phần
            var (street, ward, district, city) = TachDiaChi(address.Location ?? "");

            // Tạo DTO và gán dữ liệu
            var dto = new Buyer_SoDiaChiDTO
            {
                AddressId = address.AddressId,
                BuyerId = address.BuyerId,
                Street = street,
                Ward = ward,
                District = district,
                City = city,
                IsDefault = address.IsDefault
            };

            return dto;
        }
        
        public List<Buyer_VoucherDTO> GetVouchersByBuyerId(int buyerId)
        {
            var voucherBuyers = _voucherBuyerRepositories.GetByBuyerId(buyerId);
            var result = new List<Buyer_VoucherDTO>();

            foreach (var vb in voucherBuyers)
            {
                var voucher = _voucherRepositories.GetById(vb.VoucherId);
                if (voucher != null)
                {
                    result.Add(new Buyer_VoucherDTO
                    {
                        VoucherId = voucher.VoucherId,
                        Description = voucher.Description,
                        StartDate = voucher.StartDate,
                        EndDate = voucher.EndDate,
                        DiscountPercentage = voucher.PercentDiscount,
                        MaxDiscount = voucher.MaxDiscount,
                        IsActive = voucher.EndDate > DateTime.Now,
                        BuyerId = vb.BuyerId
                    });
                }
            }

            return result;
        }

        // ✅ Lưu voucher cho người dùng
        public void SaveVoucherForBuyer(int buyerId, string voucherId)
        {
            if (buyerId <= 0 || string.IsNullOrEmpty(voucherId))
                throw new ArgumentException("Thông tin không hợp lệ.");

            // Kiểm tra đã từng lưu
            var existing = _voucherBuyerRepositories.GetById(buyerId, voucherId);
            if (existing != null)
                throw new InvalidOperationException("Voucher đã được lưu cho người dùng này.");

            // Lấy voucher từ repo
            var voucher = _voucherRepositories.GetById(voucherId);
            if (voucher == null)
                throw new KeyNotFoundException("Không tìm thấy voucher.");

            // Kiểm tra số lượng còn lại
            if (voucher.VoucherQuantity <= 0)
                throw new InvalidOperationException("Voucher đã hết lượt sử dụng.");

            // Tạo và lưu voucher cho người dùng
            var voucherBuyer = new Voucher_Buyer
            {
                BuyerId = buyerId,
                VoucherId = voucherId
            };
            _voucherBuyerRepositories.Add(voucherBuyer);

            // Trừ số lượng voucher
            voucher.VoucherQuantity -= 1;
            if (voucher.VoucherQuantity == 0)
                voucher.IsActive = false; // Nếu hết số lượng, đánh dấu không hoạt động 
            _voucherRepositories.Update(voucher);
        }

    }
} 