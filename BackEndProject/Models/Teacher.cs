namespace BackEndProject.Models
{
    public class Teacher : BaseEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Job { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public string Degree { get; set; }
        public string Experience { get; set; }
        public string Hobbies { get; set; }
        public string Faculty { get; set; }

        public string Mail { get; set; }
        public int Phone { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<SocialMedia> SocialMedias { get; set; }
        public ICollection<Skill> Skills { get; set; }
    }
}
