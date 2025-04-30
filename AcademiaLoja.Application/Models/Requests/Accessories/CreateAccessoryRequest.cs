using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Requests.Accessoriess
{
    public class CreateAccessoryRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

    }
}
