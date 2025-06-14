﻿using DBAS.Vault.Helpers;
using DBAS.Vault.Properties;

namespace DBAS.Vault.Database
{
    public class Migrator : IDisposable
    {
        private MySqlDb MySql;

        public Migrator(MySqlDb mysql)
        {
            MySql = mysql;
        }

        private void CreateDatabase()
        {
            using var connection = MySql.OpenConnection(false).Result;
            using var command = connection.CreateCommand();

            command.CommandText = string.Format(Strings.CreateDatabaseString, Strings.DatabaseName);
            command.Connection = connection;
            command.ExecuteNonQuery();
        }

        public void Migrate()
        {
            CreateDatabase();

            using var connection = MySql.OpenConnection().Result;
            using var command = connection.CreateCommand();

            command.CommandText = EnvironmentHelper.ReadResource(Strings.DatabaseScriptFile);
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            MySql?.Dispose();
            MySql = null;

            GC.SuppressFinalize(this);
        }
    }
}
