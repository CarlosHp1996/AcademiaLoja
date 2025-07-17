using AcademiaLoja.Application.Models.Dtos;

namespace AcademiaLoja.Application.Models.Responses.Security
{
    public class UpdateUserResponse
    {
        public UserDto User { get; set; }
        public string Message { get; set; }
    }
}
