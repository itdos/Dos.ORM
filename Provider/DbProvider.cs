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
using System.Text;
using System.Data;
using System.Data.Common;
using Dos.ORM;
using Dos.ORM.Common;

namespace Dos.ORM
{

    /// <summary>
    /// DbProvider
    /// </summary>
    public abstract class DbProvider
    {


        #region Protected Members
        /// <summary>
        /// like符号。 --- 2015-09-07
        /// </summary>
        protected char likeToken;
        /// <summary>
        /// 【
        /// </summary>
        protected char leftToken;
        /// <summary>
        /// 参数前缀
        /// </summary>
        protected char paramPrefixToken;

        /// <summary>
        /// 】
        /// </summary>
        protected char rightToken;

        /// <summary>
        /// The db provider factory.
        /// </summary>
        protected System.Data.Common.DbProviderFactory dbProviderFactory;
        /// <summary>
        /// The db connection string builder
        /// </summary>
        protected DbConnectionStringBuilder dbConnStrBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DbProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The conn STR.</param>
        /// <param name="dbProviderFactory">The db provider factory.</param>
        /// <param name="leftToken">leftToken</param>
        /// <param name="paramPrefixToken">paramPrefixToken</param>
        /// <param name="rightToken">rightToken</param>
        protected DbProvider(string connectionString, DbProviderFactory dbProviderFactory, char leftToken, char rightToken, char paramPrefixToken)
        {
            dbConnStrBuilder = new DbConnectionStringBuilder();
            dbConnStrBuilder.ConnectionString = connectionString;
            this.dbProviderFactory = dbProviderFactory;
            this.leftToken = leftToken;
            this.rightToken = rightToken;
            this.paramPrefixToken = paramPrefixToken;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DbProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The conn STR.</param>
        /// <param name="dbProviderFactory">The db provider factory.</param>
        /// <param name="leftToken">leftToken</param>
        /// <param name="paramPrefixToken">paramPrefixToken</param>
        /// <param name="rightToken">rightToken</param>
        /// <param name="likeToken">likeToken</param>
        protected DbProvider(string connectionString, DbProviderFactory dbProviderFactory, char leftToken, char rightToken, char paramPrefixToken, char likeToken = '%')
        {
            dbConnStrBuilder = new DbConnectionStringBuilder();
            dbConnStrBuilder.ConnectionString = connectionString;
            this.dbProviderFactory = dbProviderFactory;
            this.leftToken = leftToken;
            this.rightToken = rightToken;
            this.paramPrefixToken = paramPrefixToken;
            this.likeToken = likeToken;
        }
        #endregion


        #region Properties
        //2015-08-12新增
        /// <summary>
        /// 
        /// </summary>
        private DatabaseType databaseType;
        //2015-08-12新增
        /// <summary>
        /// ConnectionStrings 节点名称
        /// </summary>
        public DatabaseType DatabaseType
        {
            get { return databaseType; }
            set { databaseType = value; }
        }

        private string connectionStringsName;

        /// <summary>
        /// ConnectionStrings 节点名称
        /// </summary>
        public string ConnectionStringsName
        {
            get { return connectionStringsName; }
            set { connectionStringsName = value; }
        }


        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                return dbConnStrBuilder.ConnectionString;
            }
        }

        /// <summary>
        /// Gets the db provider factory.
        /// </summary>
        /// <value>The db provider factory.</value>
        public System.Data.Common.DbProviderFactory DbProviderFactory
        {
            get
            {
                return dbProviderFactory;
            }
        }

        /// <summary>
        /// Gets the param prefix.
        /// </summary>
        /// <value>The param prefix.</value>
        public char ParamPrefix { get { return paramPrefixToken; } }

        /// <summary>
        /// Gets the left token of table name or column name.
        /// </summary>
        /// <value>The left token.</value>
        public char LeftToken { get { return leftToken; } }

        /// <summary>
        /// Gets the right token of table name or column name.
        /// </summary>
        /// <value>The right token.</value>
        public char RightToken { get { return rightToken; } }

        #endregion



        /// <summary>
        /// 缓存
        /// </summary>
        private CacheConfiguration cacheConfig;

        /// <summary>
        /// 缓存配置
        /// </summary>
        public CacheConfiguration CacheConfig
        {
            get { return cacheConfig; }
            set { cacheConfig = value; }
        }

