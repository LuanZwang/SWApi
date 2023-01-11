using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWApi.Domain.Utils;

namespace SWApi.Test.Domain.Utils
{
    [TestClass]
    public class PaginationUtilsTest
    {
        [TestMethod]
        [DataRow(61, null, null, 60)]
        [DataRow(61, 1, 60, 60)]
        [DataRow(61, 2, 60, 1)]
        public void Paginate_Should_Paginate_As_Expected(int quantityToCreate, int? page, int? pageSize, int expectedPageSize)
        {
            var ids = new List<string>();

            for (int i = 0; i < quantityToCreate; i++)
                ids.Add(Guid.NewGuid().ToString());

            ids = ids.OrderBy(x => x).ToList();

            var result = ids.AsQueryable().Paginate(page, pageSize).ToArray();

            page ??= 1;
            pageSize ??= 60;

            Assert.AreEqual(expectedPageSize, result.Length);
            Assert.IsTrue(ids.Skip((page.Value - 1) * pageSize.Value).Take(expectedPageSize).SequenceEqual(result));
        }

        [TestMethod]
        [DataRow(null, null, PaginationUtils.DefaultPageIndex, PaginationUtils.DefaultPageSize)]
        [DataRow(0, 0, PaginationUtils.DefaultPageIndex, PaginationUtils.DefaultPageSize)]
        [DataRow(-1, -2, PaginationUtils.DefaultPageIndex, PaginationUtils.DefaultPageSize)]
        [DataRow(5, 9, 5, 9)]
        public void GetRealPaginationValues_Should_Return_Expected_Values(int? page, int? pageSize, int expectedPage, int expectedPageSize)
        {
            (int actualPage, int actualPageSize) = PaginationUtils.GetRealPaginationValues(page, pageSize);

            Assert.AreEqual(expectedPage, actualPage);
            Assert.AreEqual(expectedPageSize, actualPageSize);
        }

        [TestMethod]
        [DataRow(3, 2, 2)]
        [DataRow(4, 2, 2)]
        [DataRow(1, 2, 1)]
        [DataRow(0, 2, 0)]
        public void GetTotalPages_Should_Return_Expected_Result(int totalCount, int pageSize, int expectedResult)
        {
            var totalPages = PaginationUtils.GetTotalPages(totalCount, pageSize);

            Assert.AreEqual(expectedResult, totalPages);
        }

        [TestMethod]
        [DataRow(3, 1, 2, null, 2)]
        [DataRow(4, 2, 2, 1, null)]
        [DataRow(1, 1, 2, null, null)]
        [DataRow(0, 2, 0, null, null)]
        [DataRow(1, 1, 1, null, null)]
        public void GetPreviousAndNextPages_Should_Return_Expected_Results(int totalCount, int page, int pageSize, int? expectedPreviousPage, int? expectedNextPage)
        {
            (int? previousPage, int? nextPage) = PaginationUtils.GetPreviousAndNextPages(totalCount, page, pageSize);

            Assert.AreEqual(expectedPreviousPage, previousPage);
            Assert.AreEqual(expectedNextPage, nextPage);
        }
    }
}