using DBAS.Vault.Controllers.Interfaces;
using DBAS.Vault.Models;
using DBAS.Vault.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBAS.Vault.Controllers
{
    public class SecretController : BaseController
    {
        private readonly ISecretService Service;

        public SecretController(APIEnvironment environment, ISecretService service) : base(environment)
        {
            Service = service;
        }

        [HttpGet("secrets/{name}/{version?}")]
        public async Task<SecretResponse> Get(string name, string? version)
        {
            string? accountName = GetAccountName();
            return await Service.Get(accountName, name, version);
        }

        [HttpPut("secrets/{name}")]
        public async Task<SecretResponse> Create(string name, [FromBody] SecretRequest request)
        {
            string? accountName = GetAccountName();
            return await Service.Create(accountName, name, request);
        }

        [HttpDelete("secrets/{name}")]
        public async Task<SecretResponse> Delete(string name)
        {
            string? accountName = GetAccountName();
            return await Service.Delete(accountName, name);
        }
    }
}
