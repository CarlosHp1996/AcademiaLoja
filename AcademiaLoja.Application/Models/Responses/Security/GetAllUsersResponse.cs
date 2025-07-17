﻿using AcademiaLoja.Application.Models.Dtos;

namespace AcademiaLoja.Application.Models.Responses.Security
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserDto> Users { get; set; }
        public int? TotalCount { get; set; }
        public int? PageCount { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
