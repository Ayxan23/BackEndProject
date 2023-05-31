namespace BackEndProject.Models
{
    public class Blog : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
