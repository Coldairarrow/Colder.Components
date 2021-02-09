using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Demo.Logging.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMessageBus _messageBus;
        public HomeController(ILogger<HomeController> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task Index()
        {
            await _messageBus.Publish(new RequestMessage
            {
                Text = "小明"
            });
        }
    }
}
