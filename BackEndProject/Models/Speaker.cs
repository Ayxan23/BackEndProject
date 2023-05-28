namespace BackEndProject.Models
{
    public class Speaker : BaseEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Job { get; set; }
        public string Image { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<EventSpeaker> EventSpeakers { get; set; }
    }
}
