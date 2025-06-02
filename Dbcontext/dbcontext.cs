using System;
using PBL3.Entity;
using PBL3.Enums;
using Microsoft.EntityFrameworkCore;

namespace PBL3.Dbcontext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        // DbSet - Tạo bảng tương ứng trong Database
        public DbSet<User> Users { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<AddressBuyer> AddressBuyers { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PlatformWallet> PlatformWallets { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Voucher_Buyer> Voucher_Buyers { get; set; }
        public DbSet<ReturnExchange> ReturnExchanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // pt nay giup cau hinh cac bang va mqh giua cac bang truoc khi EF framework core tao ra cac bang trong db
        {

            modelBuilder.Entity<Seller>().ToTable("Sellers");
            modelBuilder.Entity<Buyer>().ToTable("Buyers");

            base.OnModelCreating(modelBuilder); // dam bao cac cau hinh mac dinh của EF framework từ lớp dbcontext sẽ được thực hiện trước , sau đó mình sẽ có thể thêm cấu hình của riêng mình

            // 1 Buyer <-> N AddressBuyer
            modelBuilder.Entity<Buyer>() // xac định thực thể đang cấu hình
                .HasMany(b => b.Addresses) // b.address là thuộc tính navigation trong lơp buyer
                .WithOne(ab => ab.Buyer) // a.buyer là thuộc tính navigation trong lớp address
                .HasForeignKey(ab => ab.BuyerId); // bang address sẽ có 1 cột buyerid lam khóa ngoại , liên kết đến khóa chính của buyer

            // CartItem(Product,Buyer)
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.ProductId, ci.BuyerId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Buyer)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);  // Không cho phép xóa lan truyền ở đây

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            // 1 Product → 1 Seller
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 Order → N Buyer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BuyerId);
            // 1 Order -> N  Seller
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Seller)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.NoAction);


            // OrderDetail(Order , Product)
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa Order thì OrderDetails cũng bị xóa

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);  // Không cho phép xóa Product nếu có OrderDetails

            // 1 Product → N Review
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1 Review → 1 Buyer
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Buyer)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 Wallet → N Bank
            modelBuilder.Entity<PlatformWallet>()
                .HasMany(w => w.Banks)
                .WithOne(b => b.Wallet)
                .HasForeignKey(b => b.WalletId);

            // 1 Wallet -> 1 User
            modelBuilder.Entity<PlatformWallet>()
                .HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<PlatformWallet>(w => w.UserId);

            // 1 Seller -> N voucher
            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.Seller)
                .WithMany(s => s.Vouchers)
                .HasForeignKey(v => v.SellerId);

            // VoucherBuyer(Voucher,Buyer)
            modelBuilder.Entity<Voucher_Buyer>()
                .HasKey(vb => new { vb.BuyerId, vb.VoucherId });

            modelBuilder.Entity<Voucher_Buyer>()
                .HasOne(vb => vb.Voucher)
                .WithMany(v => v.Voucher_Buyers)
                .HasForeignKey(vb => vb.VoucherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voucher_Buyer>()
                .HasOne(vb => vb.Buyer)
                .WithMany(b => b.Voucher_Buyers)
                .HasForeignKey(vb => vb.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 ReturnExchange -> 1 Product
            modelBuilder.Entity<ReturnExchange>()
                .HasOne(re => re.Product)
                .WithMany()
                .HasForeignKey(re => re.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa Product nếu có ReturnExchange liên kết

            // 1 ReturnExchange -> 1 Order
            modelBuilder.Entity<ReturnExchange>()
                .HasOne(re => re.Order)
                .WithMany()
                .HasForeignKey(re => re.OrderId)
                .OnDelete(DeleteBehavior.Restrict); 
        }

    }
}