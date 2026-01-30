using System.ComponentModel.DataAnnotations;

namespace cfs.demo.Models
{
    public class UserUpdateDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; }

        [Range(0, 120)]
        public int? Age { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        [StringLength(10, MinimumLength = 4)]
        public string? Pincode { get; set; }
    }
}