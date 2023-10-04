using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NineChronicles.Headless.Services.AccessControlService;
using Libplanet.Crypto;

namespace NineChronicles.Headless.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessControlController : ControllerBase
    {
        private readonly IMutableAccessControlService _accessControlService;

        public AccessControlController(
            AccessControlServiceFactory.StorageType serviceType,
            string? connectionString = null,
            string? initialBlocklist = null
        )
        {
            _accessControlService = AccessControlServiceFactory.CreateAccessControlService(
                serviceType,
                connectionString,
                initialBlocklist
            );
        }

        [HttpGet("entries/{address}")]
        public ActionResult<bool> IsAccessDenied(string address)
        {
            return _accessControlService.IsAccessDenied(new Address(address));
        }

        [HttpPost("entries/{address}/deny")]
        public ActionResult DenyAccess(string address)
        {
            if (_accessControlService is IMutableAccessControlService mutableService)
            {
                mutableService.DenyAccess(new Address(address));
                return Ok();
            }
            return BadRequest("Service does not support this operation.");
        }

        [HttpPost("entries/{address}/allow")]
        public ActionResult AllowAccess(string address)
        {
            if (_accessControlService is IMutableAccessControlService mutableService)
            {
                mutableService.AllowAccess(new Address(address));
                return Ok();
            }
            return BadRequest("Service does not support this operation.");
        }

        [HttpGet("entries")]
        public ActionResult<List<Address>> ListBlockedAddresses(int offset, int limit)
        {
            if (_accessControlService is IMutableAccessControlService mutableService)
            {
                return mutableService.ListBlockedAddresses(offset, limit);
            }
            return BadRequest("Service does not support this operation.");
        }
    }
}
