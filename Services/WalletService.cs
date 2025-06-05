using PBL3.DTO.Buyer;
using PBL3.Entity;
using PBL3.Repositories;

namespace PBL3.Services
{
    public class WalletService
    {
        private readonly IPlatformWalletRepositories _walletRepo;
        private readonly IBankRepositories _bankRepo;
        private readonly IBuyerRepositories _buyerRepo;
        public WalletService(
            IPlatformWalletRepositories walletRepo,
            IBankRepositories bankRepo,
            IBuyerRepositories buyerRepo)
        {
            _walletRepo = walletRepo;
            _bankRepo = bankRepo;
            _buyerRepo = buyerRepo;
        }

        // 1. Kiểm tra xem ví đã thiết lập mã PIN hay chưa (false = chưa mở lần đầu)
        public bool IsWalletInitialized(int userId)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null || wallet.Pin == 0)
                return false;

            return true;
        }

        // 2. Mở ví (kiểm tra mã PIN)
        public Buyer_WalletDTO? OpenWallet(int userId, int inputPin)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null || wallet.Pin != inputPin)
                return null;

            var buyer = _buyerRepo.GetById(wallet.UserId); // Lấy thông tin người mua

            // Chuyển đổi danh sách Bank entity sang BankDTO
            var banks = _bankRepo.GetByWalletId(wallet.WalletId)
                .Select(b => new BankDTO
                {
                    BankId = b.BankAccountId,
                    BankName = b.BankName,
                    AccountNumber = b.BankNumber
                })
                .ToList();

            return new Buyer_WalletDTO
            {
                WalletId = wallet.WalletId,
                WalletBalance = wallet.WalletBalance,
                UserId = wallet.UserId,
                Pin = wallet.Pin,
                BuyerName = buyer?.Name ?? "Unknown",
                Banks = banks
            };
        }
        public bool VerifyPinForOrder(int userId, int inputPin)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null || wallet.Pin != inputPin)
                return false;
            return true;
        }
        
        public bool CheckBalanceAndDeduct(int userId, decimal amount)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null)
                return false;

            if (wallet.WalletBalance < amount)
                return false;

            wallet.WalletBalance -= amount;
            _walletRepo.Update(wallet); // Cập nhật ví sau khi trừ tiền
            return true;
        }
        public Buyer_WalletDTO? OpenWallet(int userId)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            var buyer = _buyerRepo.GetById(wallet.UserId); // Lấy thông tin người mua

            // Chuyển đổi danh sách Bank entity sang BankDTO
            var banks = _bankRepo.GetByWalletId(wallet.WalletId)
                .Select(b => new BankDTO
                {
                    BankId = b.BankAccountId,
                    BankName = b.BankName,
                    AccountNumber = b.BankNumber
                })
                .ToList();

            return new Buyer_WalletDTO
            {
                WalletId = wallet.WalletId,
                WalletBalance = wallet.WalletBalance,
                UserId = wallet.UserId,
                //Pin = wallet.Pin,
                BuyerName = buyer?.Name ?? "Unknown",
                Banks = banks
            };
        }

        // 3. Nạp tiền vào ví
        public bool Recharge(int userId, decimal amount)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null || amount <= 0)
                return false;

            wallet.WalletBalance += amount;
            _walletRepo.Update(wallet);
            return true;
        }
        public bool ChangeWalletPin(int userId, int oldPin, int newPin)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null || wallet.Pin != oldPin)
                return false;

            wallet.Pin = newPin;
            _walletRepo.Update(wallet);
            return true;
        }

        public bool InitializeWalletPin(int userId, int newPin)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null || wallet.Pin != 0)
                return false;

            wallet.Pin = newPin;
            _walletRepo.Update(wallet);
            return true;
        }
        public void AddBank(int userId, string bankName, string accountNumber)
        {
            var wallet = _walletRepo.GetByUserId(userId);
            if (wallet == null)
                throw new ArgumentException("Không tìm thấy ví cho người dùng.");

            if (string.IsNullOrWhiteSpace(bankName))
                throw new ArgumentException("Tên ngân hàng không được để trống.");

            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Số tài khoản không được để trống.");

            // Kiểm tra tài khoản ngân hàng đã tồn tại chưa
            var existingBank = _bankRepo.GetByWalletId(wallet.WalletId)
                .FirstOrDefault(b => b.BankName == bankName && b.BankNumber == accountNumber);

            if (existingBank != null)
                throw new InvalidOperationException("Tài khoản ngân hàng này đã được liên kết với ví.");

            var newBank = new Bank
            {
                BankName = bankName,
                BankNumber = accountNumber,
                WalletId = wallet.WalletId
            };

            _bankRepo.Add(newBank);
        }


    }
}
