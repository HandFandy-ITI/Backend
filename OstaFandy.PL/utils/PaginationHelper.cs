namespace OstaFandy.PL.utils
{
    public class PaginationHelper<T>
    {
        public List<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string SearchString { get; set; }

        public static PaginationHelper<T> Create(IEnumerable<T> source, int pageNumber, int pageSize, string searchString = "")
        {
            var totalCount = source.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var data = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationHelper<T>
            {
                Data = data,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalCount = totalCount,
                SearchString = searchString
            };
        }
    }
}
