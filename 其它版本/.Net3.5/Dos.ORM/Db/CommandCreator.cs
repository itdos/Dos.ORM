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

            List<ModifyField> mfields = entity.GetModifyFields();

            if (null == mfields || mfields.Count == 0)
                return null;

            Field[] fields = new Field[mfields.Count];
            object[] values = new object[mfields.Count];

            int i = 0;

            foreach (ModifyField mf in mfields)
            {
                fields[i] = mf.Field;
                values[i] = mf.NewValue;
                i++;
            }

            return CreateUpdateCommand<TEntity>(fields, values, where);

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

            int length = fields.Length;

            if (WhereClip.IsNullOrEmpty(where))
                where = WhereClip.All;

            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE ");
            sql.Append(db.DbProvider.BuildTableName(EntityCache.GetTableName<TEntity>()));
            sql.Append(" SET ");

            Field identityField = EntityCache.GetIdentityField<TEntity>();
            bool identityExist = !Field.IsNullOrEmpty(identityField);
            List<Parameter> list = new List<Parameter>();
            StringBuilder colums = new StringBuilder();
            for (int i = 0; i < length; i++)
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
                    Expression expression = (Expression)values[i];
                    colums.Append(expression.ToString());
                    list.AddRange(expression.Parameters);
                }
                else if (values[i] is Field)
                {
                    Field fieldValue = (Field)values[i];
                    colums.Append(fieldValue.TableFieldName);
                }
                else
                {
                    string pname = DataUtils.MakeUniqueKey(fields[i]);
                    //string pname = fields[i].tableName + fields[i].Name + i;
                    colums.Append(pname);
                    Parameter p = new Parameter(pname, values[i], fields[i].ParameterDbType, fields[i].ParameterSize);
                    list.Add(p);
                }
            }
            sql.Append(colums.ToString().Substring(1));
            sql.Append(where.WhereString);
            list.AddRange(where.Parameters);

            DbCommand cmd = db.GetSqlStringCommand(sql.ToString());

            db.AddCommandParameter(cmd, list.ToArray());
            return cmd;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 创建删除DbCommand
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DbCommand CreateDeleteCommand(string tableName, WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(where))
                throw new Exception("请传入删除条件，删除整表数据请使用.DeleteAll<T>()方法。");
                //where = WhereClip.All; //2015-08-08

            StringBuilder sql = new StringBuilder();
            sql.Append("DELETE FROM ");
            sql.Append(db.DbProvider.BuildTableName(tableName));
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
            return CreateDeleteCommand(EntityCache.GetTableName<TEntity>(), where);
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

            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO ");
            sql.Append(db.DbProvider.BuildTableName(EntityCache.GetTableName<TEntity>()));
            sql.Append(" (");

            Field identityField = EntityCache.GetIdentityField<TEntity>();
            bool identityExist = !Field.IsNullOrEmpty(identityField);
            bool isSequence = false;

            if (db.DbProvider is Dos.ORM.Oracle.OracleProvider)
            {
                if (!string.IsNullOrEmpty(EntityCache.GetSequence<TEntity>()))
                    isSequence = true;
            }

            Dictionary<string, string> insertFields = new Dictionary<string, string>();
            List<Parameter> parameters = new List<Parameter>();

            int length = fields.Length;
            for (int i = 0; i < length; i++)
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
                //string panme = fields[i].tableName + fields[i].Name + i;
                insertFields.Add(fields[i].FieldName, panme);
                Parameter p = new Parameter(panme, values[i], fields[i].ParameterDbType, fields[i].ParameterSize);
                parameters.Add(p);
            }
            StringBuilder fs = new StringBuilder();
            StringBuilder ps = new StringBuilder();

            foreach (KeyValuePair<string, string> kv in insertFields)
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

            DbCommand cmd = db.GetSqlStringCommand(sql.ToString());

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


            List<ModifyField> mfields = entity.GetModifyFields();

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

        #endregion
    }
}
