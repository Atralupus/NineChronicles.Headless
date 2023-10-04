using System.Collections.Generic;
using System.Linq;
using Libplanet.Crypto;
using Nekoyume.Blockchain;

namespace NineChronicles.Headless.AccessControlService
{
    public class MemoryAccessControlService : IAccessControlService, IMutableAccessControlService
    {
        private HashSet<Address> _blocklist = new HashSet<Address>();

        public MemoryAccessControlService(IEnumerable<string> initialBlocklist)
        {
            foreach (var address in initialBlocklist)
            {
                _blocklist.Add(new Address(address));
            }
        }

        public bool IsAccessDenied(Address address)
        {
            return _blocklist.Contains(address);
        }

        public void DenyAccess(Address address)
        {
            if (!_blocklist.Contains(address))
            {
                _blocklist.Add(address);
            }
        }

        public void AllowAccess(Address address)
        {
            if (_blocklist.Contains(address))
            {
                _blocklist.Remove(address);
            }
        }

        public List<Address> ListBlockedAddresses(int offset, int limit)
        {
            return _blocklist.ToList().Skip(offset).Take(limit).ToList();
        }
    }
}
