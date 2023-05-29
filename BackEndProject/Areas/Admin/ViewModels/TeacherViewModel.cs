namespace BackEndProject.Areas.Admin.ViewModels
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Job { get; set; }
        [Required, MaxLength(500)]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }

        [Required]
        public string Degree { get; set; }
        [Required]
        public string Experience { get; set; }
        [Required]
        public string Hobbies { get; set; }
        [Required]
        public string Faculty { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Mail { get; set; }
        [Required]
        public int Phone { get; set; }
    }
}
