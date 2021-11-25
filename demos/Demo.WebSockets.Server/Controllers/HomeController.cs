using Colder.WebSockets.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Demo.WebSockets.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IWebSocketServer _webSocketServer;
        public HomeController(IWebSocketServer webSocketServer)
        {
            _webSocketServer = webSocketServer;
        }

        [HttpGet]
        [Route("/send")]
        public async Task Send()
        {
            foreach (var aConnection in _webSocketServer.GetAllConnections())
            {
                await aConnection.Send(DateTime.Now.ToString());
            }
        }
    }
}
