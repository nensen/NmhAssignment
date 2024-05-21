namespace NmhAssignment.Services
{
    public class KeyValueModel
    {
        public KeyValueModel(decimal value, DateTime lastUpdated)
        {
            Value = value;
            LastUpdated = lastUpdated;
        }

        public decimal Value { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}