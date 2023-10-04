using System.Collections.Generic;
using Libplanet.Crypto;
using Nekoyume.Blockchain;

namespace NineChronicles.Headless.AccessControlService
{
    public class MemoryAccessControlService : IAccessControlService
    {
        private HashSet<string> _blocklist = new HashSet<string>();

        public MemoryAccessControlService(IEnumerable<string> initialBlocklist)
        {
            foreach (var address in initialBlocklist)
            {
                _blocklist.Add(address);
            }
        }

        public bool IsAccessDenied(Address address)
        {
            return _blocklist.Contains(address.ToString());
        }
    }
}
