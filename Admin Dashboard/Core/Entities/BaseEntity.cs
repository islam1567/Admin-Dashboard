namespace Admin_Dashboard.Core.Entities
{
    public class BaseEntity <T>
    {
        public T Id { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public bool IsAvtive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
