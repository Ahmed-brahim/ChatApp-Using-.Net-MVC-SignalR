using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SignalR_Project.Models.Data.Config
{
    public class ChatParticibantConfig:IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            // Configure the primary key
            builder.HasKey(x => new { x.ChatId , x.Id});
            // Configure the properties
            builder.Property(cp => cp.JoinedAt).HasDefaultValueSql("GETDATE()");
            // Set default value for IsAdmin
            builder.Property(cp => cp.IsAdmin).HasDefaultValue(false);
            // Configure the relationship with Chat
            builder.HasOne(cp => cp.Chat)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure the relationship with ApplicationUser
            builder.HasOne(cp => cp.User)
                .WithMany(u => u.ParticipatingChats)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
