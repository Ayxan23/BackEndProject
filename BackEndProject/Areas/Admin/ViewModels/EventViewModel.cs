namespace BackEndProject.Areas.Admin.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(300)]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        [Required, MaxLength(100)]
        public string Venue { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }

        public int[]? SpeakerIds { get; set; }
    }
}
