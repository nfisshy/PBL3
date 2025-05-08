using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IVoucher_BuyerRepositories
    {
        void Add(Voucher_Buyer voucherBuyer);
        void Update(Voucher_Buyer voucherBuyer);
        void Delete(int buyerId, string voucherId);
        Voucher_Buyer GetById(int buyerId, string voucherId);
        IEnumerable<Voucher_Buyer> GetByBuyerId(int buyerId);
        IEnumerable<Voucher_Buyer> GetByVoucherId(string voucherId);
    }
} 