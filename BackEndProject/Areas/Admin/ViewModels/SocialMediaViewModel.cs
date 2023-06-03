namespace BackEndProject.Areas.Admin.ViewModels
{
    public class SocialMediaViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string ClassName { get; set; }
        [Required, DataType(DataType.Url)]
        public string Link { get; set; }
        public int TeacherId { get; set; }
    }
}
