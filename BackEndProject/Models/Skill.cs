namespace BackEndProject.Models
{
    public class Skill : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Rate { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public bool IsDeleted { get; set; }
    }
}
