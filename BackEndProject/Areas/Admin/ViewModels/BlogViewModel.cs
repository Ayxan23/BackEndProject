namespace BackEndProject.Areas.Admin.ViewModels
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public string Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
