using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Requests.Security
{
    public class UpdateUserRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }
    }
}
