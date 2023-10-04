using Libplanet.Crypto;
using System.Collections.Generic;

namespace NineChronicles.Headless.AccessControlService
{
    public interface IMutableAccessControlService
    {
        void DenyAccess(Address address);
        void AllowAccess(Address address);
        List<Address> ListBlockedAddresses(int offset, int limit);
    }
}
