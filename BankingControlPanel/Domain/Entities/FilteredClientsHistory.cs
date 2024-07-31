using BankingControlPanel.Controllers;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingControlPanel.Domain.Entities
{
    [Table("FilteredClientsHistory")]
    public class FilteredClientsHistory
    {
        [Key]
        public int Id { get; set; }
        public string SearchTerm { get; set; }
        public DateTime SearchDate { get; set; }
        public string? SortCol { get; set; }
        public string? SortDir { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
