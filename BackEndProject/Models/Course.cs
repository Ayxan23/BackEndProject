namespace BackEndProject.Models
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public DateTime Start { get; set; }
        public int Duration { get; set; }
        public int ClassDuration { get; set; }
        public string SkillLevel { get; set; }
        public string Language { get; set; }
        public int Students { get; set; }
        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<CourseCategory> CourseCategories { get; set; }
    }
}
