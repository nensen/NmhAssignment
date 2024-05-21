using System.ComponentModel.DataAnnotations;

namespace NmhAssignment.Models
{
    public class CalculationInput
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Input must be a non-negative decimal.")]
        public decimal Input { get; set; }
    }
}
