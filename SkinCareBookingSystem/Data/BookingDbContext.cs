using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

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
        public DbSet<TherapistSchedule> TherapistSchedules { get; set; }
        public DbSet<TherapistTimeSlot> TherapistTimeSlots { get; set; }
        public DbSet<ServiceRecommendation> ServiceRecommendations { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<TherapistSpecialty> TherapistSpecialties { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingDetails>().HasKey(bd => new { bd.BookingId, bd.ServiceId });
            modelBuilder.Entity<CartItem>().HasKey(ci => new { ci.UserId, ci.ServiceId });
            modelBuilder.Entity<QaAnswer>().HasKey(qa => new { qa.UserId, qa.QaId });

            modelBuilder.Entity<ServiceRecommendation>()
                .HasOne(sr => sr.Qa)
                .WithMany()
                .HasForeignKey(sr => sr.QaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRecommendation>()
                .HasOne(sr => sr.Service)
                .WithMany()
                .HasForeignKey(sr => sr.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);
            modelBuilder.Entity<User>().HasOne(u => u.UserDetails).WithOne(ud => ud.User).HasForeignKey<UserDetails>(ud => ud.UserId);
            modelBuilder.Entity<User>().HasOne(u => u.Wallet).WithOne(w => w.User).HasForeignKey<Wallet>(w => w.UserId);

            modelBuilder.Entity<TherapistSchedule>()
                .HasMany(ts => ts.TimeSlots)
                .WithOne(ts => ts.TherapistSchedule)
                .HasForeignKey(ts => ts.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TherapistTimeSlot>()
                .HasOne(ts => ts.TimeSlot)
                .WithMany()
                .HasForeignKey(ts => ts.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.CustomerBookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.StaffUser)
                .WithMany()
                .HasForeignKey(b => b.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TherapistTimeSlot)
                .WithMany()
                .HasForeignKey(b => b.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingDetails>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingDetails>()
                .HasOne(bd => bd.Service)
                .WithMany()
                .HasForeignKey(bd => bd.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Service)
                .WithMany()
                .HasForeignKey(f => f.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>().HasOne(ci => ci.User).WithMany().HasForeignKey(ci => ci.UserId);
            modelBuilder.Entity<CartItem>().HasOne(ci => ci.Service).WithMany().HasForeignKey(ci => ci.ServiceId);

            modelBuilder.Entity<Qa>().HasOne(q => q.ServiceCategory).WithMany().HasForeignKey(q => q.ServiceCategoryId);
            modelBuilder.Entity<QaAnswer>().HasOne(qa => qa.User).WithMany().HasForeignKey(qa => qa.UserId);
            modelBuilder.Entity<QaAnswer>().HasOne(qa => qa.Qa).WithMany().HasForeignKey(qa => qa.QaId);

            modelBuilder.Entity<TherapistSchedule>()
                .HasOne(ts => ts.TherapistUser)
                .WithMany()
                .HasForeignKey(ts => ts.TherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TherapistSpecialty>()
                .HasKey(ts => ts.Id);

            modelBuilder.Entity<TherapistSpecialty>()
                .HasOne(ts => ts.Therapist)
                .WithMany(u => u.TherapistSpecialties)
                .HasForeignKey(ts => ts.TherapistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TherapistSpecialty>()
                .HasOne(ts => ts.ServiceCategory)
                .WithMany(sc => sc.TherapistSpecialties)
                .HasForeignKey(ts => ts.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,0)");

            base.OnModelCreating(modelBuilder);
        }
    }
}