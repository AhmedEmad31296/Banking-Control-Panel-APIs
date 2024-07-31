using BankingControlPanel.Application.Services.Clients.Dto;
using BankingControlPanel.Domain;
using BankingControlPanel.Domain.Entities;
using BankingControlPanel.Domain.Exceptions;
using BankingControlPanel.Domain.Helpers;
using BankingControlPanel.Domain.Heplers;
using BankingControlPanel.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic.Core;

namespace BankingControlPanel.Application.Services.Clients
{


    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<BankAccount> _bankAccountRepository;
        private readonly IRepository<FilteredClientsHistory> _searchHistoryRepository;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        readonly string _baseUrl = "";
        public ClientService(IRepository<Client> clientRepository,
            IRepository<FilteredClientsHistory> searchHistoryRepository,
            IRepository<Address> addressRepository,
            IRepository<BankAccount> bankAccountRepository,
            IHostEnvironment hostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientRepository = clientRepository;
            _addressRepository = addressRepository;
            _bankAccountRepository = bankAccountRepository;
            _searchHistoryRepository = searchHistoryRepository;
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            _baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";

        }
        public async Task<DatatableFilterdDto<ClientPagedDto>> GetPagedAsync(DatatableFilterInput input)
        {
            IQueryable<Client> query = _clientRepository.GetAllAsNoTracking();
            int totalCount = await query.CountAsync();

            // Apply filtering
            var searchTerm = input.SearchTerm?.ToLower();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b =>
                    b.FirstName.ToLower().Contains(searchTerm) ||
                    b.LastName.ToLower().Contains(searchTerm) ||
                    b.Email.ToLower().Contains(searchTerm) ||
                    b.MobileNumber.ToLower().Contains(searchTerm));

