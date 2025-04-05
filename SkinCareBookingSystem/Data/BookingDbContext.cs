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
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ImageService> ImageServices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Qa> Qas { get; set; }
        public DbSet<QaOption> QaOptions { get; set; }
        public DbSet<QaOptionService> QaOptionServices { get; set; }
        public DbSet<QaAnswer> QaAnswers { get; set; }
        public DbSet<TherapistSchedule> TherapistSchedules { get; set; }
        public DbSet<TherapistTimeSlot> TherapistTimeSlots { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<TherapistSpecialty> TherapistSpecialties { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TherapistTimeSlotLock> TherapistTimeSlotLocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TherapistTimeSlotLock relationships
            modelBuilder.Entity<TherapistTimeSlotLock>()
                .HasOne(tsl => tsl.TherapistTimeSlot)
                .WithMany(ts => ts.TimeSlotLocks)
                .HasForeignKey(tsl => tsl.TherapistTimeSlotId)
                .OnDelete(DeleteBehavior.Cascade);

            // QaAnswer composite key
            modelBuilder.Entity<QaAnswer>()
                .HasKey(qa => new { qa.UserId, qa.QaId });

            // User relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserDetails)
                .WithOne(ud => ud.User)
                .HasForeignKey<UserDetails>(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking relationships
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.CustomerBookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Therapist)
                .WithMany()
                .HasForeignKey(b => b.TherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TherapistTimeSlot)
                .WithMany()
                .HasForeignKey(b => b.TherapistTimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Qa relationships
            modelBuilder.Entity<Qa>()
                .HasOne(q => q.ServiceCategory)
                .WithMany()
                .HasForeignKey(q => q.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // QaOption relationships
            modelBuilder.Entity<QaOption>()
                .HasOne(qo => qo.Qa)
                .WithMany(q => q.Options)
                .HasForeignKey(qo => qo.QaId)
                .OnDelete(DeleteBehavior.Cascade);

            // QaOptionService relationships
            modelBuilder.Entity<QaOptionService>()
                .HasOne(qos => qos.QaOption)
                .WithMany(qo => qo.ServiceRecommendations)
                .HasForeignKey(qos => qos.QaOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QaOptionService>()
                .HasOne(qos => qos.Service)
                .WithMany()
                .HasForeignKey(qos => qos.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // QaAnswer relationships
            modelBuilder.Entity<QaAnswer>()
                .HasOne(qa => qa.User)
                .WithMany()
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QaAnswer>()
                .HasOne(qa => qa.Qa)
                .WithMany()
                .HasForeignKey(qa => qa.QaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QaAnswer>()
                .HasOne(qa => qa.QaOption)
                .WithMany()
                .HasForeignKey(qa => qa.QaOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // TherapistSchedule relationships
            modelBuilder.Entity<TherapistSchedule>()
                .HasMany(ts => ts.TimeSlots)
                .WithOne(ts => ts.TherapistSchedule)
                .HasForeignKey(ts => ts.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TherapistSchedule>()
                .HasOne(ts => ts.TherapistUser)
                .WithMany()
                .HasForeignKey(ts => ts.TherapistId)
                .OnDelete(DeleteBehavior.Restrict);

            // TherapistTimeSlot relationships
            modelBuilder.Entity<TherapistTimeSlot>()
                .HasOne(ts => ts.TimeSlot)
                .WithMany()
                .HasForeignKey(ts => ts.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback relationships
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

            // TherapistSpecialty relationships
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

            // Transaction configuration
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,0)");

            // ImageService relationships
            modelBuilder.Entity<ImageService>()
                .HasOne(ims => ims.Service)
                .WithMany()
                .HasForeignKey(ims => ims.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}