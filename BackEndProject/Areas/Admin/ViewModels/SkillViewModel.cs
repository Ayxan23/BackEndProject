namespace BackEndProject.Areas.Admin.ViewModels
{
    public class SkillViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, Range(0, 100)]
        public byte Rate { get; set; }
        public int TeacherId { get; set; }
    }
}
