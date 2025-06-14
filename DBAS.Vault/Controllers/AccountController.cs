using DBAS.Vault.Controllers.Interfaces;
using DBAS.Vault.Models;
using DBAS.Vault.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBAS.Vault.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService Service;

        public AccountController(APIEnvironment environment, IAccountService service) : base(environment)
        {
            Service = service;
        }

        [HttpGet("accounts")]
        public async Task<Account> Get()
        {
            int? accountId = GetAccountId();
            return await Service.Get(accountId);
        }

        [AllowAnonymous]
        [HttpPost("accounts")]
        public async Task<Account> Create([FromBody] Account account)
        {
            return await Service.Create(account);
        }

        [HttpDelete("accounts")]
        public async Task Delete()
        {
            int? accountId = GetAccountId();
            await Service.Delete(accountId);
        }
    }
}
