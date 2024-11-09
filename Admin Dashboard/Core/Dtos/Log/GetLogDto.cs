namespace Admin_Dashboard.Core.Dtos.Log
{
    public class GetLogDto
    {
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string UserName { get; set; }
        public string Descreption { get; set; }
    }
}
