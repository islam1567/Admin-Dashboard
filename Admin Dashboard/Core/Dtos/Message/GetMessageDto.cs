namespace Admin_Dashboard.Core.Dtos.Message
{
    public class GetMessageDto
    {
        public long Id { get; set; }
        public string SenderUserName { get; set; }
        public string RecevirUserName { get; set; }
        public string Text { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
