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

using System.Collections.Generic;
using Dos.ORM.Common;

namespace Dos.ORM
{
    /// <summary>
    /// 实体信息缓存
    /// </summary>
    public class EntityCache
    {
        /// <summary>
        /// 保存实体列表
        /// </summary>
        private static Dictionary<string, object> entityList = new Dictionary<string, object>();

        /// <summary>
        /// lock object
        /// </summary>
        private static object lockObj = new object();


        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void Reset()
        {
            entityList.Clear();
        }

        /// <summary>
        /// 清理具体实体的缓存
        /// </summary>
        public static void Reset<TEntity>()
            where TEntity : Entity
        {
            string typestring = typeof(TEntity).ToString();
            if (entityList.ContainsKey(typestring))
                entityList.Remove(typestring);
        }

        /// <summary>
        /// 返回表名
        /// </summary>
        /// <returns></returns>
        public static string GetTableName<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().GetTableName();
        }


        /// <summary>
        /// 返回T
        /// </summary>
        /// <returns></returns>
        private static TEntity getTEntity<TEntity>()
            where TEntity : Entity
        {
            string typestring = typeof(TEntity).ToString();

            if (entityList.ContainsKey(typestring))
                return (TEntity)entityList[typestring];

            lock (lockObj)
            {
                if (entityList.ContainsKey(typestring))
                    return (TEntity)entityList[typestring];

                TEntity t = DataUtils.Create<TEntity>();
                entityList.Add(typestring, t);
                return t;
            }
        }


        /// <summary>
        /// 获取主键字段
        /// </summary>
        /// <returns></returns>
        public static Field[] GetPrimaryKeyFields<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().GetPrimaryKeyFields();
        }

        /// <summary>
        /// 返回所有字段
        /// </summary>
        /// <returns></returns>
        public static Field[] GetFields<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().GetFields();
        }


        /// <summary>
        /// 返回第一个字段
        /// </summary>
        /// <returns></returns>
        public static Field GetFirstField<TEntity>()
            where TEntity : Entity
        {
            Field[] fields = GetFields<TEntity>();
            if (null != fields && fields.Length > 0)
                return fields[0];
            return null;
        }


        /// <summary>
        /// 返回标识字段
        /// </summary>
        /// <returns></returns>
        public static Field GetIdentityField<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().GetIdentityField();
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        /// <returns></returns>
        public static bool IsReadOnly<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().IsReadOnly();
        }


        /// <summary>
        /// 标识列的名称（Oracle）
        /// </summary>
        /// <returns></returns>
        public static string GetSequence<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().GetSequence();
        }
    }
}
