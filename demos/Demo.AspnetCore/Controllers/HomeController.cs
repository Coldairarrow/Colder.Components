using Colder.Common;
using Microsoft.AspNetCore.Mvc;

namespace Demo.AspnetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpPost]
        [Route("test")]
        public string Test(IdInput<string> id)
        {
            return id.Id;
        }
    }
}
