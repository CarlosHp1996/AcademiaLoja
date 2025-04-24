using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaLoja.Application.Models.Filters
{
    public class GetBrandsRequestFilter : BaseRequestFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
