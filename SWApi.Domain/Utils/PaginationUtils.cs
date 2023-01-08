namespace SWApi.Domain.Utils;

public static class PaginationUtils
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int page, int pageSize) =>
        queryable.Skip((page - 1) * pageSize).Take(pageSize);

    public static (int Page, int PageSize) GetRealPaginationValues(int? page, int? pageSize)
    {
        int realPageValue = page.HasValue && page.Value >= 1 ? page.Value : 1;
        int realPageSizeValue = pageSize.HasValue && pageSize.Value >= 1 ? pageSize.Value : 60;

        return (realPageValue, realPageSizeValue);
    }
}