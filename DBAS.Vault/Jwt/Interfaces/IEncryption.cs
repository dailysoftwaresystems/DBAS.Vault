﻿namespace DBAS.Vault.Jwt.Interfaces
{
    public interface IEncryption
	{
		public string GenerateToken(int accountId, string accountName, Guid tenantId, Guid clientId);

        public bool IsTokenValid(string token);

        public string Encrypt(string value);

        public string Decrypt(string value);

    }
}
