﻿using System.Data.Common;

namespace DBAS.Vault.Database.Interfaces
{
    public interface IDatabase : IDisposable
    {
        public Task<DbConnection> OpenConnection(bool mainDb = true);

        public DbConnection GetConnection();

        public DbCommand CreateCommand(string query);

        public DbParameter GetParameter(string name, object value);

        public Task<int?> GetLastId();

        public bool HasTransaction();

        public Task BeginTransaction();

        public Task Commit();

        public Task Rollback();

        public bool IsHealthy();
    }
}
