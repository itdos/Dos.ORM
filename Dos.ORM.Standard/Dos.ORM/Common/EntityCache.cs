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
using System.Collections.Generic;
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
        private static Dictionary<string, object> _entityList = new Dictionary<string, object>();

        /// <summary>
        /// lock object
        /// </summary>
        private static readonly object LockObj = new object();


        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void Reset()
        {
            _entityList.Clear();
        }

        /// <summary>
        /// 清理具体实体的缓存
        /// </summary>
        public static void Reset<TEntity>()
            where TEntity : Entity
        {
            var typestring = typeof(TEntity).ToString();
            if (_entityList.ContainsKey(typestring))
                _entityList.Remove(typestring);
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
        /// 返回用户名
        /// </summary>
        /// <returns></returns>
        public static string GetUserName<TEntity>()
            where TEntity : Entity
        {
            return getTEntity<TEntity>().GetUserName();
        }
        /// <summary>
        /// 返回T
        /// </summary>
        /// <returns></returns>
        private static TEntity getTEntity<TEntity>()
            where TEntity : Entity
        {
            var typestring = typeof(TEntity).ToString();

            if (_entityList.ContainsKey(typestring))
                return (TEntity)_entityList[typestring];

            lock (LockObj)
            {
                if (_entityList.ContainsKey(typestring))
                    return (TEntity)_entityList[typestring];

                var t = DataUtils.Create<TEntity>();
                _entityList.Add(typestring, t);
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
            var fields = GetFields<TEntity>();
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
