using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalR_Project.Models.Data.Config;

namespace SignalR_Project.Models.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConnectionConfig).Assembly);
            // Add query filters to exclude soft-deleted entities
            modelBuilder.Entity<ApplicationUser>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Chat>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Message>()
                .HasQueryFilter(m => !m.IsDeleted);

            modelBuilder.Entity<ChatParticipant>()
                .HasQueryFilter(cp => !cp.IsDeleted);

            modelBuilder.Entity<UserConnection>()
                .HasQueryFilter(uc => !uc.IsDeleted);
        }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
    }
    
    
}
