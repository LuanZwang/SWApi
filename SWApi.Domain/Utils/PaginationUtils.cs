namespace SWApi.Domain.Utils
{
    public static class PaginationUtils
    {
        public const int DefaultPageIndex = 1;
        public const int DefaultPageSize = 60;

        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int? page, int? pageSize)
        {
            if (!page.HasValue)
                page = DefaultPageIndex;

            if (!pageSize.HasValue)
                pageSize = DefaultPageSize;

            return queryable.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        public static (int Page, int PageSize) GetRealPaginationValues(int? page, int? pageSize)
        {
            int realPageValue = page.HasValue && page.Value >= 1 ? page.Value : DefaultPageIndex;
            int realPageSizeValue = pageSize.HasValue && pageSize.Value >= 1 ? pageSize.Value : DefaultPageSize;

            return (realPageValue, realPageSizeValue);
        }

        public static int GetTotalPages(long totalCount, int pageSize)
        {
            if (totalCount == 0 && pageSize == 0)
                return 0;

            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        public static (int? PreviousPage, int? NextPage) GetPreviousAndNextPages(long totalCount, int page, int pageSize)
        {
            var totalPages = GetTotalPages(totalCount, pageSize);

            int? nextPage = page + 1 > totalPages ? null : page + 1;
            int? previousPage = page == 1 || totalPages == 0 ? null : page - 1;

            return (previousPage, nextPage);
        }
    }
}