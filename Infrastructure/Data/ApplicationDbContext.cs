using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<StripeSettings> StripeSettings { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<NotificationPreference> NotificationPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trip>()
                .HasMany(t => t.Itineraries)
                .WithOne(i => i.Trip)
                .HasForeignKey(i => i.TripId);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.Trips)
                .WithOne(t => t.ApplicationUser)
                .HasForeignKey(t => t.ApplicationUserId);
            modelBuilder.Entity<Participant>()
                .Property(u => u.ApplicationUserId)
                .IsRequired(false);

            // Define a composite unique constraint on ApplicationUserId and Type
            modelBuilder.Entity<NotificationPreference>()
                .HasIndex(n => new { n.ApplicationUserId, n.Type })
                .IsUnique();
        }
    }
}
