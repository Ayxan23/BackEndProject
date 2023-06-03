namespace BackEndProject.ViewModels
{
    public class SubscribeViewModel
    {
        public int Id { get; set; }
        [DataType(DataType.EmailAddress), MaxLength(256)]
        public string? Email { get; set; }
    }
}
