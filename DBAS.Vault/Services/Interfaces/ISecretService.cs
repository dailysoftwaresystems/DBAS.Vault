﻿using DBAS.Vault.Models;

namespace DBAS.Vault.Services.Interfaces
{
    public interface ISecretService
    {
        public const string DEFAULT_RECOVERY_LEVEL = "Recoverable+Purgeable";

        public Task<SecretResponse> Get(string? accountName, string name, string? version);

        public Task<SecretResponse> Create(string? accountName, string name, SecretRequest request);

        public Task<SecretResponse> Delete(string? accountName, string name);
    }
}
