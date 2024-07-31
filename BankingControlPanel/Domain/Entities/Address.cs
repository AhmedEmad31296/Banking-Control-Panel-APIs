using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankingControlPanel.Domain.Entities
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Country { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(200)]
        public string Street { get; set; }
        [StringLength(20)]
        public string ZipCode { get; set; }
        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
}
