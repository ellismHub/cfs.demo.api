using System.ComponentModel.DataAnnotations;

namespace cfs.demo.Models
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; }

        [Required]
        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        public required string City { get; set; }

        [Required]
        public required string State { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 4)]
        public required string Pincode { get; set; }
    }
}