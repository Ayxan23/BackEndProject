namespace BackEndProject.Models
{
    public class Event : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public string Venue { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<EventSpeaker> EventSpeakers { get; set; }
    }
}
