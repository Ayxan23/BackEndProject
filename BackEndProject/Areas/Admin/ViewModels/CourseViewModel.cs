namespace BackEndProject.Areas.Admin.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(300)]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public int ClassDuration { get; set; }
        [Required]
        public string SkillLevel { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public int Students { get; set; }
        [Required]
        public double Price { get; set; }

        public int[]? CategoryIds { get; set;}
    }
}
