namespace BackEndProject.Areas.Admin.ViewModels
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        [Required, MaxLength(100)]
        public string Job { get; set; }
        [Required, MaxLength(400)]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }

        [Required, MaxLength(100)]
        public string Degree { get; set; }
        [Required, MaxLength(100)]
        public string Experience { get; set; }
        [Required, MaxLength(100)]
        public string Hobbies { get; set; }
        [Required, MaxLength(100)]
        public string Faculty { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Mail { get; set; }
        [Required]
        public int Phone { get; set; }
    }
}
