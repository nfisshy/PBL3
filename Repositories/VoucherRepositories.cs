using PBL3.Entity;
using PBL3.Dbcontext;
using PBL3.Enums;

namespace PBL3.Repositories 
{
    public class VoucherRepositories : IVoucherRepositories
    {
        private readonly AppDbContext _context;

        public VoucherRepositories(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            _context.SaveChanges();
        }

        public void Update(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            _context.SaveChanges();
        }

        public void Delete(string voucherId)
        {
            var voucher = _context.Vouchers.Find(voucherId);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                _context.SaveChanges();
            }
        }

        public Voucher GetById(string voucherId)
        {
            return _context.Vouchers.Find(voucherId);
        }

        public IEnumerable<Voucher> GetBySellerId(int sellerId)
        {
            return _context.Vouchers.Where(v => v.SellerId == sellerId).ToList();
        }
    }
}
