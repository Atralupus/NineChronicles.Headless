using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using Libplanet.Crypto;
using Nekoyume.Blockchain;

namespace NineChronicles.Headless.AccessControlService
{
    public class RedisAccessControlService : IAccessControlService, IMutableAccessControlService
    {
        private IDatabase _db;

        public RedisAccessControlService(string storageUri)
        {
            var redis = ConnectionMultiplexer.Connect(storageUri);
            _db = redis.GetDatabase();
        }

        public bool IsAccessDenied(Address address)
        {
            return _db.KeyExists(address.ToString());
        }

        public void DenyAccess(Address address)
        {
            _db.StringSet(address.ToString(), "denied");
        }

        public void AllowAccess(Address address)
        {
            _db.KeyDelete(address.ToString());
        }

        public List<Address> ListBlockedAddresses(int offset, int limit)
        {
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
            var keys = server
                .Keys(database: _db.Database, pattern: "*")
                .Select(k => new Address(k.ToString()))
                .ToList();

            return keys.Skip(offset).Take(limit).ToList();
        }
    }
}
