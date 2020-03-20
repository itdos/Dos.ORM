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

using System;
using System.Collections.Generic;
using System.Linq;
using Dos.ORM.Common;
using Dos;
using Dos.ORM;
using Dos.ORM.Common;

namespace Dos.ORM
{
    /// <summary>
    /// 批处理
    /// </summary>
    public class DbBatch : IDisposable
    {
        /// <summary>
        /// DbCommand生成器
        /// </summary>
        private CommandCreator cmdCreator;

        /// <summary>
        /// 批处理
        /// </summary>
        private BatchCommander batchcmd;


        /// <summary>
        /// 是否已关闭
        /// </summary>
        private bool isClose = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="batchcmd"></param>
        /// <param name="cmdCreator"></param>
        public DbBatch(CommandCreator cmdCreator, BatchCommander batchcmd)
        {
            Check.Require(cmdCreator, "cmdCreator", Check.NotNull);
            Check.Require(batchcmd, "batchcmd", Check.NotNull);

            this.cmdCreator = cmdCreator;

            this.batchcmd = batchcmd;


        }


        /// <summary>
        /// 立即执行已挂起的批处理
        /// </summary>
        public void Execute()
        {
            batchcmd.ExecuteBatch();
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (isClose)
                return;

            batchcmd.Close();
            isClose = true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }


        #region 更新

        /// <summary>
        /// 更新全部字段  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void UpdateAll<TEntity>(params TEntity[] entities)
            where TEntity : Entity
        {
            if (null == entities || entities.Length == 0)
                return;

            foreach (TEntity entity in entities)
            {
                UpdateAll<TEntity>(entity);
            }
        }



        /// <summary>
        /// 更新全部字段  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void UpdateAll<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            if (entity == null)
                return;

            WhereClip where = DataUtils.GetPrimaryKeyWhere(entity);

            Check.Require(!WhereClip.IsNullOrEmpty(where), "entity must have the primarykey!");

            UpdateAll<TEntity>(entity, where);
        }



        /// <summary>
        /// 更新全部字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public void UpdateAll<TEntity>(TEntity entity, WhereClip where)
            where TEntity : Entity
        {
            if (entity == null)
                return;

            batchcmd.Process(cmdCreator.CreateUpdateCommand<TEntity>(entity.GetFields(), entity.GetValues(), where));
        }


        /// <summary>
        /// 更新  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void Update<TEntity>(params TEntity[] entities)
            where TEntity : Entity
        {
            if (null == entities || entities.Length == 0)
                return;

            foreach (TEntity entity in entities)
            {
                Update<TEntity>(entity);
            }
        }



        /// <summary>
        /// 更新  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void Update<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            if (!entity.IsModify())
                return;

            var where = DataUtils.GetPrimaryKeyWhere(entity);

            Check.Require(!WhereClip.IsNullOrEmpty(where), "entity must have the primarykey!");

            Update<TEntity>(entity, where);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Update<TEntity>(TEntity entity, WhereClip where)
            where TEntity : Entity
        {
            if (!entity.IsModify())
                return;

            batchcmd.Process(cmdCreator.CreateUpdateCommand<TEntity>(entity, where));
        }


        /// <summary>
        /// 更新单个值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Update<TEntity>(Field field, object value, WhereClip where)
            where TEntity : Entity
        {
            if (Field.IsNullOrEmpty(field))
                return;

            batchcmd.Process(cmdCreator.CreateUpdateCommand<TEntity>(new Field[] { field }, new object[] { value }, where));
        }




        /// <summary>
        /// 更新多个值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldValue"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Update<TEntity>(Dictionary<Field, object> fieldValue, WhereClip where)
              where TEntity : Entity
        {
            if (null == fieldValue || fieldValue.Count == 0)
                return;

            Field[] fields = new Field[fieldValue.Count];
            object[] values = new object[fieldValue.Count];

            int i = 0;

            foreach (KeyValuePair<Field, object> kv in fieldValue)
            {
                fields[i] = kv.Key;
                values[i] = kv.Value;

                i++;
            }

            batchcmd.Process(cmdCreator.CreateUpdateCommand<TEntity>(fields, values, where));
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Update<TEntity>(Field[] fields, object[] values, WhereClip where)
            where TEntity : Entity
        {
            if (null == fields || fields.Length == 0)
                return;

            batchcmd.Process(cmdCreator.CreateUpdateCommand<TEntity>(fields, values, where));
        }



        #endregion


        #region 删除

        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Delete<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            WhereClip where = DataUtils.GetPrimaryKeyWhere(entity);

            Check.Require(!WhereClip.IsNullOrEmpty(where), "entity must have the primarykey!");
            Delete<TEntity>(where);
            //2015-08-20注释
            //Delete<TEntity>(entity, where);
        }


        //2015-08-20注释
        ///// <summary>
        /////  删除
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="pkValues"></param>
        ///// <returns></returns>
        //public void Delete<TEntity>(params object[] pkValues)
        //    where TEntity : Entity
        //{
        //    Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

        //    batchcmd.Process(cmdCreator.CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), DataUtils.GetPrimaryKeyWhere<TEntity>(pkValues)));
        //}

        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public void Delete<TEntity>(params string[] pkValues)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            batchcmd.Process(cmdCreator.CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>(), DataUtils.GetPrimaryKeyWhere<TEntity>(pkValues.ToArray())));
        }
        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public void Delete<TEntity>(params int[] pkValues)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            batchcmd.Process(cmdCreator.CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>(), DataUtils.GetPrimaryKeyWhere<TEntity>(pkValues.ToArray())));
        }
        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public void Delete<TEntity>(params long[] pkValues)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            batchcmd.Process(cmdCreator.CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>(), DataUtils.GetPrimaryKeyWhere<TEntity>(pkValues.ToArray())));
        }
        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public void Delete<TEntity>(params Guid[] pkValues)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            batchcmd.Process(cmdCreator.CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>(), DataUtils.GetPrimaryKeyWhere<TEntity>(pkValues.ToArray())));
        }
        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete<TEntity>(WhereClip where)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            batchcmd.Process(cmdCreator.CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>(), where));
        }

        #endregion


        #region 添加

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void Insert<TEntity>(params TEntity[] entities)
            where TEntity : Entity
        {
            if (null == entities || entities.Length == 0)
                return;

            foreach (TEntity entity in entities)
            {
                Insert<TEntity>(entity);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Insert<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            batchcmd.Process(cmdCreator.CreateInsertCommand<TEntity>(entity));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public void Insert<TEntity>(Field[] fields, object[] values)
            where TEntity : Entity
        {
            batchcmd.Process(cmdCreator.CreateInsertCommand<TEntity>(fields, values));
        }



        #endregion
    }
}
