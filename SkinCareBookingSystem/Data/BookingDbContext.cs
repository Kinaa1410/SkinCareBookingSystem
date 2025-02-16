using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        // ✅ Define all database tables (DbSets)
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ImageService> ImageServices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetails> BookingDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Qa> Qas { get; set; }
        public DbSet<QaAnswer> QaAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       
            modelBuilder.Entity<BookingDetails>()
                .HasKey(bd => new { bd.BookingId, bd.ServiceId });

            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.UserId, ci.ServiceId });

            modelBuilder.Entity<QaAnswer>()
                .HasKey(qa => new { qa.UserId, qa.QaId });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserDetails)
                .WithOne(ud => ud.User)
                .HasForeignKey<UserDetails>(ud => ud.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

   
            modelBuilder.Entity<Service>()
                .HasOne(s => s.ServiceCategory)
                .WithMany()
                .HasForeignKey(s => s.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImageService>()
                .HasOne(i => i.Service)
                .WithMany()
                .HasForeignKey(i => i.ServiceId);


            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.CustomerBookings)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.StaffUser)
                .WithMany()
                .HasForeignKey(b => b.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TherapistUser)
                .WithMany()
                .HasForeignKey(b => b.TherapistId)
                .OnDelete(DeleteBehavior.Restrict);

    
            modelBuilder.Entity<BookingDetails>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingDetails>()
                .HasOne(bd => bd.Service)
                .WithMany()
                .HasForeignKey(bd => bd.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

       
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Booking)
                .WithMany()
                .HasForeignKey(f => f.BookingId);

    
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Service)
                .WithMany()
                .HasForeignKey(ci => ci.ServiceId);


            modelBuilder.Entity<Qa>()
                .HasOne(q => q.ServiceCategory)
                .WithMany()
                .HasForeignKey(q => q.ServiceCategoryId);

            modelBuilder.Entity<QaAnswer>()
                .HasOne(qa => qa.User)
                .WithMany()
                .HasForeignKey(qa => qa.UserId);

            modelBuilder.Entity<QaAnswer>()
                .HasOne(qa => qa.Qa)
                .WithMany()
                .HasForeignKey(qa => qa.QaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
