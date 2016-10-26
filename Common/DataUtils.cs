﻿#region << 版 本 注 释 >>
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
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;
using Dos;
using Dos.ORM;

namespace Dos.ORM.Common
{
    /// <summary>
    /// 此类占位用，以兼容以前代码"using Dos.ORM.Common;"不会报错。
    /// </summary>
    public class DosORMCommon
    {

    }
}

namespace Dos.ORM
{

    /// <summary>
    /// 帮助类
    /// </summary>
    public class DataUtils
    {
        /// <summary>
        /// 格式化sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="leftToken"></param>
        /// <param name="rightToken"></param>
        /// <returns></returns>
        internal static string FormatSQL(string sql, char leftToken, char rightToken)
        {
            if (sql == null)
            {
                return string.Empty;
            }

            sql = sql.Replace("{0}", leftToken.ToString()).Replace("{1}", rightToken.ToString());

            return sql;
        }

        /// <summary>
        /// 格式化数据为数据库通用格式
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string FormatValue(object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "null";
            }

            Type type = val.GetType();

            if (type == typeof(DateTime) || type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            else if (type == typeof(TimeSpan))
            {
                DateTime baseTime = new DateTime(1900, 01, 01);
                return string.Format("(cast('{0}' as datetime) - cast('{1}' as datetime))", baseTime + ((TimeSpan)val), baseTime);
            }
            else if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            else if (val is Field)
            {
                return ((Field)val).TableFieldName;
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            else if (type.IsValueType)
            {
                return val.ToString();
            }
            else
            {
                return string.Concat("N'", val.ToString().Replace("'", "''"), "'");
            }
        }

        /// <summary>
        /// CheckStuct
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CheckStruct(Type type)
        {
            return ((type.IsValueType && !type.IsEnum) && (!type.IsPrimitive && !type.IsSerializable));
        }

        /// <summary>
        /// DeepClone
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Clone(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0L;
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ConvertValue(Type type, object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return null;
            }
            if (CheckStruct(type))
            {
                string data = value.ToString();
                return SerializationManager.Deserialize(type, data);
            }
            Type type2 = value.GetType();
            if (type == type2)
            {
                return value;
            }
            if (((type == typeof(Guid)) || (type == typeof(Guid?))) && (type2 == typeof(string)))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return null;
                }
                return new Guid(value.ToString());
            }
            if (((type == typeof(DateTime)) || (type == typeof(DateTime?))) && (type2 == typeof(string)))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return null;
                }
                return Convert.ToDateTime(value);
            }
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, value.ToString(), true);
                }
                catch
                {
                    return Enum.ToObject(type, value);
                }
            }
            if (((type == typeof(bool)) || (type == typeof(bool?))))
            {
                bool tempbool = false;
                if (bool.TryParse(value.ToString(), out tempbool))
                {
                    return tempbool;
                }
                else
                {
                    //处理  Request.Form  的 checkbox  如果没有返回值就是没有选中false  
                    if (string.IsNullOrEmpty(value.ToString()))
                        return false;
                    else
                    {
                        if (value.ToString() == "0")
                        {
                            return false;
                        }
                        return true;
                    }
                }

            }

            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }

            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TResult ConvertValue<TResult>(object value)
        {
            if (Convert.IsDBNull(value) || value == null)
                return default(TResult);

            object obj = ConvertValue(typeof(TResult), value);
            if (obj == null)
            {
                return default(TResult);
            }
            return (TResult)obj;
        }


        /// <summary>
        /// 快速执行Method
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object FastMethodInvoke(object obj, MethodInfo method, params object[] parameters)
        {
            return DynamicCalls.GetMethodInvoker(method)(obj, parameters);
        }

        /// <summary>
        /// 快速实例化一个T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>()
        {
            return (T)Create(typeof(T))();
        }

        /// <summary>
        /// 快速实例化一个FastCreateInstanceHandler
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FastCreateInstanceHandler Create(Type type)
        {
            return DynamicCalls.GetInstanceCreator(type);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(object obj, PropertyInfo property, object value)
        {
            if (property.CanWrite)
            {
                FastPropertySetHandler propertySetter = DynamicCalls.GetPropertySetter(property);
                value = ConvertValue(property.PropertyType, value);
                propertySetter(obj, value);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            SetPropertyValue(obj.GetType(), obj, propertyName, value);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(Type type, object obj, string propertyName, object value)
        {
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                SetPropertyValue(obj, property, value);
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<TEntity>(TEntity entity, string propertyName)
        {
            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(entity, null);
            }

            return null;
        }

        /// <summary>
        /// 从Entity数组转换成DataTable
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static DataTable EntityArrayToDataTable<TEntity>(TEntity[] entities)
            where TEntity : Entity
        {
            DataTable dt = new DataTable();
            if (entities == null || entities.Length == 0) return dt;

            Field[] fields = entities[0].GetFields();
            int fieldLength = fields.Length;
            foreach (Field field in fields)
            {
                dt.Columns.Add(field.Name);
            }

            foreach (TEntity entity in entities)
            {
                DataRow dtRow = dt.NewRow();
                object[] values = entity.GetValues();

                for (int i = 0; i < fieldLength; i++)
                {
                    dtRow[fields[i].Name] = values[i];
                }
                dt.Rows.Add(dtRow);
            }
            return dt;
        }


        /// <summary>
        /// DataRow转化为T
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static TEntity DataRowToEntity<TEntity>(DataRow row) where TEntity : Entity
        {
            //2015-08-10注释
            //TEntity entity = Create<TEntity>();
            //entity.SetPropertyValues(row);
            //return entity;

            //2015-08-10恢复注释
            TEntity local2;
            try
            {
                TEntity local = Create<TEntity>();
                Field[] fields = local.GetFields();
                Type type = typeof(TEntity);
                foreach (Field field in fields)
                {
                    if ((row.Table.Columns.Contains(field.Name)) && (null != row[field.Name]) && (!Convert.IsDBNull(row[field.Name])))
                    {
                        SetPropertyValue(type, local, field.PropertyName, row[field.Name]);
                    }
                }
                local2 = local;
            }
            catch
            {
                throw;
            }
            return local2;
        }

        /// <summary>
        /// DataTable转化为TEntity[]
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static TEntity[] DataTableToEntityArray<TEntity>(DataTable dt) where TEntity : Entity
        {
            return DataTableToEntityList<TEntity>(dt).ToArray();
        }

        /// <summary>
        /// DataTable转化为 List
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<TEntity> DataTableToEntityList<TEntity>(DataTable dt) where TEntity : Entity
        {
            List<TEntity> list = new List<TEntity>();
            if ((dt == null) || (dt.Rows.Count == 0))
                return list;


            foreach (DataRow row in dt.Rows)
            {
                list.Add(DataRowToEntity<TEntity>(row));
            }

            return list;
        }

        private static System.Text.RegularExpressions.Regex keyReg = new System.Text.RegularExpressions.Regex("[^a-zA-Z]", System.Text.RegularExpressions.RegexOptions.Compiled);
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static byte[] bt = new byte[5];
        private static Random rd = new Random();
        private static object obj = new object();
        private static readonly ThreadLocal<Random> appRandom = new ThreadLocal<Random>(() => new Random());

        public static int GetRandomNumber()
        {
            return appRandom.Value.Next();
        }

        public static int paramCount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetNewParamCount()
        {
            if (paramCount >= 9999)
            {
                paramCount = 0;
            }
            paramCount++;
            return paramCount;
        }

        /// <summary>
        /// 生成唯一字符串
        /// </summary>
        /// <returns></returns>
        public static string MakeUniqueKey(Field field)//string prefix,
        {
            //TODO 此处应该根据数据库类型来附加@、?、:
            //return string.Concat("@", field.tableName, "_", field.Name, "_", GetNewParamCount()).Replace(".","_");
            //如遇Oracle超过30字符Bug，把field.tableName去掉即可
            return string.Concat("@", field.Name, GetNewParamCount());
            //byte[] data = new byte[16];
            //new RNGCryptoServiceProvider().GetBytes(data);
            //string keystring = keyReg.Replace(Convert.ToBase64String(data).Trim(), string.Empty);

            //if (keystring.Length > 16)
            //    return keystring.Substring(0, 16).ToLower();

            //return keystring.ToLower();

            //return string.Concat(prefix, Guid.NewGuid().ToString().Replace("-", ""));
        }


        /// <summary>
        /// 生成主键条件
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static WhereClip GetPrimaryKeyWhere(Entity entity)
        {
            WhereClip where = new WhereClip();

            var keyfields = entity.GetPrimaryKeyFields();
            var allfields = entity.GetFields();
            var allValues = entity.GetValues();
            var fieldlength = allfields.Length;
            if (keyfields == null) return where;
            foreach (var pkField in keyfields)
            {
                for (int i = 0; i < fieldlength; i++)
                {
                    if (string.Compare(allfields[i].PropertyName, pkField.PropertyName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        where = where.And(new WhereClip(pkField, allValues[i], QueryOperator.Equal));
                        break;
                    }
                }

            }
            return where;
        }
        /// <summary>
        /// 生成主键条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        internal static WhereClip GetPrimaryKeyWhere<TEntity>(Array pkValues)//params object[] pkValues  2015-08-20
            where TEntity : Entity
        {
            WhereClip where = new WhereClip();
            Field[] keyfields = EntityCache.GetPrimaryKeyFields<TEntity>();

            if (keyfields == null)
                return where;

            Check.Require(keyfields.Length == pkValues.Length, "主键列与主键值无法对应!");

            int index = keyfields.Length;
            for (int i = 0; i < index; i++)
            {
                where = where.And(new WhereClip(keyfields[i], pkValues.GetValue(i), QueryOperator.Equal));
                //2015-08-20注释
                //where = where.And(new WhereClip(keyfields[i], pkValues[i], QueryOperator.Equal)); 
                //where = where.And(keyfields[i].In(pkValues));//2015-06-09
            }
            return where;
        }

        /// <summary>
        /// 数组转成字典
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<Field, object> FieldValueToDictionary(Field[] fields, object[] values)
        {
            Dictionary<Field, object> dic = new Dictionary<Field, object>();
            if (null == fields || fields.Length == 0)
                return dic;
            int length = fields.Length;

            for (int i = 0; i < length; i++)
            {
                dic.Add(fields[i], values[i]);
            }

            return dic;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string ToString(QueryOperator op)
        {
            switch (op)
            {
                case QueryOperator.Add:
                    return " + ";
                case QueryOperator.BitwiseAND:
                    return " & ";
                case QueryOperator.BitwiseNOT:
                    return " ~ ";
                case QueryOperator.BitwiseOR:
                    return " | ";
                case QueryOperator.BitwiseXOR:
                    return " ^ ";
                case QueryOperator.Divide:
                    return " / ";
                case QueryOperator.Equal:
                    return " = ";
                case QueryOperator.Greater:
                    return " > ";
                case QueryOperator.GreaterOrEqual:
                    return " >= ";
                case QueryOperator.IsNULL:
                    return " IS NULL ";
                case QueryOperator.IsNotNULL:
                    return " IS NOT NULL ";
                case QueryOperator.Less:
                    return " < ";
                case QueryOperator.LessOrEqual:
                    return " <= ";
                case QueryOperator.Like:
                    return " LIKE ";
                case QueryOperator.Modulo:
                    return " % ";
                case QueryOperator.Multiply:
                    return " * ";
                case QueryOperator.NotEqual:
                    return " <> ";
                case QueryOperator.Subtract:
                    return " - ";
            }

            throw new NotSupportedException("Unknown QueryOperator: " + op.ToString() + "!");
        }


        public static int GetEndIndexOfMethod(string cmdText, int startIndexOfCharIndex)
        {
            int foundEnd = -1;
            int endIndexOfCharIndex = 0;
            for (int i = startIndexOfCharIndex; i < cmdText.Length; ++i)
            {
                if (cmdText[i] == '(')
                {
                    --foundEnd;
                }
                else if (cmdText[i] == ')')
                {
                    ++foundEnd;
                }

                if (foundEnd == 0)
                {
                    endIndexOfCharIndex = i;
                    break;
                }
            }
            return endIndexOfCharIndex;
        }

        public static string[] SplitTwoParamsOfMethodBody(string bodyText)
        {
            int colonIndex = 0;
            int foundEnd = 0;
            for (int i = 1; i < bodyText.Length - 1; i++)
            {
                if (bodyText[i] == '(')
                {
                    --foundEnd;
                }
                else if (bodyText[i] == ')')
                {
                    ++foundEnd;
                }

                if (bodyText[i] == ',' && foundEnd == 0)
                {
                    colonIndex = i;
                    break;
                }
            }

            return new string[] { bodyText.Substring(0, colonIndex), bodyText.Substring(colonIndex + 1) };
        }

        internal static class DBConvert
        {
            public static bool IsDBNull(object value)
            {
                return object.Equals(DBNull.Value, value);
            }
            public static short ToInt16(object value)
            {
                if (value is short)
                {
                    return (short)value;
                }
                try
                {
                    return Convert.ToInt16(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static ushort ToUInt16(object value)
            {
                if (value is ushort)
                {
                    return (ushort)value;
                }
                try
                {
                    return Convert.ToUInt16(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static int ToInt32(object value)
            {
                if (value is int)
                {
                    return (int)value;
                }
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static uint ToUInt32(object value)
            {
                if (value is uint)
                {
                    return (uint)value;
                }
                try
                {
                    return Convert.ToUInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static long ToInt64(object value)
            {
                if (value is long)
                {
                    return (long)value;
                }
                try
                {
                    return Convert.ToInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static ulong ToUInt64(object value)
            {
                if (value is long)
                {
                    return (ulong)value;
                }
                try
                {
                    return Convert.ToUInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static bool ToBoolean(object value)
            {
                if (value == null)
                {
                    return false;
                }
                if (value is bool)
                {
                    return (bool)value;
                }
                if (value.Equals("1") || value.Equals("-1"))
                {
                    value = "true";
                }
                else if (value.Equals("0"))
                {
                    value = "false";
                }

                try
                {
                    return Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }
            public static DateTime ToDateTime(object value)
            {
                if (value is DateTime)
                {
                    return (DateTime)value;
                }
                try
                {
                    return Convert.ToDateTime(value);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            public static decimal ToDecimal(object value)
            {
                if (value is decimal)
                {
                    return (decimal)value;
                }
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static double ToDouble(object value)
            {
                if (value is double)
                {
                    return (double)value;
                }
                try
                {
                    return Convert.ToDouble(value);
                }
                catch
                {
                    return 0;
                }
            }
            //2015-09-22
            public static float ToFloat(object value)
            {
                if (value is Single || value is float)
                {
                    return (float)value;
                }
                try
                {
                    return Convert.ToSingle(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static Guid ToGuid(object value)
            {
                if (value is Guid)
                {
                    return (Guid)value;
                }
                try
                {
                    return Guid.Parse(value.ToString());
                }
                catch
                {
                    return new Guid();
                }
            }
            public static byte[] ToByteArr(object value)
            {
                var arr = value as byte[];
                return arr;
            }

            public static Nullable<short> ToNInt16(object value)
            {
                if (value is short)
                {
                    return new Nullable<short>((short)value);
                }
                try
                {
                    return new Nullable<short>(Convert.ToInt16(value));
                }
                catch
                {
                    return new Nullable<short>();
                }
            }
            public static Nullable<ushort> ToNUInt16(object value)
            {
                if (value is ushort)
                {
                    return new Nullable<ushort>((ushort)value);
                }
                try
                {
                    return new Nullable<ushort>(Convert.ToUInt16(value));
                }
                catch
                {
                    return new Nullable<ushort>();
                }
            }
            public static Nullable<int> ToNInt32(object value)
            {
                if (value is int)
                {
                    return new Nullable<int>((int)value);
                }
                try
                {
                    return new Nullable<int>(Convert.ToInt32(value));
                }
                catch
                {
                    return new Nullable<int>();
                }
            }
            public static Nullable<uint> ToNUInt32(object value)
            {
                if (value is uint)
                {
                    return new Nullable<uint>((uint)value);
                }
                try
                {
                    return new Nullable<uint>(Convert.ToUInt32(value));
                }
                catch
                {
                    return new Nullable<uint>();
                }
            }
            public static Nullable<long> ToNInt64(object value)
            {
                if (value is long)
                {
                    return new Nullable<long>((long)value);
                }
                try
                {
                    return new Nullable<long>(Convert.ToInt64(value));
                }
                catch
                {
                    return new Nullable<long>();
                }
            }
            public static Nullable<ulong> ToNUInt64(object value)
            {
                if (value is long)
                {
                    return new Nullable<ulong>((ulong)value);
                }
                try
                {
                    return new Nullable<ulong>(Convert.ToUInt64(value));
                }
                catch
                {
                    return new Nullable<ulong>();
                }
            }
            public static Nullable<bool> ToNBoolean(object value)
            {
                if (value is bool)
                {
                    return new Nullable<bool>((bool)value);
                }
                try
                {
                    return new Nullable<bool>(Convert.ToBoolean(value));
                }
                catch
                {
                    return new Nullable<bool>();
                }
            }
            public static Nullable<DateTime> ToNDateTime(object value)
            {
                if (value is DateTime)
                {
                    return new Nullable<DateTime>((DateTime)value);
                }
                try
                {
                    return new Nullable<DateTime>(Convert.ToDateTime(value));
                }
                catch
                {
                    return new Nullable<DateTime>();
                }
            }
            public static Nullable<decimal> ToNDecimal(object value)
            {
                if (value is decimal)
                {
                    return new Nullable<decimal>((decimal)value);
                }
                try
                {
                    return new Nullable<decimal>(Convert.ToDecimal(value));
                }
                catch
                {
                    return new Nullable<decimal>();
                }
            }
            public static Nullable<double> ToNDouble(object value)
            {
                if (value is double)
                {
                    return new Nullable<double>((double)value);
                }
                try
                {
                    return new Nullable<double>(Convert.ToDouble(value));
                }
                catch
                {
                    return new Nullable<double>();
                }
            }
            public static Nullable<float> ToNFloat(object value)
            {
                if (value is Single || value is float)
                {
                    return new Nullable<float>((float)value);
                }
                try
                {
                    return new Nullable<float>(Convert.ToSingle(value));
                }
                catch
                {
                    return new Nullable<float>();
                }
            }
            public static Nullable<Guid> ToNGuid(object value)
            {
                if (value is Guid)
                {
                    return new Nullable<Guid>((Guid)value);
                }
                try
                {
                    return new Nullable<Guid>(Guid.Parse(value.ToString()));
                }
                catch
                {
                    return new Nullable<Guid>();
                }
            }
        }
        private static T ConvertObj<T>(dynamic obj)
        {
            return (T)obj;
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("此方法仅供内部使用", false)]
        public static char ReadChar(object value)
        {
            if (value == null || value is DBNull) throw new ArgumentNullException("value");
            string s = value as string;
            if (s == null || s.Length != 1) throw new ArgumentException("A single-character was expected", "value");
            return s[0];
        }
        /// <summary>
        /// Internal use only
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is for internal usage only", false)]
        public static char? ReadNullableChar(object value)
        {
            if (value == null || value is DBNull) return null;
            string s = value as string;
            if (s == null || s.Length != 1) throw new ArgumentException("A single-character was expected", "value");
            return s[0];
        }
        /// <summary>
        /// Throws a data exception, only used internally
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="index"></param>
        /// <param name="reader"></param>
        public static void ThrowDataException(Exception ex, int index, IDataReader reader)
        {
            string name = "(n/a)", value = "(n/a)";
            if (reader != null && index >= 0 && index < reader.FieldCount)
            {
                name = reader.GetName(index);
                object val = reader.GetValue(index);
                if (val == null || val is DBNull)
                {
                    value = "<null>";
                }
                else
                {
                    value = Convert.ToString(val) + " - " + Type.GetTypeCode(val.GetType());
                }
            }
            if (!(index >= reader.FieldCount))
            {
                throw new DataException(string.Format("Error parsing column {0} ({1}={2})", index, name, value), ex);
            }
        }
    }

    public static class DosORMCommonExpand
    {
        private static Dictionary<MemberInfo, Object> _micache1 = new Dictionary<MemberInfo, Object>();
        private static Dictionary<MemberInfo, Object> _micache2 = new Dictionary<MemberInfo, Object>();
        /// <summary>
        /// 获取自定义特性，带有缓存功能，避免因.Net内部GetCustomAttributes没有缓存而带来的损耗
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo member, Boolean inherit)
        {
            if (member == null) return new TAttribute[0];

            // 根据是否可继承，分属两个缓存集合
            var cache = inherit ? _micache1 : _micache2;

            Object obj = null;
            if (cache.TryGetValue(member, out obj)) return (TAttribute[])obj;
            lock (cache)
            {
                if (cache.TryGetValue(member, out obj)) return (TAttribute[])obj;

                var atts = member.GetCustomAttributes(typeof(TAttribute), inherit) as TAttribute[];
                var att = atts == null ? new TAttribute[0] : atts;
                cache[member] = att;
                return att;
            }
        }
        /// <summary>获取自定义属性</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member, Boolean inherit)
        {
            var atts = member.GetCustomAttributes<TAttribute>(inherit);
            if (atts == null || atts.Length < 1) return default(TAttribute);
            return atts[0];
        }
    }
}
