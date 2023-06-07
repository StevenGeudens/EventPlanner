using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EventPlanner.Data
{
    public class EventPlannerContext : IdentityDbContext<CustomUser>
    {
        public EventPlannerContext(DbContextOptions<EventPlannerContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Collaboration> Collaborations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Collaboration>().ToTable("Collaboration");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<Favorite>().ToTable("Favorite");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Status>().ToTable("Status");
        }
    }
}
