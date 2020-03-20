#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) ITdos
* CLR 版本: 4.0.30319.18408
* 创 建 人：steven hu
* 电子邮箱：
* 官方网站：www.ITdos.com
* 创建日期：2010/2/10
* 文件描述：
******************************************************
* 修 改 人：ITdos
* 修改日期：
* 备注描述：
*******************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Dos.ORM
{
    /// <summary>
    /// The db provider factory.
    /// </summary>
    public sealed class ProviderFactory
    {
        #region Private Members

        private static Dictionary<string, DbProvider> providerCache = new Dictionary<string, DbProvider>();

        private ProviderFactory() { }

        #endregion

        #region Public Members

        /// <summary>
        /// 创建数据库事件提供程序
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="connectionString">The conn STR.</param>
        /// <param name="databaseType">The DatabaseType.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(string assemblyName, string className, string connectionString, DatabaseType? databaseType)
        {
            Check.Require(connectionString, "connectionString", Check.NotNullOrEmpty);

            if (connectionString.IndexOf("microsoft.jet.oledb", StringComparison.OrdinalIgnoreCase) > -1 || connectionString.IndexOf(".db3", StringComparison.OrdinalIgnoreCase) > -1)
            {
                Check.Require(connectionString.IndexOf("data source", StringComparison.OrdinalIgnoreCase) > -1, "ConnectionString的格式有错误，请查证！");

                string mdbPath = connectionString.Substring(connectionString.IndexOf("data source", StringComparison.OrdinalIgnoreCase) + "data source".Length + 1).TrimStart(' ', '=');
                if (mdbPath.ToLower().StartsWith("|datadirectory|"))
                {
                    mdbPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\App_Data" + mdbPath.Substring("|datadirectory|".Length);
                }
                else if (connectionString.StartsWith("./") || connectionString.EndsWith(".\\"))
                {
                    connectionString = connectionString.Replace("/", "\\").Replace(".\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                }
                connectionString = connectionString.Substring(0, connectionString.ToLower().IndexOf("data source")) + "Data Source=" + mdbPath;
            }

            //如果是~则表示当前目录
            if (connectionString.Contains("~/") || connectionString.Contains("~\\"))
            {
                connectionString = connectionString.Replace("/", "\\").Replace("~\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
            }

            //by default, using sqlserver db provider
            if (string.IsNullOrEmpty(className))
            {
                className = typeof(SqlServer.SqlServerProvider).ToString();
                if (databaseType == null)
                {
                    databaseType = DatabaseType.SqlServer;
                }
            }
            else if (String.Compare(className, "System.Data.SqlClient", StringComparison.OrdinalIgnoreCase) == 0 || String.Compare(className, "Dos.ORM.SqlServer", StringComparison.OrdinalIgnoreCase) == 0)
            {
                className = typeof(SqlServer.SqlServerProvider).ToString();
                if (databaseType == null)
                {
                    databaseType = DatabaseType.SqlServer;
                }
            }
            else if (String.Compare(className, "Dos.ORM.SqlServer9", StringComparison.OrdinalIgnoreCase) == 0 || className.IndexOf("SqlServer9", StringComparison.OrdinalIgnoreCase) >= 0 || className.IndexOf("sqlserver2005", StringComparison.OrdinalIgnoreCase) >= 0 || className.IndexOf("sql2005", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                className = typeof(SqlServer9.SqlServer9Provider).ToString();
                if (databaseType == null)
                {
                    databaseType = DatabaseType.SqlServer9;
                }
            }
            else if (className.IndexOf("oracle", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                className = typeof(Oracle.OracleProvider).ToString();
                if (databaseType == null)
                {
                    databaseType = DatabaseType.Oracle;
                }
            }
            else if (className.IndexOf("access", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                className = typeof(MsAccess.MsAccessProvider).ToString();
                if (databaseType == null)
                {
                    databaseType = DatabaseType.MsAccess;
                }
            }
            else if (className.IndexOf("mysql", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                className = "Dos.ORM.MySql.MySqlProvider";
                assemblyName = "Dos.ORM.MySql";
                if (databaseType == null)
                {
                    databaseType = DatabaseType.MySql;
                }
            }
            else if (className.IndexOf("sqlite", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                className = "Dos.ORM.Sqlite.SqliteProvider";
                assemblyName = "Dos.ORM.Sqlite";
                if (databaseType == null)
                {
                    databaseType = DatabaseType.Sqlite3;
                }
            }

            string cacheKey = string.Concat(assemblyName, className, connectionString);
            if (providerCache.ContainsKey(cacheKey))
            {
                return providerCache[cacheKey];
            }
            else
            {
                System.Reflection.Assembly ass;

                ass = assemblyName == null ? typeof(DbProvider).Assembly 
                    : System.Reflection.Assembly.Load(assemblyName);

                DbProvider retProvider = ass.CreateInstance(className, false, System.Reflection.BindingFlags.Default, null, new object[] { connectionString }, null, null) as DbProvider;
                if (retProvider != null && databaseType != null)
                {
                    retProvider.DatabaseType = databaseType.Value;
                }
                providerCache.Add(cacheKey, retProvider);
                return retProvider;
            }
        }

        /// <summary>
        /// Gets the default db provider.
        /// </summary>
        /// <value>The default.</value>
        public static DbProvider Default
        {
            get
            {
                try
                {
                    if (ConfigurationManager.ConnectionStrings.Count > 0)
                    {
                        DbProvider dbProvider;
                        ConnectionStringSettings connStrSetting = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1];
                        string[] assAndClass = connStrSetting.ProviderName.Split(',');
                        if (assAndClass.Length > 1)
                        {
                            dbProvider = CreateDbProvider(assAndClass[1].Trim(), assAndClass[0].Trim(), connStrSetting.ConnectionString, null);
                        }
                        else
                        {
                            dbProvider = CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString, null);
                        }

                        dbProvider.ConnectionStringsName = connStrSetting.Name;

                        return dbProvider;
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="connStrName">Name of the conn STR.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(string connStrName)
        {
            Check.Require(connStrName, "connStrName", Check.NotNullOrEmpty);

            var connStrSetting = ConfigurationManager.ConnectionStrings[connStrName];
            Check.Invariant(connStrSetting != null, null, new ConfigurationErrorsException(string.Concat("Cannot find specified connection string setting named as ", connStrName, " in application config file's ConnectionString section.")));
            //2015-08-13新增
            if (connStrSetting == null || string.IsNullOrWhiteSpace(connStrSetting.ConnectionString))
            {
                throw new Exception("数据库连接字符串【" + connStrName + "】没有配置！");
            }
            var assAndClass = connStrSetting.ProviderName.Split(',');
            var dbProvider = assAndClass.Length > 1 
                ? CreateDbProvider(assAndClass[0].Trim(), assAndClass[1].Trim(), connStrSetting.ConnectionString, null) 
                : CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString, null);
            dbProvider.ConnectionStringsName = connStrName;
            return dbProvider;
        }
        #endregion
    }
}
