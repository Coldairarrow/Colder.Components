namespace Colder.Common
{
    /// <summary>
    /// Api请求结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 代码，默认200表示成功，其余根据业务需求自行定义
        /// </summary>
        public int Code { get; set; } = 200;

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// Api请求结果
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
    }
}
