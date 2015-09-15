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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Dos.ORM.Common;
using System.Web;
using System.Web.Caching;


namespace Dos.ORM
{

    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="T"></typeparam>    
    public class FromSection<T> : FromSection
        where T : Entity
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database"></param>
        public FromSection(Database database)
            : this(database, (DbTransaction)null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database"></param>
        /// <param name="trans"></param>
        public FromSection(Database database, DbTransaction trans)
            : base(database, database.DbProvider.BuildTableName(EntityCache.GetTableName<T>()), trans)
        {

        }

        #region 连接  join

        /// <summary>
        /// Inner Join。Lambda写法：.InnerJoin&lt;Model2>((d1,d2) => d1.ID == d2.ID)
        /// </summary>
        public FromSection<T> InnerJoin<TEntity>(WhereClip where)
             where TEntity : Entity
        {
            return join(EntityCache.GetTableName<TEntity>(), where, JoinType.InnerJoin);
        }
        
        /// <summary>
        /// Cross Join
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection<T> CrossJoin<TEntity>(WhereClip where)
            where TEntity : Entity
        {
            return join(EntityCache.GetTableName<TEntity>(), where, JoinType.CrossJoin);
        }
        /// <summary>
        /// Right Join
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection<T> RightJoin<TEntity>(WhereClip where)
            where TEntity : Entity
        {
            return join(EntityCache.GetTableName<TEntity>(), where, JoinType.RightJoin);
        }

        /// <summary>
        /// Left Join。经典写法：Model1._.ID == Model2._.ID
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection<T> LeftJoin<TEntity>(WhereClip where)
             where TEntity : Entity
        {
            return join(EntityCache.GetTableName<TEntity>(), where, JoinType.LeftJoin);
        }
        /// <summary>
        /// Full Join
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection<T> FullJoin<TEntity>(WhereClip where)
            where TEntity : Entity
        {
            return join(EntityCache.GetTableName<TEntity>(), where, JoinType.FullJoin);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        private new FromSection<T> join(string tableName, WhereClip where, JoinType joinType)
        {
            return (FromSection<T>)base.join(tableName, where, joinType);

            //if (string.IsNullOrEmpty(tableName) || WhereClip.IsNullOrEmpty(where))
            //    return this;

            //tableName = dbProvider.BuildTableName(tableName);

            //if (!joins.ContainsKey(tableName))
            //{
            //    string joinString = string.Empty;
            //    switch (joinType)
            //    {
            //        case JoinType.InnerJoin:
            //            joinString = "INNER JOIN";
            //            break;
            //        case JoinType.LeftJoin:
            //            joinString = "LEFT OUTER JOIN";
            //            break;
            //        case JoinType.RightJoin:
            //            joinString = "RIGHT OUTER JOIN";
            //            break;
            //        case JoinType.CrossJoin:
            //            joinString = "CROSS JOIN";
            //            break;
            //        case JoinType.FullJoin:
            //            joinString = "FULL OUTER JOIN";
            //            break;
            //        default:
            //            joinString = "INNER JOIN";
            //            break;
            //    }

            //    joins.Add(tableName, new KeyValuePair<string, WhereClip>(joinString, where));
            //}

            //return this;
        }

        #endregion

        #region 私有方法


        /// <summary>
        ///  设置默认主键排序 
        /// </summary>
        private void setPrimarykeyOrderby()
        {

            Field[] primarykeys = EntityCache.GetPrimaryKeyFields<T>();

            OrderByClip temporderBy;

            if (null != primarykeys && primarykeys.Length > 0)
            {
                temporderBy = new OrderByClip(primarykeys[0]);
            }
            else
            {
                temporderBy = new OrderByClip(EntityCache.GetFirstField<T>());
            }

            OrderBy(temporderBy);
        }

        #endregion

        #region 操作


        /// <summary>
        /// Having 
        /// </summary>
        public new FromSection<T> Having(WhereClip havingWhere)
        {
            return (FromSection<T>)base.Having(havingWhere);
        }
        /// <summary>
        /// whereclip
        /// </summary>
        public new FromSection<T> Where(WhereClip where)
        {
            return (FromSection<T>)base.Where(where);
        }
        /// <summary>
        /// groupby
        /// </summary>
        public new FromSection<T> GroupBy(GroupByClip groupBy)
        {
            return (FromSection<T>)base.GroupBy(groupBy);
        }
        /// <summary>
        /// groupby
        /// </summary>
        public new FromSection<T> GroupBy(params Field[] fields)
        {
            return (FromSection<T>)base.GroupBy(fields);
        }
        #region 2015-09-08新增
        /// <summary>
        /// 
        /// </summary>
        public new FromSection<T> OrderBy(params Field[] f)
        {
            var gb = OrderByClip.None;
            foreach (var field in f)
            {
                gb = gb && field.Asc;
            }
            return (FromSection<T>)base.OrderBy(gb);
        }
        /// <summary>
        /// 
        /// </summary>
        public new FromSection<T> OrderByDescending(params Field[] f)
        {
            var gb = OrderByClip.None;
            foreach (var field in f)
            {
                gb = gb && field.Desc;
            }
            return (FromSection<T>)base.OrderBy(gb);
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public new FromSection<T> OrderBy(OrderByClip orderBy)
        {
            return (FromSection<T>)base.OrderBy(orderBy);
        }
        /// <summary>
        /// orderby
        /// </summary>
        public new FromSection<T> OrderBy(params OrderByClip[] orderBys)
        {
            return (FromSection<T>)base.OrderBy(orderBys);
        }
        /// <summary>
        /// select field
        /// </summary>
        public new FromSection<T> Select(params Field[] fields)
        {
            return (FromSection<T>)base.Select(fields);
        }
        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        public new FromSection<T> Distinct()
        {
            return (FromSection<T>)base.Distinct();
        }

        /// <summary>
        /// Top
        /// </summary>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public new FromSection<T> Top(int topCount)
        {
            return From(1, topCount);
        }


        /// <summary>
        /// Page
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public new FromSection<T> Page(int pageSize, int pageIndex)
        {
            return From(pageSize * (pageIndex - 1) + 1, pageIndex * pageSize);
        }


        /// <summary>
        /// 设置默认排序
        /// </summary>
        private void setDefaultOrderby()
        {
            if (OrderByClip.IsNullOrEmpty(this.OrderByClip))
            {
                if (fields.Count > 0)
                {
                    foreach (Field f in fields)
                    {
                        if (f.PropertyName.Trim().Equals("*"))
                        {
                            setPrimarykeyOrderby();
                            break;
                        }
                    }

                }
                else
                {
                    setPrimarykeyOrderby();
                }
            }
        }

        /// <summary>
        /// From  1-10
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public new FromSection<T> From(int startIndex, int endIndex)
        {
            if (startIndex > 1)
            {
                setDefaultOrderby();
            }

            return (FromSection<T>)base.From(startIndex, endIndex);
            //return (FromSection<T>)dbProvider.CreatePageFromSection(this, startIndex, endIndex);
        }


        /// <summary>
        /// 设置缓存有效期  单位：秒
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public new FromSection<T> SetCacheTimeOut(int timeout)
        {
            this.timeout = timeout;
            return this;
        }

        /// <summary>
        /// 设置缓存依赖
        /// </summary>
        /// <param name="dep"></param>
        /// <returns></returns>
        public new FromSection<T> SetCacheDependency(CacheDependency dep)
        {
            this.cacheDep = dep;
            return this;
        }


        /// <summary>
        /// 重新加载
        /// </summary>
        /// <returns></returns>
        public new FromSection<T> Refresh()
        {
            isRefresh = true;
            return this;
        }


        /// <summary>
        /// select sql
        /// </summary>
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public new FromSection<T> AddSelect(FromSection fromSection)
        {
            return AddSelect(fromSection, null);
        }

        /// <summary>
        /// select sql
        /// </summary>
        /// <param name="fromSection"></param>
        /// <param name="aliasName">别名</param>
        /// <returns></returns>
        public new FromSection<T> AddSelect(FromSection fromSection, string aliasName)
        {
            return (FromSection<T>)base.AddSelect(fromSection, aliasName);
        }

        #endregion

        #region 查询
        private readonly string[] notClass = new string[] { "String" };
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public List<TResult> ToList<TResult>()
        {
            var typet = typeof(TResult);
            if (typet == typeof(T))
            {
                return ToList() as List<TResult>;
            }
            FromSection from = getPagedFromSection();
            if (typet.IsClass && !((IList) notClass).Contains(typet.Name))
            {
                string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "List", "|",
                    formatSql(from.SqlString, from));
                object cacheValue = getCache(cacheKey);

                if (null != cacheValue)
                {
                    return (List<TResult>)cacheValue;
                }
                List<TResult> list = new List<TResult>();
                using (IDataReader reader = ToDataReader(from))
                {
                    list = EntityUtils.Mapper.Map<TResult>(reader);
                    reader.Close();
                }
                setCache<List<TResult>>(list, cacheKey);
                return list;
            }
            if (@from.Fields.Count == 0)
            {
                throw new Exception(".ToList<" + typet.Name + ">()至少需要.Select()一个字段！");
            }
            else if (@from.Fields.Count > 1)
            {
                throw new Exception(".ToList<" + typet.Name + ">()最多.Select()一个字段！");
            }
            else
            {
                string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "List", "|",
                    formatSql(@from.SqlString, @from));
                object cacheValue = getCache(cacheKey);

                if (null != cacheValue)
                {
                    return (List<TResult>)cacheValue;
                }
                List<TResult> list = new List<TResult>();
                using (IDataReader reader = ToDataReader(@from))
                {
                    while (reader.Read())
                    {
                        list.Add(DataUtils.ConvertValue<TResult>(reader[@from.Fields[0].Name]));
                    }
                    reader.Close();
                }
                setCache<List<TResult>>(list, cacheKey);
                return list;
            }
        }

        /// <summary>
        /// To List&lt;T>
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            FromSection from = getPagedFromSection();
            string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "List", "|", formatSql(from.SqlString, from));
            object cacheValue = getCache(cacheKey);
            if (null != cacheValue)
            {
                return (List<T>)cacheValue;
            }
            List<T> list = new List<T>();
            using (IDataReader reader = ToDataReader(from))
            {
                list = EntityUtils.Mapper.Map<T>(reader);
                #region 2015-08-10注释
                //if (@from.Joins.Any() || from.Fields.Any())
                //{
                //    //list = new EmitMapper.Mappers.DataReaderToObjectMapper<T>().ReadCollection(reader).ToList();
                //    //while (reader.Read())
                //    //{
                //    //    T t = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<IDataReader, T>().Map(reader);
                //    //    list.Add(t);
                //    //}
                //    //DataRecordInternal
                //    //T result = MapUsingState(reader, reader);
                //    //list = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<IDataReader, List<T>>().Map(reader);
                //    list = EntityUtils.Mapper.Map<T>(reader);
                //}
                //else
                //{
                //    while (reader.Read())
                //    {
                //        T t = DataUtils.Create<T>();
                //        t.SetPropertyValues(reader);
                //        list.Add(t);
                //    }
                //}
                #endregion
                reader.Close();
            }
            setCache<List<T>>(list, cacheKey);
            //2015-09-08
            if (list != null)
            {
                foreach (var m in list)
                {
                    m.ClearModifyFields();
                }
            }

