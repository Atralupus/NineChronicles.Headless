using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Libplanet.Crypto;

namespace NineChronicles.Headless.Services.AccessControlService
{
    public class SQLiteAccessControlService : IMutableAccessControlService
    {
        private const string CreateTableSql =
            "CREATE TABLE IF NOT EXISTS blocklist (address VARCHAR(42))";
        private const string CheckAccessSql =
            "SELECT EXISTS(SELECT 1 FROM blocklist WHERE address=@Address)";
        private const string DenyAccessSql =
            "INSERT OR IGNORE INTO blocklist (address) VALUES (@Address)";
        private const string AllowAccessSql = "DELETE FROM blocklist WHERE address=@Address";

        private readonly string _connectionString;

        public SQLiteAccessControlService(string connectionString)
        {
            _connectionString = connectionString;
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = CreateTableSql;
            command.ExecuteNonQuery();
        }

        public bool IsAccessDenied(Address address)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = CheckAccessSql;
            command.Parameters.AddWithValue("@Address", address.ToString());

            var result = command.ExecuteScalar();

            return result is not null && (long)result == 1;
        }

        public void DenyAccess(Address address)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = DenyAccessSql;
            command.Parameters.AddWithValue("@Address", address.ToString());
            command.ExecuteNonQuery();
        }

        public void AllowAccess(Address address)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = AllowAccessSql;
            command.Parameters.AddWithValue("@Address", address.ToString());
            command.ExecuteNonQuery();
        }

        public List<Address> ListBlockedAddresses(int offset, int limit)
        {
            var blockedAddresses = new List<Address>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT address FROM blocklist LIMIT @Limit OFFSET @Offset";
            command.Parameters.AddWithValue("@Limit", limit);
            command.Parameters.AddWithValue("@Offset", offset);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                blockedAddresses.Add(new Address(reader.GetString(0)));
            }

            return blockedAddresses;
        }
    }
}
