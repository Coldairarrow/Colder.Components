namespace Colder.Logging.Abstractions
{
    /// <summary>
    /// 文件日志配置
    /// </summary>
    public class FileOption : EnableOption
    {
        /// <summary>
        /// 保留天数
        /// </summary>
        public int RetainedFileDays { get; set; } = 30;
    }
}
