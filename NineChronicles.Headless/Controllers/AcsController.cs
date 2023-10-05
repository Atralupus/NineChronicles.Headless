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

        public AccessControlController(IMutableAccessControlService accessControlService)
        {
            _accessControlService = accessControlService;
        }

        [HttpGet("entries/{address}")]
        public ActionResult<bool> IsAccessDenied(string address)
        {
            return _accessControlService.IsAccessDenied(new Address(address));
        }

        [HttpPost("entries/{address}/deny")]
        public ActionResult DenyAccess(string address)
        {
            _accessControlService.DenyAccess(new Address(address));
            return Ok();
        }

        [HttpPost("entries/{address}/allow")]
        public ActionResult AllowAccess(string address)
        {
            _accessControlService.AllowAccess(new Address(address));
            return Ok();
        }

        [HttpGet("entries")]
        public ActionResult<List<Address>> ListBlockedAddresses(int offset, int limit)
        {
            return _accessControlService.ListBlockedAddresses(offset, limit);
        }
    }
}
