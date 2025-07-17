using AcademiaLoja.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AcademiaLoja.Domain.Entities.Security
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Cpf { get; set; }
        public EnumGender? Gender { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }

        public ApplicationUser()
        {
            Addresses = new HashSet<Address>();
        }
    }
}
