namespace BackEndProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        public string? Fullname { get; set; }
        [Required, MaxLength(30)]
        public string? Username { get; set; }
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}
