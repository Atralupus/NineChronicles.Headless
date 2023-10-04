using System;
using Nekoyume.Blockchain;

namespace NineChronicles.Headless.AccessControlService
{
    public static class AccessControlServiceFactory
    {
        public enum StorageType
        {
            /// <summary>
            /// Use Memory
            /// </summary>
            Memory,

            /// <summary>
            /// Use Redis
            /// </summary>
            Redis,

            /// <summary>
            /// Use SQLite
            /// </summary>
            SQLite
        }

        public static IAccessControlService CreateAccessControlService(
            StorageType storageType,
            string? connectionString = null,
            string? initialBlocklist = null
        )
        {
            return storageType switch
            {
                StorageType.Memory
                    => initialBlocklist is null
                        ? throw new ArgumentOutOfRangeException(
                            nameof(initialBlocklist),
                            initialBlocklist,
                            null
                        )
                        : new MemoryAccessControlService(initialBlocklist.Split(",")),

                StorageType.Redis
                    => connectionString is null
                        ? throw new ArgumentOutOfRangeException(
                            nameof(connectionString),
                            connectionString,
                            null
                        )
                        : new RedisAccessControlService(connectionString),

                StorageType.SQLite
                    => connectionString is null
                        ? throw new ArgumentOutOfRangeException(
                            nameof(connectionString),
                            connectionString,
                            null
                        )
                        : new SQLiteAccessControlService(connectionString),

                _ => throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null)
            };
        }
    }
}
