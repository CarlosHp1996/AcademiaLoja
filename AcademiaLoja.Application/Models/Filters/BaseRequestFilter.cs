namespace AcademiaLoja.Application.Models.Filters
{
    public class BaseRequestFilter
    {
        public int Take { get; set; } = 100;
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public int? Offset { get; set; }
        public string? SortingProp { get; set; }
        public bool Ascending { get; set; } = true;        

        // Ordenação
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } // "asc" ou "desc"

        internal int Skip
        {
            get
            {
                if (Offset != null)
                    return (int)Offset;

                if (Page != null)
                    return ((int)Page - 1) * Take;

                return 0;
            }
        }
    }
}
