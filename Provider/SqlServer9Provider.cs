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
using Dos.ORM;

namespace Dos.ORM.SqlServer9
{
    
    /// <summary>
    /// Sql Server 2005
    /// </summary>
    public class SqlServer9Provider : SqlServer.SqlServerProvider
    {
        public SqlServer9Provider(string connectionString)
            : base(connectionString)
        {
        }


        /// <summary>
        /// 创建分页查询
        /// </summary>
        /// <param name="fromSection"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public override FromSection CreatePageFromSection(FromSection fromSection, int startIndex, int endIndex)
        {
            Check.Require(startIndex, "startIndex", Check.GreaterThanOrEqual<int>(1));
            Check.Require(endIndex, "endIndex", Check.GreaterThanOrEqual<int>(1));
            Check.Require(startIndex <= endIndex, "startIndex must be less than endIndex!");
            Check.Require(fromSection, "fromSection", Check.NotNullOrEmpty);

            if (startIndex == 1)
            {
                return base.CreatePageFromSection(fromSection, startIndex, endIndex);
            }


            if (OrderByClip.IsNullOrEmpty(fromSection.OrderByClip))
            {
                foreach (Field f in fromSection.Fields)
                {
                    if (!f.PropertyName.Equals("*"))
                    {
                        fromSection.OrderBy(f.Asc);
                        break;
                    }
                }
            }

            Check.Require(!OrderByClip.IsNullOrEmpty(fromSection.OrderByClip), "query.OrderByClip could not be null or empty!");

            if (fromSection.Fields.Count == 0)
            {
                fromSection.Select(Field.All);
            }

            fromSection.AddSelect(new Field(string.Concat("row_number() over(", fromSection.OrderByString, ") AS tmp_rowid")));
            //OrderByClip tempOrderBy = fromSection.OrderByClip;
            fromSection.OrderBy(OrderByClip.None);
            fromSection.TableName = string.Concat("(", fromSection.SqlString, ") AS tmp_table");
            fromSection.Parameters = fromSection.Parameters;
            fromSection.DistinctString = string.Empty;
            fromSection.PrefixString = string.Empty;
            fromSection.GroupBy(GroupByClip.None);
            fromSection.Select(Field.All);
            //fromSection.OrderBy(tempOrderBy);
            fromSection.Where(new WhereClip(string.Concat("tmp_rowid BETWEEN ", startIndex.ToString(), " AND ", endIndex.ToString())));

            return fromSection;
        }
    }
}
