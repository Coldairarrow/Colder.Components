using System;

namespace Colder.Common
{
    /// <summary>
    /// 业务异常
    /// 注:并不会当作真正的异常处理,仅为方便返回前端错误提示信息
    /// </summary>
    public class MsgException : Exception
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MsgException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">错误信息</param>
        public MsgException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="code">错误代码</param>
        public MsgException(string message, int code)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="innerException"></param>
        public MsgException(string message, int code, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }
}
