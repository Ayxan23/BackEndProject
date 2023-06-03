namespace BackEndProject.Areas.Admin.ViewModels
{
    public class SpeakerViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        [Required, MaxLength(100)]
        public string Job { get; set; }
        public IFormFile? Image { get; set; }
    }
}
