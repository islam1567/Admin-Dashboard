namespace Admin_Dashboard.Core.Entities
{
    public class Log : BaseEntity <int>
    {
        public string UserName { get; set; }
        public string Descreption { get; set; }
    }
}
