using Microsoft.Data.Sqlite;
using Libplanet.Crypto;
using Nekoyume.Blockchain;

namespace Lib9c.Policy.AccessControlService
{
    public class SQLiteAccessControlService : IAccessControlService
    {
        private string _connectionString;

        public SQLiteAccessControlService(string connectionString)
        {
            _connectionString = connectionString;
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS blocklist (address VARCHAR(42))";
            command.ExecuteNonQuery();
        }

        public bool IsAccessDenied(Address address)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                $"SELECT EXISTS(SELECT 1 FROM blocklist WHERE address='{address}')";

            var result = command.ExecuteScalar();

            return result is not null && (long) result == 1;
        }
    }
}
