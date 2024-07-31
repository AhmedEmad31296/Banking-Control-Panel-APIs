using BankingControlPanel.Application.Services.Clients.Dto;
using BankingControlPanel.Application.Services.Clients;
using BankingControlPanel.Domain.Exceptions;
using BankingControlPanel.Domain.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet("GetPaged")]
        public async Task<IActionResult> GetPaged([FromQuery] DatatableFilterInput input)
        {
            try
            {
                var result = await _clientService.GetPagedAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged clients");
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpGet("GetRecentSearches")]
        public async Task<IActionResult> GetRecentSearches()
        {
            var recentSearches = await _clientService.GetRecentSearchesAsync();
            return Ok(recentSearches);
        }
        
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var client = await _clientService.GetAsync(id);
                if (client == null) return NotFound("Client not found");

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting client with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreateClientDto input)
        {
            try
            {
                var result = await _clientService.CreateAsync(input);
                return Ok(result);
            }
            catch (EmailAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Email already exists");
                return BadRequest("Email already exists");
            }
            catch (MobileNumberAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Mobile number already exists");
                return BadRequest("Mobile number already exists");
            }
            catch (PersonalIdAlreadyExistsException ex)
            {
                _logger.LogError(ex, "PersonalId already exists");
                return BadRequest("PersonalId already exists");
            }
            catch (RequiredAtLeastOneBankAccountException ex)
            {
                _logger.LogError(ex, "At least one account is required");
                return BadRequest("At least one account is required");
            } 
            catch (BankAccountAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Bank account already exists");
                return BadRequest("Bank account already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating client");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] UpdateClientDto input)
        {
            try
            {
                var result = await _clientService.UpdateAsync(input);
                return Ok(result);
            }
            catch (EmailAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Email already exists");
                return BadRequest("Email already exists");
            }
            catch (MobileNumberAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Mobile number already exists");
                return BadRequest("Mobile number already exists");
            }
            catch (PersonalIdAlreadyExistsException ex)
            {
                _logger.LogError(ex, "PersonalId already exists");
                return BadRequest("PersonalId already exists");
            }
            catch (RequiredAtLeastOneBankAccountException ex)
            {
                _logger.LogError(ex, "At least one account is required");
                return BadRequest("At least one account is required");
            }
            catch (BankAccountAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Bank account already exists");
                return BadRequest("Bank account already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating client");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("UpdateProfilePhoto")]
        public async Task<IActionResult> UpdateProfilePhoto([FromForm] UpdateClientProfilePhoto input)
        {
            try
            {
                var result = await _clientService.UpdateProfilePhoto(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile photo");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _clientService.DeleteAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"Client with ID {id} not found");
                return NotFound("Client not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting client with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
