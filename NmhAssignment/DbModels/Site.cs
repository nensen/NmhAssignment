namespace NmhAssignment.DbModels
{
    public class Site
    {
        public int Id { get; set; } // Primary key
        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}