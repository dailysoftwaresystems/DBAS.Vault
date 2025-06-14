﻿using DBAS.Vault.Models;

namespace DBAS.Vault.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Account> Get(int? accountId);
        
        public Task<Account> GetByName(string accountName);

        public Task<Account> Create(Account account);

        public Task Delete(int? accountId);
    }
}
