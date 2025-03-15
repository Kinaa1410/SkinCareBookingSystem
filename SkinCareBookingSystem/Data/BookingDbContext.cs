using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        // Define all database tables (DbSets)
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
        public DbSet<ServiceRecommendation> ServiceRecommendations { get; set; } // ✅ Added
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<TherapistSpecialty> TherapistSpecialties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define composite keys
            modelBuilder.Entity<BookingDetails>().HasKey(bd => new { bd.BookingId, bd.ServiceId });
            modelBuilder.Entity<CartItem>().HasKey(ci => new { ci.UserId, ci.ServiceId });
            modelBuilder.Entity<QaAnswer>().HasKey(qa => new { qa.UserId, qa.QaId });

            // ServiceRecommendation Relationships
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

            // User Relationships
            modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);
            modelBuilder.Entity<User>().HasOne(u => u.UserDetails).WithOne(ud => ud.User).HasForeignKey<UserDetails>(ud => ud.UserId);
            modelBuilder.Entity<User>().HasOne(u => u.Wallet).WithOne(w => w.User).HasForeignKey<Wallet>(w => w.UserId);

            // TherapistSchedule & TherapistTimeSlot Relationship
            modelBuilder.Entity<TherapistSchedule>()
                .HasMany(ts => ts.TimeSlots)
                .WithOne(ts => ts.TherapistSchedule)
                .HasForeignKey(ts => ts.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TherapistTimeSlot>()
                .HasOne(ts => ts.TimeSlot) // A TherapistTimeSlot links to a predefined TimeSlot
                .WithMany() // A TimeSlot can be linked to many TherapistTimeSlots
                .HasForeignKey(ts => ts.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking Relationships
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

            // BookingDetails Relationships
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

            // Feedback Relationship
            modelBuilder.Entity<Feedback>().HasOne(f => f.Booking).WithMany().HasForeignKey(f => f.BookingId);

            // CartItem Relationships
            modelBuilder.Entity<CartItem>().HasOne(ci => ci.User).WithMany().HasForeignKey(ci => ci.UserId);
            modelBuilder.Entity<CartItem>().HasOne(ci => ci.Service).WithMany().HasForeignKey(ci => ci.ServiceId);

            // Qa & Related Entities
            modelBuilder.Entity<Qa>().HasOne(q => q.ServiceCategory).WithMany().HasForeignKey(q => q.ServiceCategoryId);
            modelBuilder.Entity<QaAnswer>().HasOne(qa => qa.User).WithMany().HasForeignKey(qa => qa.UserId);
            modelBuilder.Entity<QaAnswer>().HasOne(qa => qa.Qa).WithMany().HasForeignKey(qa => qa.QaId);

            // TherapistSchedule Relationship
            modelBuilder.Entity<TherapistSchedule>()
                .HasOne(ts => ts.TherapistUser)
                .WithMany()
                .HasForeignKey(ts => ts.TherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TherapistSpecialty>()
            .HasKey(ts => ts.Id);

            modelBuilder.Entity<TherapistSpecialty>()
            .HasOne(ts => ts.Therapist)   // Each TherapistSpecialty is associated with one User (Therapist)
            .WithMany(u => u.TherapistSpecialties)  // A User (Therapist) can have many specialties
            .HasForeignKey(ts => ts.TherapistId)   // Foreign key in the TherapistSpecialty table
            .OnDelete(DeleteBehavior.Cascade);  // If a Therapist is deleted, also delete associated specialties

            // ServiceCategory to TherapistSpecialty relationship (one-to-many)
            modelBuilder.Entity<TherapistSpecialty>()
                .HasOne(ts => ts.ServiceCategory)  // Each TherapistSpecialty is associated with one ServiceCategory
                .WithMany(sc => sc.TherapistSpecialties)  // A ServiceCategory can have many therapists
                .HasForeignKey(ts => ts.ServiceCategoryId)  // Foreign key in the TherapistSpecialty table
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

    }
}
