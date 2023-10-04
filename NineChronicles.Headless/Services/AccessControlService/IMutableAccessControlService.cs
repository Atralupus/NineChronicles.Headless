using Libplanet.Crypto;
using System.Collections.Generic;
using Nekoyume.Blockchain;

namespace NineChronicles.Headless.Services.AccessControlService
{
    public interface IMutableAccessControlService : IAccessControlService
    {
        void DenyAccess(Address address);
        void AllowAccess(Address address);
        List<Address> ListBlockedAddresses(int offset, int limit);
    }
}
