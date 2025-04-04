using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SignalR_Project.Models.Data.Config
{
    public class UserConnectionConfig : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder.HasKey(x => x.ConnectionId);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Connections)
                .HasForeignKey(x => x.UserId);
        }
    }
}