        /// <summary>
        /// 实体缓存
        /// </summary>
        private Dictionary<string, CacheInfo> entitiesCache = new Dictionary<string, CacheInfo>();

        /// <summary>
        /// 实体缓存
        /// </summary>
        public Dictionary<string, CacheInfo> EntitiesCache
        {
            get { return entitiesCache; }
            set { entitiesCache = value; }
        }


        /// <summary>
        /// 自增长字段查询语句
        /// </summary>
        public abstract string RowAutoID
        {
            get;
        }

        /// <summary>
        /// 是否支持批量sql提交
        /// </summary>
        public abstract bool SupportBatch
        {
            get;
        }

        /// <summary>
        /// Builds the name of the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual string BuildParameterName(string name)
        {
            string nameStr = name.Trim(leftToken, rightToken);
            if (nameStr[0] != paramPrefixToken)
            {
                if ("@?:".Contains(nameStr[0].ToString()))
                {
                    return nameStr.Substring(1).Insert(0, new string(paramPrefixToken, 1));
                }
                else {
                    return nameStr.Insert(0, new string(paramPrefixToken, 1));
                }
            }
            return nameStr;
        }

        /// <summary>
        /// Builds the name of the table.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual string BuildTableName(string name)
        {
            return string.Concat(leftToken.ToString(), name.Trim(leftToken, rightToken), rightToken.ToString());
        }


        /// <summary>
        /// 创建分页查询
        /// </summary>
        /// <param name="fromSection"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public virtual FromSection CreatePageFromSection(FromSection fromSection, int startIndex, int endIndex)
        {
            Check.Require(startIndex, "startIndex", Check.GreaterThanOrEqual<int>(1));
            Check.Require(endIndex, "endIndex", Check.GreaterThanOrEqual<int>(1));
            Check.Require(startIndex <= endIndex, "startIndex must be less than endIndex!");
            Check.Require(fromSection, "fromSection", Check.NotNullOrEmpty);



            int pageSize = endIndex - startIndex + 1;
            if (startIndex == 1)
            {
                fromSection.PrefixString = string.Concat(" TOP ", pageSize.ToString());
            }
            else
            {

                if (OrderByClip.IsNullOrEmpty(fromSection.OrderByClip))
                {
                    foreach (Field f in fromSection.Fields)
                    {
                        if (!f.PropertyName.Equals("*") && f.PropertyName.IndexOf('(') == -1)
                        {
                            fromSection.OrderBy(f.Asc);
                            break;
                        }
                    }
                }



                Check.Require(!OrderByClip.IsNullOrEmpty(fromSection.OrderByClip), "query.OrderByClip could not be null or empty!");

                int count = fromSection.Count(fromSection);

                List<Parameter> list = fromSection.Parameters;

                if (endIndex > count)
                {
                    int lastnumber = count - startIndex + 1;
                    if (startIndex > count)
                        lastnumber = count % pageSize;

                    fromSection.PrefixString = string.Concat(" TOP ", lastnumber.ToString());

                    fromSection.OrderBy(fromSection.OrderByClip.ReverseOrderByClip);

                    //

                    fromSection.TableName = string.Concat(" (", fromSection.SqlString, ") AS temp_table ");

                    fromSection.PrefixString = string.Empty;

                    fromSection.DistinctString = string.Empty;

                    fromSection.GroupBy(GroupByClip.None);

                    fromSection.Select(Field.All);

                    fromSection.OrderBy(fromSection.OrderByClip.ReverseOrderByClip);

                    fromSection.Where(WhereClip.All);

                }
                else
                {

                    if (startIndex < count / 2)
                    {

                        fromSection.PrefixString = string.Concat(" TOP ", endIndex.ToString());

                        fromSection.TableName = string.Concat(" (", fromSection.SqlString, ") AS tempIntable ");

                        fromSection.PrefixString = string.Concat(" TOP ", pageSize.ToString());

                        fromSection.DistinctString = string.Empty;

                        fromSection.GroupBy(GroupByClip.None);

                        fromSection.Select(Field.All);

                        fromSection.OrderBy(fromSection.OrderByClip.ReverseOrderByClip);

                        fromSection.Where(WhereClip.All);

                        //

                        fromSection.TableName = string.Concat(" (", fromSection.SqlString, ") AS tempOuttable ");

                        fromSection.PrefixString = string.Empty;

                        fromSection.OrderBy(fromSection.OrderByClip.ReverseOrderByClip);
                    }
                    else
                    {
                        fromSection.PrefixString = string.Concat(" TOP ", (count - startIndex + 1).ToString());

                        fromSection.OrderBy(fromSection.OrderByClip.ReverseOrderByClip);

                        fromSection.TableName = string.Concat(" (", fromSection.SqlString, ") AS tempIntable ");

                        fromSection.PrefixString = string.Concat(" TOP ", pageSize.ToString());

                        fromSection.DistinctString = string.Empty;

                        fromSection.GroupBy(GroupByClip.None);

                        fromSection.Select(Field.All);

                        fromSection.OrderBy(fromSection.OrderByClip.ReverseOrderByClip);

                        fromSection.Where(WhereClip.All);
                    }

                }

                fromSection.Parameters = list;

            }

            return fromSection;

        }

