using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SignalR_Project.Models.Data.Config
{
    public class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Sender)
                .WithMany(x => x.SentMessages)
                .HasForeignKey(x => x.SenderId);

            builder.HasOne(x => x.Chat)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ChatId);
            // Configure the properties
            builder.Property(m => m.StatusId)
            .IsRequired();

            builder.HasOne(m => m.Status)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.StatusId);
        }
    }
}
