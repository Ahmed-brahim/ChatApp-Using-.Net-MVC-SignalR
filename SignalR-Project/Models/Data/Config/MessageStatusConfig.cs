using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SignalR_Project.Models.Data.Config
{
    public class MessageStatusConfig : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.HasKey(ms => ms.Id);
            builder.HasData(
                new MessageStatus { Id = 1, StatusName = "Sent" },
                new MessageStatus { Id = 2, StatusName = "Delivered" },
                new MessageStatus { Id = 3, StatusName = "Read" }
            );
        }
    }
}
