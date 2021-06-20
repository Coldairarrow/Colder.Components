using Microsoft.AspNetCore.Mvc;

namespace Colder.OpenService.Hosting
{
    /// <summary>
    /// OpenService服务基类,具体服务实现必须继承此类
    /// </summary>
    [Route("")]
    [ApiController]
    public class OpenServiceBase : ControllerBase
    {

    }
}
