using BankingControlPanel.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingControlPanel.Domain.Entities
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(60)]
        public string FirstName { get; set; }
        [Required, StringLength(60)]
        public string LastName { get; set; }
        [Required, StringLength(11)]
        public string PersonalId { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        [Required,Phone]
        public string MobileNumber { get; set; }
        [Required]
        public Sex Sex { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    }
}
