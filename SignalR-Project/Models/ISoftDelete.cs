namespace SignalR_Project.Models
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }

        void Undo();
    }
}
