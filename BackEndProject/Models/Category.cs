namespace BackEndProject.Models
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public bool IsDeleted { get; set; }

        public ICollection<CourseCategory> CourseCategories { get; set; }
    }
}
