namespace Admin_Dashboard.Core.Entities
{
    public class Message : BaseEntity<long>
    {
        public string SendUserName { get; set; }
        public string ReceverUserName { get; set; }
        public string Text { get; set; }
    }
}
