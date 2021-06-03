namespace Colder.CommonUtil
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public struct FileEntry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileBytes"></param>
        public FileEntry(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件字节
        /// </summary>
        public byte[] FileBytes { get; set; }
    }
}
