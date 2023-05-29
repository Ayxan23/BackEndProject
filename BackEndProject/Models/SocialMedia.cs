namespace BackEndProject.Models
{
    public class SocialMedia : BaseEntity
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string Link { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public bool IsDeleted { get; set; }
    }
}
