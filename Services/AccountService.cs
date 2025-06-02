using System;
using System.Threading.Tasks;
using PBL3.DTO.Shared;
using PBL3.Entity;
using PBL3.Enums;
using PBL3.Repositories;
using System.Text.RegularExpressions;
namespace PBL3.Services
{
    public class AccountService
    {
        private readonly IUserRepositories _userRepository;
        private readonly IPlatformWalletRepositories _walletRepository;

        public AccountService(
            IUserRepositories userRepository,
            IPlatformWalletRepositories walletRepository)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
        }

        public User Login(LoginDTO loginDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDTO.Username) || string.IsNullOrEmpty(loginDTO.Password))
                {
                    throw new ArgumentException("Tài khoản và mật khẩu không được để trống");
                }

                var user = _userRepository.GetByUsername(loginDTO.Username);

                if (user == null || user.Password != loginDTO.Password)
                {
                    throw new InvalidOperationException("Tài khoản hoặc mật khẩu không đúng");
                }
                
                if(user.IsActive == false){
                    throw new InvalidOperationException("Tài khoản của bạn đã bị khoá. Hãy liên hệ với chúng tôi qua email cdtstore@gmail.com để biết thêm thông tin chi tiết");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi đăng nhập: {ex.Message}");
            }
        }

        public User Register(RegisterDTO registerDTO)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(registerDTO.Account) || 
                    string.IsNullOrEmpty(registerDTO.Password) || 
                    string.IsNullOrEmpty(registerDTO.RePassword) ||
                    string.IsNullOrEmpty(registerDTO.Name) ||
                    string.IsNullOrEmpty(registerDTO.PhoneNumber))
                {
                    throw new ArgumentException("Vui lòng điền đầy đủ thông tin");
                }
                if (registerDTO.Password.Length < 6)
                {
                    throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự");
                }
                if (!Regex.IsMatch(registerDTO.PhoneNumber, @"^\d{10}$"))
                {
                    throw new ArgumentException("Số điện thoại phải bao gồm đúng 10 chữ số");
                }

                if (registerDTO.Password != registerDTO.RePassword)
                {
                    throw new ArgumentException("Mật khẩu xác nhận không khớp");
                }

                // Kiểm tra tài khoản đã tồn tại
                var existingUser = _userRepository.GetByUsername(registerDTO.Account);
                if (existingUser != null )
                    throw new Exception("Tài khoản đã tồn tại");

                // Kiểm tra số điện thoại đã tồn tại
                var existingPhone = _userRepository.GetByPhone(registerDTO.PhoneNumber);
                if (existingPhone != null)
                    throw new ArgumentException("Số điện thoại đã được sử dụng");

                // Create new user based on role
                User newUser;
                if (registerDTO.RoleName == Roles.Buyer)
                {
                    newUser = new Buyer
                    {
                        Username = registerDTO.Account,
                        Password = registerDTO.Password,
                        Name = registerDTO.Name,
                        Sex = registerDTO.Sex,
                        PhoneNumber = registerDTO.PhoneNumber,
                        RoleName = Roles.Buyer,
                        Date = registerDTO.Date ?? DateTime.Now,
                        IsActive = true,
                        Location = "",
                    };
                }
                else if (registerDTO.RoleName == Roles.Seller)
                {
                    newUser = new Seller
                    {
                        Username = registerDTO.Account,
                        Password = registerDTO.Password,
                        Name = registerDTO.Name,
                        Sex = registerDTO.Sex,
                        PhoneNumber = registerDTO.PhoneNumber,
                        RoleName = Roles.Seller,
                        Date = registerDTO.Date ?? DateTime.Now,
                        IsActive = true,
                        AddressSeller = "",
                        EmailGeneral = "",
                        JoinedDate = DateTime.Now,
                        StoreName = "",
                    };
                }
                else
                {
                    throw new ArgumentException("Vai trò không hợp lệ");
                }

                // Save user first to get the ID
                _userRepository.Add(newUser);

                // Create wallet for new user
                var wallet = new PlatformWallet
                {
                    WalletBalance = 0,
                    User = newUser
                };

                _walletRepository.Add(wallet);

                return newUser;
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Lỗi đăng ký: {message}");
            }
        }
    }
} 