        /// <summary>
        /// 调整DbCommand命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual void PrepareCommand(DbCommand cmd)
        {
            bool isStoredProcedure = (cmd.CommandType == CommandType.StoredProcedure);
            if (!isStoredProcedure)
            {
                cmd.CommandText = DataUtils.FormatSQL(cmd.CommandText, leftToken, rightToken);
            }

            foreach (DbParameter p in cmd.Parameters)
            {
                
                if (!isStoredProcedure)
                {
                    //TODO 这里可以继续优化
                    if (cmd.CommandText.IndexOf(p.ParameterName, StringComparison.Ordinal) == -1)
                    {
                        //2015-08-11修改
                        cmd.CommandText = cmd.CommandText.Replace("@"+p.ParameterName.Substring(1), p.ParameterName);
                        cmd.CommandText = cmd.CommandText.Replace("?"+p.ParameterName.Substring(1), p.ParameterName);
                        cmd.CommandText = cmd.CommandText.Replace(":"+p.ParameterName.Substring(1), p.ParameterName);
                        //if (p.ParameterName.Substring(0, 1) == "?" || p.ParameterName.Substring(0, 1) == ":"
                        //        || p.ParameterName.Substring(0, 1) == "@")
                        //    cmd.CommandText = cmd.CommandText.Replace(paramPrefixToken + p.ParameterName.Substring(1), p.ParameterName);
                        //else
                        //    cmd.CommandText = cmd.CommandText.Replace(p.ParameterName.Substring(1), p.ParameterName);
                    }
                }

                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                {
                    continue;
                }

                object value = p.Value;
                DbType dbType = p.DbType;

                if (value == DBNull.Value)
                {
                    continue;
                }

                if (value == null)
                {
                    p.Value = DBNull.Value;
                    continue;
                }

                Type type = value.GetType();

                if (type.IsEnum)
                {
                    p.DbType = DbType.Int32;
                    p.Value = Convert.ToInt32(value);
                    continue;
                }

                if (dbType == DbType.Guid && type != typeof(Guid))
                {
                    p.Value = new Guid(value.ToString());
                    continue;
                }
                #region 2015-09-08注释
                ////2015-09-07 写
                //var v = value.ToString();
                //if (DatabaseType == DatabaseType.MsAccess
                //    && (dbType == DbType.AnsiString || dbType == DbType.String)
                //    && !string.IsNullOrWhiteSpace(v)
                //    && cmd.CommandText.ToLower()
                //    .IndexOf("like " + p.ParameterName.ToLower(), StringComparison.Ordinal) > -1)
                //{
                //    if (v[0] == '%')
                //    {
                //        v = "*" + v.Substring(1);
                //    }
                //    if (v[v.Length-1] == '%')
                //    {
                //        v = v.TrimEnd('%') + "*";
                //    }
                //    p.Value = v;
                //}
                #endregion
                //if ((dbType == DbType.AnsiString || dbType == DbType.String ||
                //    dbType == DbType.AnsiStringFixedLength || dbType == DbType.StringFixedLength) && (!(value is string)))
                //{
                //    p.Value = SerializationManager.Serialize(value);
                //    continue;
                //}

                if (type == typeof(Boolean))
                {
                    p.Value = (((bool)value) ? 1 : 0);
                    continue;
                }
            }
        }
    }
}
