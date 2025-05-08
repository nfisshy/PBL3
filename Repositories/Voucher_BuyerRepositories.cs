using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class Voucher_BuyerRepositories : IVoucher_BuyerRepositories
    {
        private readonly AppDbContext _context;

        public Voucher_BuyerRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Voucher_Buyer voucherBuyer)
        {
            _context.Voucher_Buyers.Add(voucherBuyer);
            _context.SaveChanges();
        }

        public void Update(Voucher_Buyer voucherBuyer)
        {
            _context.Voucher_Buyers.Update(voucherBuyer);
            _context.SaveChanges();
        }

        public void Delete(int buyerId, string voucherId)
        {
            var voucherBuyer = _context.Voucher_Buyers.Find(buyerId, voucherId);
            if (voucherBuyer != null)
            {
                _context.Voucher_Buyers.Remove(voucherBuyer);
                _context.SaveChanges();
            }
        }

        public Voucher_Buyer GetById(int buyerId, string voucherId)
        {
            return _context.Voucher_Buyers.Find(buyerId, voucherId);
        }

        public IEnumerable<Voucher_Buyer> GetByBuyerId(int buyerId)
        {
            return _context.Voucher_Buyers.Where(vb => vb.BuyerId == buyerId).ToList();
        }

        public IEnumerable<Voucher_Buyer> GetByVoucherId(string voucherId)
        {
            return _context.Voucher_Buyers.Where(vb => vb.VoucherId == voucherId).ToList();
        }
    }
} 