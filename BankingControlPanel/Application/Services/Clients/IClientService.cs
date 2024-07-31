using BankingControlPanel.Application.Services.Clients.Dto;
using BankingControlPanel.Domain.Helpers;

namespace BankingControlPanel.Application.Services.Clients
{
    public interface IClientService
    {
        Task<DatatableFilterdDto<ClientPagedDto>> GetPagedAsync(DatatableFilterInput input);
        Task<List<string>> GetRecentSearchesAsync();
        Task<ClientDto> GetAsync(int id);
        Task<string> CreateAsync(CreateClientDto clientDto);
        Task<string> UpdateAsync(UpdateClientDto clientDto);
        Task<string> UpdateProfilePhoto(UpdateClientProfilePhoto input);
        Task<string> DeleteAsync(int id);
    }
}
