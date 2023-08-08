using Colder.Common;
using Colder.DistributedId;
using Microsoft.AspNetCore.Mvc;

namespace Demo.AspnetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "default")]
    public class HomeController : ControllerBase
    {
        private readonly IDistributedId _distributedId;
        public HomeController(IDistributedId distributedId)
        {
            _distributedId = distributedId;
        }

        [HttpPost]
        [Route("test")]
        //[AllowAnonymous]
        public string Test(IdInput<string> id)
        {
            return _distributedId.NewLongId().ToString();
        }
    }
}
