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
using System.Data.Common;
using System.Linq;
using System.Text;
using Dos.ORM.Common;
using Dos;
using Dos.ORM;

namespace Dos.ORM
{
    /// <summary>
    /// Command Creator
    /// </summary>
    public class CommandCreator
    {

        /// <summary>
        /// 
        /// </summary>
        private Database db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public CommandCreator(Database db)
        {
            this.db = db;
        }


        #region 更新

        /// <summary>
        /// 创建更新DbCommand
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DbCommand CreateUpdateCommand<TEntity>(TEntity entity, WhereClip where)
            where TEntity : Entity
        {
            var v11056 = entity.V1_10_5_6_Plus();
            if (v11056)
            {
                var fs = entity.GetModifyFieldsStr();
                if (null == fs || fs.Count == 0)
                    return null;
                var fields = new Field[fs.Count];
                var values = new object[fs.Count];
                var i = 0;
                var fields2 = entity.GetFields().ToList();
                var values2 = entity.GetValues();
                foreach (string f in fs)
                {
                    var index = fields2.FindIndex(d => d.Name == f);
                    fields[i] = fields2[index];
                    values[i] = values2[index];
                    i++;
                }
                return CreateUpdateCommand<TEntity>(fields, values, where);
            }
            else
            {
                var mfields = entity.GetModifyFields();
                if (null == mfields || mfields.Count == 0)
                    return null;
                var fields = new Field[mfields.Count];
                var values = new object[mfields.Count];
                var i = 0;
                foreach (ModifyField mf in mfields)
                {
                    fields[i] = mf.Field;
                    values[i] = mf.NewValue;
                    i++;
                }
                return CreateUpdateCommand<TEntity>(fields, values, where);
            }
        }

        /// <summary>
        /// 创建更新DbCommand
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DbCommand CreateUpdateCommand<TEntity>(Field[] fields, object[] values, WhereClip where)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            if (null == fields || fields.Length == 0 || null == values || values.Length == 0)
                return null;

            Check.Require(fields.Length == values.Length, "fields.Length must be equal values.Length");

            var length = fields.Length;

            if (WhereClip.IsNullOrEmpty(where))
                where = WhereClip.All;

            var sql = new StringBuilder();
            sql.Append("UPDATE ");
            sql.Append(db.DbProvider.BuildTableName(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>()));
            sql.Append(" SET ");

            var identityField = EntityCache.GetIdentityField<TEntity>();
            var identityExist = !Field.IsNullOrEmpty(identityField);
            var list = new List<Parameter>();
            var colums = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                if (identityExist)
                {
                    //标识列  排除
                    if (fields[i].PropertyName.Equals(identityField.PropertyName))
                        continue;
                }

                colums.Append(",");
                colums.Append(fields[i].FieldName);
                colums.Append("=");

                if (values[i] is Expression)
                {
                    var expression = (Expression)values[i];
                    colums.Append(expression);
                    list.AddRange(expression.Parameters);
                }
                else if (values[i] is Field)
                {
                    var fieldValue = (Field)values[i];
                    colums.Append(fieldValue.TableFieldName);
                }
                else
                {
                    var pname = DataUtils.MakeUniqueKey(fields[i]);
                    //var pname = string.Concat("@", fields[i].Name, i);
                    colums.Append(pname);
                    var p = new Parameter(pname, values[i], fields[i].ParameterDbType, fields[i].ParameterSize);
                    list.Add(p);
                }
            }
            sql.Append(colums.ToString().Substring(1));
            sql.Append(where.WhereString);
            list.AddRange(where.Parameters);

            var cmd = db.GetSqlStringCommand(sql.ToString());

            db.AddCommandParameter(cmd, list.ToArray());
            return cmd;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 创建删除DbCommand
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="userName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DbCommand CreateDeleteCommand(string tableName, string userName, WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(where))
                throw new Exception("请传入删除条件，删除整表数据请使用.DeleteAll<T>()方法。");
            //where = WhereClip.All; //2015-08-08

            StringBuilder sql = new StringBuilder();
            sql.Append("DELETE FROM ");
            sql.Append(db.DbProvider.BuildTableName(tableName, userName));
            sql.Append(where.WhereString);
            DbCommand cmd = db.GetSqlStringCommand(sql.ToString());
            db.AddCommandParameter(cmd, where.Parameters.ToArray());

