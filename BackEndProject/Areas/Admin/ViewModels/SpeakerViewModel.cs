namespace BackEndProject.Areas.Admin.ViewModels
{
    public class SpeakerViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Job { get; set; }
        public IFormFile? Image { get; set; }
    }
}