                await AddSearchTermAsync(new FilteredClientsHistoryDto
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchTerm = searchTerm,
                    SortDir = input.SortDirection,
                    SortCol = input.SortColumn
                });
            }

            int recordsFiltered = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(input.SortColumn) && !string.IsNullOrEmpty(input.SortDirection))
            {
                query = query.OrderBy(string.Concat(input.SortColumn, " ", input.SortDirection));
            }

            // Pagination
            List<ClientPagedDto> clients = await query
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(c => new ClientPagedDto
                {
                    Id = c.Id,
                    Email = c.Email,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    PersonalId = c.PersonalId,
                    MobileNumber = c.MobileNumber,
                    Sex = c.Sex,
                }).ToListAsync();

            return new DatatableFilterdDto<ClientPagedDto>
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = totalCount,
                AaData = clients,
                Draw = input.Draw
            };

        }
        public async Task<List<string>> GetRecentSearchesAsync()
        {
            return await _searchHistoryRepository.GetAllAsNoTracking()
                .OrderByDescending(x => x.SearchDate)
                .Select(x => x.SearchTerm)
                .Take(3)
                .ToListAsync();
        }
        public async Task<ClientDto> GetAsync(int id)
        {
            var client = await _clientRepository.GetAll()
                                                .Where(x => x.Id == id)
                                                .Include(x => x.Addresses)
                                                .Include(x => x.BankAccounts)
                                                .FirstOrDefaultAsync();
            if (client == null) throw new KeyNotFoundException("Client Not Found");

            var path = Path.Combine(_baseUrl, BankingControlPanalConsts.ClientProfilePath.FolderPath);
            var profilePhotoUrl = !string.IsNullOrEmpty(client.ProfilePhotoUrl)
                                    ? $"{_baseUrl}/{BankingControlPanalConsts.ClientProfilePath.FolderPath}/{client.ProfilePhotoUrl}"
                                    : string.Empty;
            return new ClientDto
            {
                Id = client.Id,
                Email = client.Email,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PersonalId = client.PersonalId,
                ProfilePhotoUrl = profilePhotoUrl,
                MobileNumber = client.MobileNumber,
                Sex = client.Sex,
                Addresses = client.Addresses?.Select(a => new AddressDto
                {
                    Country = a.Country,
                    City = a.City,
                    Street = a.Street,
                    ZipCode = a.ZipCode
                }).ToList() ?? new List<AddressDto>(),
                BankAccounts = client.BankAccounts?.Select(a => new BankAccountDto
                {
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance
                }).ToList() ?? new List<BankAccountDto>()
            };
        }
        public async Task<string> CreateAsync(CreateClientDto input)
        {
            // Check if email already exists
            bool existingEmail = await _clientRepository.GetAll()
                .Where(b => b.Email.Equals(input.Email))
                .AnyAsync();

            if (existingEmail)
                throw new EmailAlreadyExistsException();

            // Check if mobile number already exists
            bool existingPhone = await _clientRepository.GetAll()
                .Where(b => b.MobileNumber.Equals(input.MobileNumber))
                .AnyAsync();

            if (existingPhone)
                throw new MobileNumberAlreadyExistsException();

            // Check if personalId already exists
            bool existingPersonalId = await _clientRepository.GetAll()
                .Where(b => b.PersonalId.Equals(input.PersonalId))
                .AnyAsync();

            if (existingPersonalId)
                throw new PersonalIdAlreadyExistsException();

            // Ensure at least one bank account is provided
            if (!input.BankAccounts.Any())
                throw new RequiredAtLeastOneBankAccountException();

            // Check if any bank account number already exists
            foreach (var bankAccount in input.BankAccounts)
            {
                bool existingBankAccount = await _clientRepository.GetAll()
                    .AnyAsync(c => c.BankAccounts.Any(ba => ba.AccountNumber == bankAccount.AccountNumber));

                if (existingBankAccount)
                    throw new BankAccountAlreadyExistsException();
            }
            var client = new Client
            {
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                PersonalId = input.PersonalId,
                MobileNumber = input.MobileNumber,
                Sex = input.Sex,
                Addresses = input.Addresses?.Select(a => new Address
                {
                    Country = a.Country,
                    City = a.City,
                    Street = a.Street,
                    ZipCode = a.ZipCode
                }).ToList() ?? new List<Address>(),
                BankAccounts = input.BankAccounts.Select(a => new BankAccount
                {
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance
                }).ToList()
            };

            // Profile photo upload
            if (input.ProfilePhoto != null)
            {
                var path = Path.Combine(_hostEnvironment.ContentRootPath, BankingControlPanalConsts.ClientProfilePath.UploadPath);
                var uploadedPhoto = await MediaFileService.UploadMediaFileAsync(input.ProfilePhoto, path);

                if (uploadedPhoto != null)
                {
                    client.ProfilePhotoUrl = uploadedPhoto.FileName;
                }
            }

            await _clientRepository.AddAsync(client);

            return "Client Created Successfully";
        }
        public async Task<string> UpdateAsync(UpdateClientDto input)
        {
            // Retrieve the existing client
            var client = await _clientRepository.GetAll()
                                                .Where(x => x.Id == input.Id)
                                                .Include(x => x.Addresses)
                                                .Include(x => x.BankAccounts)
                                                .FirstOrDefaultAsync();
            if (client == null)
                throw new KeyNotFoundException("Client Not Found");

            // Check for existing email 
            var emailExists = await _clientRepository.GetAll()
                .AnyAsync(b => b.Id != input.Id && b.Email == input.Email);

            if (emailExists)
                throw new EmailAlreadyExistsException();

            // Check for existing mobile number
            var mobileExists = await _clientRepository.GetAll()
                .AnyAsync(b => b.Id != input.Id && b.MobileNumber == input.MobileNumber);

            if (mobileExists)
                throw new MobileNumberAlreadyExistsException();

            // Check if personalId already exists
            bool existingPersonalId = await _clientRepository.GetAll()
                .Where(b => b.Id != input.Id && b.PersonalId.Equals(input.PersonalId))
                .AnyAsync();

            if (existingPersonalId)
                throw new PersonalIdAlreadyExistsException();

            // Ensure at least one bank account is provided
            if (!input.BankAccounts.Any())
                throw new RequiredAtLeastOneBankAccountException();

            // Check if any bank account number already exists
            foreach (var bankAccount in input.BankAccounts)
            {
                bool existingBankAccount = await _clientRepository.GetAll()
                    .AnyAsync(c => c.BankAccounts.Any(ba => ba.AccountNumber == bankAccount.AccountNumber && ba.ClientId != input.Id));

                if (existingBankAccount)
                    throw new BankAccountAlreadyExistsException();
            }
            // Update client details
            client.Email = input.Email;
            client.FirstName = input.FirstName;
            client.LastName = input.LastName;
            client.PersonalId = input.PersonalId;
            client.MobileNumber = input.MobileNumber;
            client.Sex = input.Sex;

            // Remove existing addresses and accounts from the database
            if (client.Addresses.Any())
            {
                await _addressRepository.DeleteRangeAsync(client.Addresses);
            }
            if (client.BankAccounts.Any())
            {
                await _bankAccountRepository.DeleteRangeAsync(client.BankAccounts);
            }

            // Clear existing addresses and accounts
            client.Addresses.Clear();
            client.BankAccounts.Clear();

            // Add new addresses
            if (input.Addresses.Any())
                foreach (var addressDto in input.Addresses)
                {
                    client.Addresses.Add(new Address
                    {
                        Country = addressDto.Country,
                        City = addressDto.City,
                        Street = addressDto.Street,
                        ZipCode = addressDto.ZipCode
                    });
                }

            // Add new bank accounts
            foreach (var accountDto in input.BankAccounts)
            {
                client.BankAccounts.Add(new BankAccount
                {
                    AccountNumber = accountDto.AccountNumber,
                    Balance = accountDto.Balance
                });
            }

            // Update client in the repository
            await _clientRepository.UpdateAsync(client);

            return "Client Updated Successfully";
        }
        public async Task<string> UpdateProfilePhoto(UpdateClientProfilePhoto input)
        {
            // Retrieve the client
            var client = await _clientRepository.GetByIdAsync(input.Id);
            if (client == null)
                throw new KeyNotFoundException("Client Not Found");

            var uploadPath = Path.Combine(_hostEnvironment.ContentRootPath, BankingControlPanalConsts.ClientProfilePath.UploadPath);

            // Handle file upload if provided
            if (input.ProfilePhoto != null)
            {
                // Upload the new profile photo
                var uploadedPhoto = await MediaFileService.UploadMediaFileAsync(input.ProfilePhoto, uploadPath);
                if (uploadedPhoto != null)
                {
                    // If there was an existing photo, delete it
                    if (!string.IsNullOrEmpty(client.ProfilePhotoUrl))
                    {
                        var existingPhotoPath = Path.Combine(uploadPath, client.ProfilePhotoUrl);
                        MediaFileService.DeleteMediaFile(client.ProfilePhotoUrl, uploadPath);
                    }

                    // Update the client with the new photo URL
                    client.ProfilePhotoUrl = uploadedPhoto.FileName;
                }
                else
                {
                    throw new Exception("Failed to upload the profile photo.");
                }
            }
            else
            {
                // If no photo is provided, remove the existing photo
                if (!string.IsNullOrEmpty(client.ProfilePhotoUrl))
                {
                    var existingPhotoPath = Path.Combine(uploadPath, client.ProfilePhotoUrl);
                    MediaFileService.DeleteMediaFile(client.ProfilePhotoUrl, uploadPath);
                    client.ProfilePhotoUrl = string.Empty;
                }
            }

            // Update client in the repository
            await _clientRepository.UpdateAsync(client);

            return "Client Profile Photo Updated Successfully";
        }
        public async Task<string> DeleteAsync(int id)
        {
            // Retrieve the client from the repository
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                throw new KeyNotFoundException("Client not found");

            // If the client has a profile photo, delete the associated file
            if (!string.IsNullOrEmpty(client.ProfilePhotoUrl))
            {
                var path = Path.Combine(_hostEnvironment.ContentRootPath, BankingControlPanalConsts.ClientProfilePath.UploadPath);
                MediaFileService.DeleteMediaFile(client.ProfilePhotoUrl, path);
            }
            // Remove existing addresses and accounts from the database
            if (client.Addresses.Any())
                await _addressRepository.DeleteRangeAsync(client.Addresses);

            if (client.BankAccounts.Any())
                await _bankAccountRepository.DeleteRangeAsync(client.BankAccounts);

            // Delete the client from the repository
            await _clientRepository.DeleteAsync(id);

            return "Client Deleted successfully";

        }

        async Task AddSearchTermAsync(FilteredClientsHistoryDto input)
        {
            FilteredClientsHistory filtered = new()
            {
                SearchTerm = input.SearchTerm,
                Page = input.Page,
                PageSize = input.PageSize,
                SearchDate = DateTime.Now,
                SortCol = input.SortCol,
                SortDir = input.SortDir,
            };
            await _searchHistoryRepository.AddAsync(filtered);
        }
    }
}