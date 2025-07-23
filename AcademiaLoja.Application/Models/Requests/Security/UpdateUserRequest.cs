using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AcademiaLoja.Application.Models.Requests.Security
{
    public class UpdateUserRequest
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Password { get; set; }

        public string? Cpf { get; set; }

        public EnumGender? Gender { get; set; }

        public ICollection<AddressDto>? Addresses { get; set; }

        // NOVA PROPRIEDADE: Para identificar se a atualização é por email (recuperação de senha)
        public bool IsPasswordRecovery { get; set; } = false;
    }
}
