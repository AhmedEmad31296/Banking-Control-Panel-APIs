
namespace BankingControlPanel.Domain.Helpers
{
    public class DatatableFilterInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }= "asc";
        public int? Draw { get; set; }
    }
    public class DatatableFilterdDto<T>
    {
        public int? Draw { get; set; }

        public long RecordsTotal { get; set; }

        public long RecordsFiltered { get; set; }

        public List<T> AaData { get; set; }

    }
}