            return list;
        }

        /// <summary>
        /// 返回第一个实体  如果为null，则默认实例化一个
        /// </summary>
        /// <returns></returns>
        public T ToFirstDefault()
        {
            T t = this.ToFirst();
            if (t == null)
                t = DataUtils.Create<T>();
            return t;
        }

        /// <summary>
        /// 返回第一个实体，同ToFirst()。无数据返回Null。
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            return ToFirst();
        }
        /// <summary>
        /// 返回第一个实体，同ToFirst()。无数据返回Null。
        /// </summary>
        /// <returns></returns>
        public TResult First<TResult>() where TResult : class
        {
            return ToFirst<TResult>();
        }
        /// <summary>
        /// 返回第一个实体 ，同First()。无数据返回Null。
        /// </summary>
        /// <returns></returns>
        public TResult ToFirst<TResult>() where TResult : class
        {
            var typet = typeof(TResult);
            if (typet == typeof(T))
            {
                return ToFirst() as TResult;
            }
            FromSection from = this.Top(1).getPagedFromSection();
            string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "FirstT", "|", formatSql(from.SqlString, from));
            object cacheValue = getCache(cacheKey);

            if (null != cacheValue)
            {
                return (TResult)cacheValue;
            }


            TResult t = default(TResult);
            using (IDataReader reader = ToDataReader(from))
            {
                var tempt = EntityUtils.Mapper.Map<TResult>(reader);
                if (tempt.Count > 0)
                {
                    t = tempt[0];
                }
                reader.Close();
            }

            setCache<TResult>(t, cacheKey);
            return t;
        }
        /// <summary>
        /// 返回第一个实体 ，同First()。无数据返回Null。
        /// </summary>
        /// <returns></returns>
        public T ToFirst()
        {
            FromSection from = this.Top(1).getPagedFromSection();
            string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "FirstT", "|", formatSql(from.SqlString, from));
            object cacheValue = getCache(cacheKey);

            if (null != cacheValue)
            {
                return (T)cacheValue;
            }


            T t = null;
            using (IDataReader reader = ToDataReader(from))
            {
                var tempt = EntityUtils.Mapper.Map<T>(reader);
                if (tempt.Count > 0)
                {
                    t = tempt[0];
                }
                #region 2015-08-10注释
                //if (@from.Joins.Any() || from.Fields.Any())
                //{
                //    var tempt = EntityUtils.Mapper.Map<T>(reader);
                //    if (tempt.Any())
                //    {
                //        t = tempt.First();
                //    }
                //}
                //else
                //{
                //    if (reader.Read())
                //    {
                //        t = DataUtils.Create<T>();
                //        t.SetPropertyValues(reader);
                //    }
                //}
                #endregion
                reader.Close();
            }

            setCache<T>(t, cacheKey);
            //2015-09-08
            if (t != null)
            {
                t.ClearModifyFields();
            }
            return t;
        }

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public new FromSection<T> Union(FromSection fromSection)
        {
            StringBuilder tname = new StringBuilder();

            tname.Append("(");

            tname.Append(this.SqlNoneOrderbyString);

            tname.Append(" UNION ");

            tname.Append(fromSection.SqlNoneOrderbyString);

            tname.Append(")");

            tname.Append(" ");

            tname.Append(EntityCache.GetTableName<T>());


            FromSection<T> tmpfromSection = new FromSection<T>(this.database);
            tmpfromSection.tableName = tname.ToString();

            tmpfromSection.parameters.AddRange(this.Parameters);
            tmpfromSection.parameters.AddRange(fromSection.Parameters);

            return tmpfromSection;
        }

        /// <summary>
        /// Union All
        /// </summary>        
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public new FromSection<T> UnionAll(FromSection fromSection)
        {
            StringBuilder tname = new StringBuilder();

            tname.Append("(");

            tname.Append(this.SqlNoneOrderbyString);

            tname.Append(" UNION ALL ");

            tname.Append(fromSection.SqlNoneOrderbyString);

            tname.Append(")");

            tname.Append(" ");

            tname.Append(EntityCache.GetTableName<T>());

            FromSection<T> tmpfromSection = new FromSection<T>(this.database);
            tmpfromSection.tableName = tname.ToString();

            tmpfromSection.parameters.AddRange(this.Parameters);
            tmpfromSection.parameters.AddRange(fromSection.Parameters);

            return tmpfromSection;
        }

        #endregion

    }

    /// <summary>
    /// 查询
    /// </summary>    
    public class FromSection
    {
        #region 变量

        protected WhereClip where = WhereClip.All;
        protected WhereClip havingWhere = WhereClip.All;
        protected OrderByClip orderBy = OrderByClip.None;
        protected GroupByClip groupBy = GroupByClip.None;
        protected string tableName;
        protected List<Parameter> parameters = new List<Parameter>();
        protected List<Field> fields = new List<Field>();
        protected DbProvider dbProvider;
        protected Dictionary<string, KeyValuePair<string, WhereClip>> joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
        protected Database database;
        protected string distinctString;
        protected string prefixString;



        /// <summary>
        /// 开始项
        /// </summary>
        protected int startIndex;
        /// <summary>
        /// 结束项
        /// </summary>
        protected int endIndex;


        /// <summary>
        /// 缓存超时时间
        /// </summary>
        protected int? timeout;


        /// <summary>
        /// 缓存依赖
        /// </summary>
        protected CacheDependency cacheDep = null;


        /// <summary>
        /// 
        /// </summary>
        protected string typeTableName;

        /// <summary>
        /// 是否重新加载
        /// </summary>
        protected bool isRefresh = false;

        /// <summary>
        /// 是否已经执行过分页
        /// </summary>
        protected bool isPageFromSection = false;

        /// <summary>
        /// 事务   -- 查询
        /// </summary>
        protected DbTransaction trans;
        #endregion

        #region 属性
        //2015-08-12恢复注释
        /// <summary>
        /// DbProvider。
        /// </summary>
        public DbProvider DbProvider
        {
            get { return dbProvider; }
        }
        //2015-08-12新增
        /// <summary>
        /// DbProvider。
        /// </summary>
        public Database Database
        {
            get { return database; }
        }
        /// <summary>
        /// 设置 distinct
        /// </summary>
        internal string DistinctString
        {
            set
            {
                distinctString = value;
            }
        }

        /// <summary>
        /// 前置值如 Top N
        /// </summary>
        internal string PrefixString
        {
            set
            {
                prefixString = value;
            }
        }

        private string limitString;
        /// <summary>
        /// limit 
        /// </summary>
        public string LimitString
        {
            set
            {

                limitString = value;
            }
        }

        /// <summary>
        /// 记录数sql语句 count
        /// </summary>
        public string CountSqlString
        {
            get
            {
                StringBuilder sql = new StringBuilder();

                if (GroupByClip.IsNullOrEmpty(groupBy) && string.IsNullOrEmpty(distinctString))
                {
                    sql.Append(" SELECT count(*) as r_cnt FROM ");
                    sql.Append(FromString);
                    if (!WhereClip.IsNullOrEmpty(where))
                    {
                        sql.Append(where.WhereString);
                    }
                }
                else
                {

                    sql.Append("SELECT count(*) as r_cnt FROM (");

                    sql.Append(SqlNoneOrderbyString);

                    sql.Append(") tmp__table");
                }

                return sql.ToString();
            }
        }

        /// <summary>
        /// 没有没有排序字段
        /// </summary>
        internal string SqlNoneOrderbyString
        {
            get
            {
                StringBuilder sql = new StringBuilder();

                sql.Append(" SELECT ");
                sql.Append(distinctString);
                sql.Append(" ");
                sql.Append(prefixString);
                sql.Append(" ");
                sql.Append(ColumnsString);
                sql.Append(" FROM ");
                sql.Append(FromString);
                sql.Append(" ");

                if (!WhereClip.IsNullOrEmpty(where))
                {
                    sql.Append(where.WhereString);
                }
                if (!GroupByClip.IsNullOrEmpty(groupBy))
                {
                    sql.Append(GroupByString);
                    if (!WhereClip.IsNullOrEmpty(havingWhere))
                    {
                        sql.Append(" HAVING ");
                        sql.Append(havingWhere.ToString());
                    }
                }
                return sql.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string FromString
        {
            get
            {
                StringBuilder fromstring = new StringBuilder();


                //处理ACCESS 的多表联合查询
                if (database.DbProvider is MsAccess.MsAccessProvider)
                {
                    fromstring.Append('(', joins.Count);
                    fromstring.Append(tableName);
                    foreach (KeyValuePair<string, KeyValuePair<string, WhereClip>> kv in joins)
                    {
                        fromstring.Append(" ");
                        fromstring.Append(kv.Value.Key);
                        fromstring.Append(" ");
                        fromstring.Append(kv.Key);
                        fromstring.Append(" ON ");
                        fromstring.Append(kv.Value.Value.ToString());
                        fromstring.Append(")");
                    }

                }
                else
                {
                    fromstring.Append(tableName);
                    foreach (KeyValuePair<string, KeyValuePair<string, WhereClip>> kv in joins)
                    {
                        fromstring.Append(" ");
                        fromstring.Append(kv.Value.Key);
                        fromstring.Append(" ");
                        fromstring.Append(kv.Key);
                        fromstring.Append(" ON ");
                        fromstring.Append(kv.Value.Value.ToString());
                    }
                }

                return fromstring.ToString();
            }

        }

        /// <summary>
        /// 连接信息
        /// </summary>
        internal Dictionary<string, KeyValuePair<string, WhereClip>> Joins
        {
            get
            {
                return joins;
            }
            set
            {
                joins = value;
            }
        }

        /// <summary>
        /// 获取 sql语句
        /// </summary>
        public string SqlString
        {
            get
            {
                StringBuilder sql = new StringBuilder();

                sql.Append(" SELECT ");
                sql.Append(distinctString);
                sql.Append(" ");
                sql.Append(prefixString);
                sql.Append(" ");
                sql.Append(ColumnsString);
                sql.Append(" FROM ");
                sql.Append(FromString);
                sql.Append(" ");

                if (!WhereClip.IsNullOrEmpty(where))
                {
                    sql.Append(where.WhereString);
                }
                if (!GroupByClip.IsNullOrEmpty(groupBy))
                {
                    sql.Append(GroupByString);
                    if (!WhereClip.IsNullOrEmpty(havingWhere))
                    {
                        sql.Append(" HAVING ");
                        sql.Append(havingWhere.ToString());
                    }
                }

                sql.Append(OrderByString);
                sql.Append(" ");
                sql.Append(limitString);
                return sql.ToString();
            }
        }

        /// <summary>
        /// 返回  表名
        /// </summary>
        public string TableName
        {
            get
            {
                return tableName;
            }
            internal set
            {
                tableName = value;

                this.joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
            }
        }


        /// <summary>
        /// 返回  排序
        /// </summary>
        public OrderByClip OrderByClip
        {
            get
            {
                return orderBy;
            }
            internal set
            {
                orderBy = value;
            }
        }

        /// <summary>
        /// 返回排序字符串  例如：orderby id desc
        /// </summary>
        public string OrderByString
        {
            get
            {
                if (OrderByClip.IsNullOrEmpty(orderBy))
                    return string.Empty;

                if (tableName.IndexOf('(') >= 0 || tableName.IndexOf(')') >= 0 || tableName.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase) >= 0 || tableName.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase) >= 0)
                    return orderBy.RemovePrefixTableName().OrderByString;
                return orderBy.OrderByString;
            }
        }

        /// <summary>
        /// 返回 分组
        /// </summary>
        public GroupByClip GroupByClip
        {
            get
            {
                return groupBy;
            }
            internal set
            {
                groupBy = value;
            }
        }

        /// <summary>
        /// 返回排序字符串 例如：group by id
        /// </summary>
        public string GroupByString
        {
            get
            {
                if (GroupByClip.IsNullOrEmpty(groupBy))
                    return string.Empty;
                if (tableName.IndexOf('(') >= 0 || tableName.IndexOf(')') >= 0 || tableName.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase) >= 0 || tableName.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase) >= 0)
                    return groupBy.RemovePrefixTableName().GroupByString;
                return groupBy.GroupByString;
            }
        }

        /// <summary>
        /// 返回 条件
        /// </summary>
        public WhereClip WhereClip
        {
            get
            {
                return where;
            }
        }

        /// <summary>
        /// 返回 参数  (包含where 和 from)
        /// </summary>
        public List<Parameter> Parameters
        {
            get
            {
                List<Parameter> ps = new List<Parameter>();

                if (!WhereClip.IsNullOrEmpty(where))
                    ps.AddRange(where.Parameters);

                //处理groupby的having
                if (!GroupByClip.IsNullOrEmpty(groupBy) && !WhereClip.IsNullOrEmpty(havingWhere))
                    ps.AddRange(havingWhere.Parameters);

                ps.AddRange(parameters);

                return ps;
            }
            internal set
            {
                this.parameters = value;
            }


        }

        /// <summary>
        /// 返回  选择列
        /// </summary>
        public string ColumnsString
        {
            get
            {
                if (fields.Count == 0)
                    return "*";

                StringBuilder columns = new StringBuilder();
                foreach (Field filed in fields)
                {
                    columns.Append(",");
                    columns.Append(filed.FullName);
                }

                return columns.ToString().Substring(1);
            }
        }


        /// <summary>
        /// 查询的字段
        /// </summary>
        public List<Field> Fields
        {
            get
            {
                return this.fields;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tableName"></param>
        public FromSection(Database database, string tableName)
            : this(database, tableName, (DbTransaction)null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tableName"></param>
        /// <param name="trans"></param>
        public FromSection(Database database, string tableName, DbTransaction trans)
        {
            Check.Require(database, "database", Check.NotNull);
            Check.Require(tableName, "tableName", Check.NotNullOrEmpty);

            this.trans = trans;
            this.dbProvider = database.DbProvider;
            this.database = database;
            this.tableName = tableName;
            this.typeTableName = tableName.Trim(dbProvider.LeftToken, dbProvider.RightToken);
        }

        #endregion

        #region 操作


        /// <summary>
        /// 是否开启缓存
        /// </summary>
        /// <returns></returns>
        protected bool isTurnonCache()
        {
            if (null == dbProvider.CacheConfig)
                return false;

            return dbProvider.CacheConfig.Enable;

        }

        /// <summary>
        /// 判断是否用户自定义缓存策略
        /// </summary>
        /// <returns></returns>
        protected bool isCustomerCache()
        {
            return (timeout.HasValue || null != cacheDep);
        }


        /// <summary>
        /// 设置缓存有效期  单位：秒
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public FromSection SetCacheTimeOut(int timeout)
        {
            this.timeout = timeout;
            return this;
        }

        /// <summary>
        /// 设置缓存依赖
        /// </summary>
        /// <param name="dep"></param>
        /// <returns></returns>
        public FromSection SetCacheDependency(CacheDependency dep)
        {
            this.cacheDep = dep;
            return this;
        }


        /// <summary>
        /// 重新加载
        /// </summary>
        /// <returns></returns>
        public FromSection Refresh()
        {
            isRefresh = true;
            return this;
        }


        /// <summary>
        /// whereclip
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection Where(WhereClip where)
        {
            //2015-09-08修改
            this.where = where;
            //this.where = this.where && where;
            return this;
        }


        /// <summary>
        /// groupby
        /// </summary>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        public FromSection GroupBy(GroupByClip groupBy)
        {
            //2015-09-08修改
            this.groupBy = groupBy;
            //this.groupBy = this.groupBy && groupBy;
            return this;
        }


        /// <summary>
        /// having条件
        /// </summary>
        /// <param name="havingWhere"></param>
        /// <returns></returns>
        public FromSection Having(WhereClip havingWhere)
        {
            //2015-09-08修改
            this.havingWhere = havingWhere;
            //this.havingWhere = this.havingWhere && havingWhere;
            return this;
        }

        /// <summary>
        /// groupby
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public FromSection GroupBy(params Field[] fields)
        {
            if (null != fields && fields.Length > 0)
            {
                GroupByClip tempgroupby = GroupByClip.None;
                foreach (Field f in fields)
                {
                    tempgroupby = tempgroupby && f.GroupBy;
                }
                //2015-09-08修改
                this.groupBy = tempgroupby;
                //this.groupBy = this.groupBy && tempgroupby;
            }
            return this;
        }

        /// <summary>
        /// orderby
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public FromSection OrderBy(OrderByClip orderBy)
        {
            //2015-09-08修改
            this.orderBy = orderBy;
            //this.orderBy = this.orderBy && orderBy;
            return this;
        }


        /// <summary>
        /// orderby
        /// </summary>
        /// <param name="orderBys"></param>
        /// <returns></returns>
        public FromSection OrderBy(params OrderByClip[] orderBys)
        {
            if (null != orderBys && orderBys.Length > 0)
            {
                OrderByClip temporderby = OrderByClip.None;
                foreach (OrderByClip ob in orderBys)
                {
                    temporderby = temporderby && ob;
                }
                //2015-09-08修改
                this.orderBy =temporderby;
                //this.orderBy = this.orderBy && temporderby;
            }
            return this;
        }


        /// <summary>
        /// select field
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public FromSection Select(params Field[] fields)
        {
            //2015-09-08取消注释
            this.fields.Clear();
            return AddSelect(fields);
        }


        /// <summary>
        /// select sql
        /// </summary>
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public FromSection AddSelect(FromSection fromSection)
        {
            return AddSelect(fromSection, null);
        }

        /// <summary>
        /// select sql
        /// </summary>
        /// <param name="fromSection"></param>
        /// <param name="aliasName">别名</param>
        /// <returns></returns>
        public FromSection AddSelect(FromSection fromSection, string aliasName)
        {
            if (null == fromSection)
                return this;

            Check.Require(fromSection.Fields.Count == 1 && !fromSection.Fields[0].PropertyName.Equals("*"), "fromSection's fields must be only one!");

            this.fields.Add(new Field(string.Concat("(", fromSection.SqlString, ")")).As(aliasName));

            this.parameters.AddRange(fromSection.Parameters);

            return this;
        }


        /// <summary>
        /// add select field
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal FromSection AddSelect(params Field[] fields)
        {
            if (null != fields && fields.Length > 0)
            {
                foreach (Field field in fields)
                {
                    Field f = this.fields.Find(delegate(Field fi) { return fi.Name.Equals(field.Name); });
                    if (Field.IsNullOrEmpty(f))
                        this.fields.Add(field);
                }
            }
            return this;
        }

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        public FromSection Distinct()
        {
            this.distinctString = " DISTINCT ";
            return this;
        }

        /// <summary>
        /// Top
        /// </summary>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public FromSection Top(int topCount)
        {
            return From(1, topCount);
        }


        /// <summary>
        /// Page
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageIndex">第几页</param>
        /// <returns></returns>
        public FromSection Page(int pageSize, int pageIndex)
        {
            return From(pageSize * (pageIndex - 1) + 1, pageIndex * pageSize);
        }


        /// <summary>
        /// From startIndex to endIndex
        /// </summary>
        /// <param name="startIndex">开始记录数</param>
        /// <param name="endIndex">结束记录数</param>
        /// <returns></returns>
        public FromSection From(int startIndex, int endIndex)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;

            isPageFromSection = false;

            return this;
        }


        /// <summary>
        /// 格式化sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        protected string formatSql(string sql, FromSection from)
        {
            string tempSql = DataUtils.FormatSQL(sql, from.dbProvider.LeftToken, from.dbProvider.RightToken);
            List<Parameter> listPara = from.Parameters;
            foreach (Parameter p in listPara)
            {
                tempSql = tempSql.Replace(p.ParameterName, p.ParameterValue == null ? string.Empty : p.ParameterValue.ToString());
            }
            return tempSql;
        }

        #endregion

        #region 查询


        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            //FromSection from;

            //if (!isPageFromSection)
            //    from = getPagedFromSection();
            //else
            //    from = this;

            return Count(getPagedFromSection());
        }

        /// <summary>
        /// 获取记录数(内部使用)
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        internal int Count(FromSection from)
        {
            string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "COUNT", "|", formatSql(from.CountSqlString, from));

            object cacheValue = getCache(cacheKey);
            if (null != cacheValue)
            {
                return (int)cacheValue;
            }

            DbCommand dbCommand = database.GetSqlStringCommand(from.CountSqlString);
            database.AddCommandParameter(dbCommand, from.Parameters.ToArray());
            int returnValue;
            if (trans == null)
                returnValue = DataUtils.ConvertValue<int>(database.ExecuteScalar(dbCommand));
            else
                returnValue = DataUtils.ConvertValue<int>(database.ExecuteScalar(dbCommand, trans));

            setCache<int>(returnValue, cacheKey);

            return returnValue;
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        protected object getCache(string cacheKey)
        {
            if (isRefresh)
                return null;

            object cacheValue = Cache.Default.GetCache(cacheKey);

            //if (null != cacheValue && isCustomerCache())
            //{
            //    Cache.Default.AddCacheDependency(cacheKey, cacheValue, timeout.HasValue ? timeout.Value : 0, cacheDep);
            //}

            return cacheValue;
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cacheKey"></param>
        protected void setCache<T>(T value, string cacheKey)
        {
            if (isCustomerCache())
                Cache.Default.AddCacheDependency(cacheKey, value, timeout.HasValue ? timeout.Value : 0, cacheDep);
            else
            {
                if (isTurnonCache())
                {
                    string entityCacheKey = string.Concat(dbProvider.ConnectionStringsName, typeTableName);
                    if (dbProvider.EntitiesCache.ContainsKey(entityCacheKey))
                    {
                        int? temptimeOut = dbProvider.EntitiesCache[entityCacheKey].TimeOut;
                        if (temptimeOut.HasValue)
                        {
                            Cache.Default.AddCacheSlidingExpiration(cacheKey, value, temptimeOut.Value);
                        }
                        else
                        {
                            Cache.Default.AddCacheDependency(cacheKey, value, 0, new CacheDependency(dbProvider.EntitiesCache[entityCacheKey].FilePath));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// To DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ToDataSet()
        {
            FromSection from = getPagedFromSection();
            string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "DataSet", "|", formatSql(from.SqlString, from));
            object cacheValue = getCache(cacheKey);
            if (null != cacheValue)
            {
                return (DataSet)cacheValue;
            }

            DataSet ds;
            if (trans == null)
                ds = database.ExecuteDataSet(createDbCommand(from));
            else
                ds = database.ExecuteDataSet(createDbCommand(from), trans);

            setCache<DataSet>(ds, cacheKey);

            return ds;
        }

        /// <summary>
        /// 获取分页过的FromSection
        /// </summary>
        /// <returns></returns>
        internal FromSection getPagedFromSection()
        {
            if (startIndex > 0 && endIndex > 0 && !isPageFromSection)
            {
                isPageFromSection = true;
                return dbProvider.CreatePageFromSection(this, startIndex, endIndex);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// 创建  查询的DbCommand
        /// </summary>
        /// <returns></returns>
        protected DbCommand createDbCommand(FromSection from)
        {
            DbCommand dbCommand = database.GetSqlStringCommand(from.SqlString);
            database.AddCommandParameter(dbCommand, from.Parameters.ToArray());
            return dbCommand;
        }

        /// <summary>
        /// To DataReader
        /// </summary>
        /// <returns></returns>
        public IDataReader ToDataReader()
        {
            return ToDataReader(getPagedFromSection());
        }

        /// <summary>
        ///  To DataReader
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        protected IDataReader ToDataReader(FromSection from)
        {
            if (trans == null)
                return database.ExecuteReader(createDbCommand(from));
            else
                return database.ExecuteReader(createDbCommand(from), trans);
        }

        /// <summary>
        /// To DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            return this.ToDataSet().Tables[0];
        }

        /// <summary>
        /// To Scalar
        /// </summary>
        /// <returns></returns>
        public object ToScalar()
        {
            Check.Require(this.fields.Count == 1, "fields must be one!");
            Check.Require(!this.fields[0].PropertyName.Trim().Equals("*"), "fields cound not be * !");

            FromSection from = getPagedFromSection();
            string cacheKey = string.Concat(dbProvider.ConnectionStringsName, "Scalar", "|", formatSql(from.SqlString, from));
            object cacheValue = getCache(cacheKey);
            if (null != cacheValue)
            {
                return cacheValue;
            }

            object returnValue;

            if (trans == null)
                returnValue = database.ExecuteScalar(createDbCommand(from));
            else
                returnValue = database.ExecuteScalar(createDbCommand(from), trans);

            setCache<object>(returnValue, cacheKey);

            return returnValue;

        }

        /// <summary>
        /// To Scalar
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            return DataUtils.ConvertValue<TResult>(this.ToScalar());
        }


        #endregion

        #region 连接 join


        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        protected FromSection join(string tableName, WhereClip where, JoinType joinType)
        {
            if (string.IsNullOrEmpty(tableName) || WhereClip.IsNullOrEmpty(where))
                return this;

            tableName = dbProvider.BuildTableName(tableName);

            if (!joins.ContainsKey(tableName))
            {
                string joinString = string.Empty;
                switch (joinType)
                {
                    case JoinType.InnerJoin:
                        joinString = "INNER JOIN";
                        break;
                    case JoinType.LeftJoin:
                        joinString = "LEFT OUTER JOIN";
                        break;
                    case JoinType.RightJoin:
                        joinString = "RIGHT OUTER JOIN";
                        break;
                    case JoinType.CrossJoin:
                        joinString = "CROSS JOIN";
                        break;
                    case JoinType.FullJoin:
                        joinString = "FULL OUTER JOIN";
                        break;
                    default:
                        joinString = "INNER JOIN";
                        break;
                }

                joins.Add(tableName, new KeyValuePair<string, WhereClip>(joinString, where));

                if (where.Parameters.Count > 0)
                    parameters.AddRange(where.Parameters);
            }

            return this;
        }


        /// <summary>
        /// Inner Join
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection InnerJoin(string tableName, WhereClip where)
        {
            return join(tableName, where, JoinType.InnerJoin);
        }



        /// <summary>
        /// Left Join
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection LeftJoin(string tableName, WhereClip where)
        {
            return join(tableName, where, JoinType.LeftJoin);
        }



        /// <summary>
        /// Right Join
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection RightJoin(string tableName, WhereClip where)
        {
            return join(tableName, where, JoinType.RightJoin);
        }


        /// <summary>
        /// Cross Join
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection CrossJoin(string tableName, WhereClip where)
        {
            return join(tableName, where, JoinType.CrossJoin);
        }



        /// <summary>
        /// Full Join
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public FromSection FullJoin(string tableName, WhereClip where)
        {
            return join(tableName, where, JoinType.FullJoin);
        }

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public FromSection Union(FromSection fromSection)
        {
            StringBuilder tname = new StringBuilder();

            tname.Append("(");

            tname.Append(this.SqlNoneOrderbyString);

            tname.Append(" UNION ");

            tname.Append(fromSection.SqlNoneOrderbyString);

            tname.Append(") tempuniontable ");

            FromSection tmpfromSection = new FromSection(this.database, tname.ToString());
            tmpfromSection.typeTableName = this.typeTableName;
            tmpfromSection.timeout = this.timeout;
            tmpfromSection.cacheDep = this.cacheDep;
            tmpfromSection.isRefresh = this.isRefresh;


            tmpfromSection.parameters.AddRange(this.Parameters);
            tmpfromSection.parameters.AddRange(fromSection.Parameters);

            return tmpfromSection;
        }

        /// <summary>
        /// Union All
        /// </summary>
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public FromSection UnionAll(FromSection fromSection)
        {
            StringBuilder tname = new StringBuilder();

            tname.Append("(");

            tname.Append(this.SqlNoneOrderbyString);

            tname.Append(" UNION ALL ");

            tname.Append(fromSection.SqlNoneOrderbyString);

            tname.Append(") tempuniontable ");

            FromSection tmpfromSection = new FromSection(this.database, tname.ToString());
            tmpfromSection.typeTableName = this.typeTableName;
            tmpfromSection.timeout = this.timeout;
            tmpfromSection.cacheDep = this.cacheDep;
            tmpfromSection.isRefresh = this.isRefresh;

            tmpfromSection.parameters.AddRange(this.Parameters);
            tmpfromSection.parameters.AddRange(fromSection.Parameters);

            return tmpfromSection;
        }

        #endregion
    }
}
