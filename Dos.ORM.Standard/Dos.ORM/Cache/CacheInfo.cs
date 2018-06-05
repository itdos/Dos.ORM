#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) ITdos
* CLR 版本: 4.0.30319.18408
* 创 建 人：steven hu
* 电子邮箱：
* 官方网站：www.ITdos.com
* 创建日期：2010-2-10
* 文件描述：
******************************************************
* 修 改 人：ITdos
* 修改日期：
* 备注描述：
*******************************************************/
#endregion

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
