/*************************************************************************
 * 
 * Hxj.Data
 * 
 * 2010-2-10
 * 
 * steven hu   
 *  
 * Support: http://www.cnblogs.com/huxj
 *   
 * 
 * Change History:
 * 
 * 
**************************************************************************/

namespace Dos.ORM
{
    public class CacheInfo
    {
        /// <summary>
        /// 过期时间 单位：秒
        /// </summary>
        private int? timeout;

        /// <summary>
        /// 过期时间 单位：秒
        /// </summary>
        public int? TimeOut
        {
            get { return timeout; }
            set { timeout = value; }
        }


        /// <summary>
        /// 文件路径
        /// </summary>
        private string filePath;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }


        /// <summary>
        /// 判断该cache是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsNullOrEmpty()
        {
            if (timeout.HasValue || !string.IsNullOrEmpty(filePath))
                return false;

            return true;
        }
    }
}
