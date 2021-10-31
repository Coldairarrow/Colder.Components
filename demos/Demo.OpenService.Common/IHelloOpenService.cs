using Colder.Common;
using Colder.OpenService.Abstractions;
using System.Threading.Tasks;

namespace Demo.OpenService.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Route("hello")]
    public interface IHelloOpenService : IOpenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInput"></param>
        /// <returns></returns>
        [Route("say")]
        Task<string> SayHello(IdInput<string> idInput);
    }
}
