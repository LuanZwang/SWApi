namespace SWApi.Domain.Utils
{
    public static class EnumerableUtils
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable is null || !enumerable.Any();
    }
}