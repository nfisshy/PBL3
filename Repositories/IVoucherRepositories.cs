using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public interface IVoucherRepositories
    {
        void Add(Voucher voucher);
        void Update(Voucher voucher);
        void Delete(string voucherId);
        Voucher GetById(string voucherId);
        IEnumerable<Voucher> GetBySellerId(int sellerId);
    }
}
