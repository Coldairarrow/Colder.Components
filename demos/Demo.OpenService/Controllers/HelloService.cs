using Colder.CommonUtil.Primitives;
using Colder.OpenService.Hosting;
using System.Threading.Tasks;

namespace Demo.OpenService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HelloService : OpenServiceBase, IHelloOpenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInput"></param>
        /// <returns></returns>
        public Task<string> SayHello(IdInput<string> idInput)
        {
            return Task.FromResult(idInput.Id);
        }
    }
}
