using BankingControlPanel.Domain.Common;

using System.ComponentModel.DataAnnotations;

namespace BankingControlPanel.Application.Services.Clients.Dto
{
    public class ClientPagedDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string MobileNumber { get; set; }
        public Sex Sex { get; set; }
    }

    public class FilteredClientsHistoryDto
    {
        public string SearchTerm { get; set; }
        public string? SortCol { get; set; }
        public string? SortDir { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ClientDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string MobileNumber { get; set; }
        public Sex Sex { get; set; }
        public List<AddressDto> Addresses { get; set; }
        public List<BankAccountDto> BankAccounts { get; set; }
    }
    public class CreateClientDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public IFormFile ProfilePhoto { get; set; }
        public string MobileNumber { get; set; }
        public Sex Sex { get; set; }
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
        public List<BankAccountDto> BankAccounts { get; set; } = new List<BankAccountDto>();
    }
    public class UpdateClientDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string MobileNumber { get; set; }
        public Sex Sex { get; set; }
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
        public List<BankAccountDto> BankAccounts { get; set; } = new List<BankAccountDto>();
    }
    public class UpdateClientProfilePhoto
    {
        public int Id { get; set; }
        public IFormFile ProfilePhoto { get; set; }
    }
    public class AddressDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
    }

    public class BankAccountDto
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }

}
