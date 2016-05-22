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
using System.Data.OleDb;
using System.Data.Common;
using System.Data;
using Dos.ORM.Common;
using Dos.ORM.Common;

namespace Dos.ORM.MsAccess
{
    
    public class MsAccessProvider : DbProvider
    {
        public MsAccessProvider(string connectionString)
            : base(connectionString, OleDbFactory.Instance, '[', ']', '@','*')
        {
        }

        public override string RowAutoID
        {
            get { return string.Empty; }
        }

        public override bool SupportBatch
        {
            get { return false; }
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
                OleDbParameter oleDbParam = (OleDbParameter)p;

                if (oleDbParam.DbType != DbType.Guid && type == typeof(Guid))
                {
                    oleDbParam.OleDbType = OleDbType.Char;
                    oleDbParam.Size = 36;
                    continue;
                }

                if ((p.DbType == DbType.Time || p.DbType == DbType.DateTime) && type == typeof(TimeSpan))
                {
                    oleDbParam.OleDbType = OleDbType.Double;
                    oleDbParam.Value = ((TimeSpan)value).TotalDays;
                    continue;
                }

                if (type == typeof(Boolean))
                {
                    p.Value = ((bool)value).ToString();
                    continue;
                }

                switch (p.DbType)
                {
                    case DbType.Binary:
                        if (((byte[])value).Length > 2000)
                        {
                            oleDbParam.OleDbType = OleDbType.LongVarBinary;
                        }
                        break;
                    case DbType.Time:
                        oleDbParam.OleDbType = OleDbType.LongVarWChar;
                        p.Value = value.ToString();
                        break;
                    case DbType.DateTime:
                        oleDbParam.OleDbType = OleDbType.LongVarWChar;
                        p.Value = value.ToString();
                        break;
                    case DbType.AnsiString:
                        if (value.ToString().Length > 4000)
                        {
                            oleDbParam.OleDbType = OleDbType.LongVarChar;
                        }
                        break;
                    case DbType.String:
                        if (value.ToString().Length > 2000)
                        {
                            oleDbParam.OleDbType = OleDbType.LongVarWChar;
                        }
                        break;
                    case DbType.Object:
                        oleDbParam.OleDbType = OleDbType.LongVarWChar;
                        p.Value = SerializationManager.Serialize(value);
                        break;
                }
            }

            //replace "N'" to "'"
            cmd.CommandText = cmd.CommandText.Replace("N'", "'");

            //replace msaccess specific function names in cmdText
            cmd.CommandText = cmd.CommandText.Replace("upper(", "ucase(")
                            .Replace("lower(", "lcase(")
                            .Replace("substring(", "mid(")
                            .Replace("getdate()", "date() + time()")
                            .Replace("datepart(year", "datepart('yyyy'")
                            .Replace("datepart(month", "datepart('m'")
                            .Replace("datepart(day", "datepart('d'");

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
                    cmd.CommandText.Substring(endIndexOfCharIndex + 1) : "");

                startIndexOfCharIndex = cmd.CommandText.IndexOf("charindex(");
            }

        }
    }
}
