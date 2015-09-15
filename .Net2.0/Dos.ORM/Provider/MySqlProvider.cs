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
using System.Data.Common;
using System.Data;
using Dos.ORM.Common;
using MySql.Data.MySqlClient;

namespace Dos.ORM.MySql
{

    public class MySqlProvider : DbProvider
    {

        public MySqlProvider(string connectionString)
            : base(connectionString, global::MySql.Data.MySqlClient.MySqlClientFactory.Instance, '`', '`', '?')
        {
        }

        public override string RowAutoID
        {
            get { return "select last_insert_id();"; }
        }

        public override bool SupportBatch
        {
            get { return true; }
        }


        public override void PrepareCommand(DbCommand cmd)
        {
            base.PrepareCommand(cmd);

            foreach (DbParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                {
                    continue;
                }

                object value = p.Value;
                if (value == DBNull.Value)
                {
                    continue;
                }
                Type type = value.GetType();
                MySqlParameter mySqlParam = (MySqlParameter)p;

                if (type == typeof(Guid))
                {
                    mySqlParam.MySqlDbType = MySqlDbType.VarChar;
                    mySqlParam.Size = 36;
                    continue;
                }

                if ((p.DbType == DbType.Time || p.DbType == DbType.DateTime) && type == typeof(TimeSpan))
                {
                    mySqlParam.MySqlDbType = MySqlDbType.Double;
                    mySqlParam.Value = ((TimeSpan)value).TotalDays;
                    continue;
                }

                switch (p.DbType)
                {
                    case DbType.Binary:
                        if (((byte[])value).Length > 2000)
                        {
                            mySqlParam.MySqlDbType = MySqlDbType.LongBlob;
                        }
                        break;
                    case DbType.Time:
                        mySqlParam.MySqlDbType = MySqlDbType.Datetime;
                        break;
                    case DbType.DateTime:
                        mySqlParam.MySqlDbType = MySqlDbType.Datetime;
                        break;
                    case DbType.AnsiString:
                        if (value.ToString().Length > 65535)
                        {
                            mySqlParam.MySqlDbType = MySqlDbType.LongText;
                        }
                        break;
                    case DbType.String:
                        if (value.ToString().Length > 65535)
                        {
                            mySqlParam.MySqlDbType = MySqlDbType.LongText;
                        }
                        break;
                    case DbType.Object:
                        mySqlParam.MySqlDbType = MySqlDbType.LongText;
                        p.Value = SerializationManager.Serialize(value);
                        break;
                }
            }




            //replace mysql specific function names in cmd.CommandText
            cmd.CommandText = cmd.CommandText
                .Replace("len(", "length(")
                .Replace("getdate()", "now()")
                .Replace("datepart(year,", "year(")
                .Replace("datepart(month,", "month(")
                .Replace("datepart(day,", "day(");

            //replace CHARINDEX with INSTR and reverse seqeunce of param items in CHARINDEX()
            int startIndexOfCharIndex = cmd.CommandText.IndexOf("charindex(");
            while (startIndexOfCharIndex > 0)
            {
                int endIndexOfCharIndex = DataUtils.GetEndIndexOfMethod(cmd.CommandText, startIndexOfCharIndex + "charindex(".Length);
                string[] itemsInCharIndex = DataUtils.SplitTwoParamsOfMethodBody(
                    cmd.CommandText.Substring(startIndexOfCharIndex + "charindex(".Length,
                    endIndexOfCharIndex - startIndexOfCharIndex - "charindex(".Length));
                cmd.CommandText = cmd.CommandText.Substring(0, startIndexOfCharIndex)
                    + "instr(" + itemsInCharIndex[1] + "," + itemsInCharIndex[0] + ")"
                    + (cmd.CommandText.Length - 1 > endIndexOfCharIndex ?
                    cmd.CommandText.Substring(endIndexOfCharIndex + 1) : string.Empty);

                startIndexOfCharIndex = cmd.CommandText.IndexOf("charindex(");
            }
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
            //Check.Require(fromSection.OrderByClip, "query.OrderByClip", Check.NotNullOrEmpty);

            fromSection.LimitString = string.Concat(" limit ", (startIndex - 1).ToString(), ",", (endIndex - startIndex + 1).ToString());


            return fromSection;
        }
    }
}
