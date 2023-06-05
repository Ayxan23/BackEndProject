namespace BackEndProject.ViewModels
{
    public class HomeViewModel
    {
        public List<Course>? Courses { get; set;}
        public List<Event>? Events { get; set; }
        public List<Blog>? Blogs { get; set; }
        public SubscribeViewModel SubscribeViewModel { get; set; }
    }
}
