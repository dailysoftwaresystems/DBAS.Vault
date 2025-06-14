﻿using DBAS.Vault.Database.Interfaces;
using DBAS.Vault.Jwt.Interfaces;
using DBAS.Vault.Models;
using DBAS.Vault.Properties;
using DBAS.Vault.Services.Interfaces;

namespace DBAS.Vault.Services
{
    public class AccountService : IAccountService
    {
        private readonly IDatabase Database;
        private readonly IEncryption Encryption;

        public AccountService(IDatabase db, IEncryption encryption)
        {
            Database = db;
            Encryption = encryption;
        }

        public async Task<Account> Get(int? accountId)
        {
            if (accountId == null)
            {
                throw new InvalidDataException(string.Format(Strings.Service_AccountNotFound, Strings.NullRepresentation));
            }

            using var cmd = Database.CreateCommand(Strings.AccountService_Get);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.AccountId), accountId));

            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read())
            {
                throw new InvalidDataException(string.Format(Strings.Service_AccountNotFound, accountId));
            }

            return new Account()
            {
                AccountId = reader.GetInt32(0),
                Name = reader.GetString(1),
                TenantId = reader.GetGuid(2),
                ClientId = reader.GetGuid(3),
                ClientSecret = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                DeletedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
            };
        }

        public async Task<Account> GetByName(string accountName)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                return null;
            }

            using var cmd = Database.CreateCommand(Strings.AccountService_GetByName);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.Name), accountName));

            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read())
            {
                return null;
            }

            return new Account()
            {
                AccountId = reader.GetInt32(0),
                Name = reader.GetString(1),
                TenantId = reader.GetGuid(2),
                ClientId = reader.GetGuid(3),
                ClientSecret = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                DeletedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
            };
        }

        public async Task<Account> Create(Account account)
        {
            bool validSecret = !string.IsNullOrEmpty(account.ClientSecret) && Encryption.IsTokenValid(account.ClientSecret);
            account.TenantId = Guid.NewGuid();
            account.ClientId = Guid.NewGuid();
            account.ClientSecret = validSecret ? account.ClientSecret : Guid.NewGuid().ToString();

            using var cmd = Database.CreateCommand(Strings.AccountService_Create);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.Name), account.Name));
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.TenantId), account.TenantId));
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.ClientId), account.ClientId));
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.ClientSecret), account.ClientSecret));

            await cmd.ExecuteNonQueryAsync();
            var id = (int) (await Database.GetLastId());
            
            if (!validSecret)
            {
                string clientSecret = Encryption.GenerateToken(id, account.Name, account.TenantId.Value, account.ClientId.Value);

                cmd.CommandText = Strings.AccountService_UpdateClientSecret;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(Database.GetParameter(nameof(Account.ClientSecret), clientSecret));
                cmd.Parameters.Add(Database.GetParameter(nameof(Account.AccountId), id));
                await cmd.ExecuteNonQueryAsync();
            }

            account = await Get(id);
            return account;
        }

        public async Task Delete(int? accountId)
        {
            await Get(accountId);

            using var cmd = Database.CreateCommand(Strings.AccountService_Delete);
            cmd.Parameters.Add(Database.GetParameter(nameof(Account.AccountId), accountId));
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = Strings.AccountService_DeleteAccountSecrets;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
