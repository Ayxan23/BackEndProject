namespace BackEndProject.Areas.Admin.ViewModels
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        [Required, MaxLength(300)]
        public string Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
