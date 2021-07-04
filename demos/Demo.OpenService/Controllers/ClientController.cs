using Colder.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Demo.OpenService.Controllers
{
    [Route("api/client")]
    public class ClientController : Controller
    {
        private readonly IHelloOpenService _helloOpenService;
        public ClientController(IHelloOpenService helloOpenService)
        {
            _helloOpenService = helloOpenService;
        }

        [Route("api-test")]
        [HttpGet]
        public async Task<string> ApiTest()
        {
            return await _helloOpenService.SayHello(new IdInput<string>
            {
                Id = Guid.NewGuid().ToString()
            });
        }
    }
}
