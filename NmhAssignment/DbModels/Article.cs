namespace NmhAssignment.DbModels
{
    public class Article
    {
        public int Id { get; set; } // Primary key
        public string Title { get; set; } // Index


        public virtual ICollection<Author> Authors { get; set; } // Many-To-Many

        public int SiteId { get; set; }
        public virtual Site Site { get; set; } // One-To-Many
    }
}