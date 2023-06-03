namespace BackEndProject.Areas.Admin.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
