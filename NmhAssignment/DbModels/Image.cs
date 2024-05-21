namespace NmhAssignment.DbModels
{
    public class Image
    {
        public int Id { get; set; } // Primary key
        public string Description { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}