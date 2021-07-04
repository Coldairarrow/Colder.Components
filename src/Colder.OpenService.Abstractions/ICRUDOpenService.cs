using Colder.Common;
using System.Threading.Tasks;

namespace Colder.OpenService.Abstractions
{
    /// <summary>
    /// CRUD通用接口
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TInfoDto">InfoDto</typeparam>
    /// <typeparam name="TOutputDto">OutputDto</typeparam>
    /// <typeparam name="TQueryParamsDto">QueryParamsDto</typeparam>
    public interface ICRUDOpenService<TKey, TInfoDto, TOutputDto, TQueryParamsDto> where TQueryParamsDto : new()
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [Route("get-pagelist")]
        Task<PageList<TOutputDto>> GetList(PageInput<TQueryParamsDto> input);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [Route("get-list")]
        Task<TOutputDto[]> GetList(TQueryParamsDto input);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        [Route("get")]
        Task<TOutputDto> Get(IdInput<TKey> id);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data">数据</param>
        [Route("create")]
        Task<TInfoDto> Create(TInfoDto data);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [Route("update")]
        Task<TInfoDto> Update(TInfoDto data);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="ids">主键列表</param>
        /// <returns></returns>
        [Route("delete")]
        Task Delete(params TKey[] ids);
    }
}