            return cmd;
        }

        /// <summary>
        /// 创建删除DbCommand
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public DbCommand CreateDeleteCommand<TEntity>(WhereClip where)
             where TEntity : Entity
        {
            return CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>(), where);
        }

        #endregion

        #region 添加

        /// <summary>
        /// 创建添加DbCommand
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DbCommand CreateInsertCommand<TEntity>(Field[] fields, object[] values)
            where TEntity : Entity
        {
            Check.Require(!EntityCache.IsReadOnly<TEntity>(), string.Concat("Entity(", EntityCache.GetTableName<TEntity>(), ") is readonly!"));

            if (null == fields || fields.Length == 0 || null == values || values.Length == 0)
                return null;

            var sql = new StringBuilder();
            sql.Append("INSERT INTO ");
            sql.Append(db.DbProvider.BuildTableName(EntityCache.GetTableName<TEntity>(), EntityCache.GetUserName<TEntity>()));
            sql.Append(" (");

            var identityField = EntityCache.GetIdentityField<TEntity>();
            var identityExist = !Field.IsNullOrEmpty(identityField);
            var isSequence = false;

            if (db.DbProvider is Dos.ORM.Oracle.OracleProvider)
            {
                if (!string.IsNullOrEmpty(EntityCache.GetSequence<TEntity>()))
                    isSequence = true;
            }

            var insertFields = new Dictionary<string, string>();
            var parameters = new List<Parameter>();

            var length = fields.Length;
            for (var i = 0; i < length; i++)
            {
                if (identityExist)
                {
                    if (fields[i].PropertyName.Equals(identityField.PropertyName))
                    {
                        if (isSequence)
                        {
                            insertFields.Add(fields[i].FieldName, string.Concat(EntityCache.GetSequence<TEntity>(), ".nextval"));
                        }
                        continue;
                    }
                }
                string panme = DataUtils.MakeUniqueKey(fields[i]);
                //var panme = string.Concat("@", fields[i].Name, i);
                insertFields.Add(fields[i].FieldName, panme);
                var p = new Parameter(panme, values[i], fields[i].ParameterDbType, fields[i].ParameterSize);
                parameters.Add(p);
            }
            var fs = new StringBuilder();
            var ps = new StringBuilder();

            foreach (var kv in insertFields)
            {
                fs.Append(",");
                fs.Append(kv.Key);

                ps.Append(",");
                ps.Append(kv.Value);
            }

            sql.Append(fs.ToString().Substring(1));
            sql.Append(") VALUES (");
            sql.Append(ps.ToString().Substring(1));
            sql.Append(")");

            var cmd = db.GetSqlStringCommand(sql.ToString());

            db.AddCommandParameter(cmd, parameters.ToArray());
            return cmd;
        }

        /// <summary>
        /// 创建添加DbCommand
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DbCommand CreateInsertCommand<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            if (null == entity)
                return null;
            var v11056 = entity.V1_10_5_6_Plus();
            if (v11056)
            {
                var fs = entity.GetModifyFieldsStr();
                if (null == fs || fs.Count == 0)
                    return CreateInsertCommand<TEntity>(entity.GetFields(), entity.GetValues());

                var fields = new Field[fs.Count];
                var values = new object[fs.Count];
                var i = 0;
                var fields2 = entity.GetFields().ToList();
                var values2 = entity.GetValues();
                foreach (string f in fs)
                {
                    var index = fields2.FindIndex(d => d.Name == f);
                    fields[i] = fields2[index];
                    values[i] = values2[index];
                    i++;
                }
                return CreateInsertCommand<TEntity>(fields, values);
            }
            else
            {
                var mfields = entity.GetModifyFields();

                if (null == mfields || mfields.Count == 0)
                {
                    return CreateInsertCommand<TEntity>(entity.GetFields(), entity.GetValues());
                }
                else
                {
                    List<Field> fields = new List<Field>();
                    List<object> values = new List<object>();
                    foreach (ModifyField m in mfields)
                    {
                        fields.Add(m.Field);
                        values.Add(m.NewValue);
                    }

                    return CreateInsertCommand<TEntity>(fields.ToArray(), values.ToArray());
                }
            }
        }

        #endregion
    }
}
