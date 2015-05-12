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
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Data;
using System.Web.UI;
using Dos.ORM;
using Dos.ORM.Common;

namespace Dos.ORM.Common
{

    /// <summary>
    /// 实体帮助类
    /// </summary>
    public class EntityUtils
    {
        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="t">model</param>
        public static void UpdateModel<T>(T t)
            where T : Entity
        {
            UpdateModel<T>(t, HttpContext.Current.Request.Form);
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="t">model</param>
        /// <param name="prefix">前缀</param>
        public static void UpdateModel<T>(T t, string prefix)
            where T : Entity
        {
            UpdateModel<T>(t, HttpContext.Current.Request.Form, prefix);
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="t">model</param>
        /// <param name="form">Request</param>
        public static void UpdateModel<T>(T t, NameValueCollection form)
            where T : Entity
        {
            UpdateModel<T>(t, form, string.Empty);
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="t">Entity</param>
        /// <param name="form">Request</param>
        /// <param name="prefix">前缀</param>
        public static void UpdateModel<T>(T t, NameValueCollection form, string prefix)
             where T : Entity
        {
            Field[] fields = EntityCache.GetFields<T>();
            string temp = prefix ?? string.Empty;

            foreach (Field f in fields)
            {
                string tempValue = form[temp + f.PropertyName];

                if (null != tempValue)
                {
                    string propertyName = f.PropertyName;
                    PropertyInfo property = t.GetType().GetProperty(propertyName);
                    if (null == property)
                    {
                        propertyName = string.Concat(propertyName.Substring(0, 1).ToUpper(), propertyName.Substring(1));
                        property = t.GetType().GetProperty(propertyName);
                    }
                    if (null != property)
                    {
                        DataUtils.SetPropertyValue(t, property, tempValue);
                    }
                }
            }
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="t">Entity</param>      
        /// <returns>转换是否成功</returns>
        public static bool TryUpdateModel<T>(T t)
             where T : Entity
        {
            return TryUpdateModel<T>(t, HttpContext.Current.Request.Form);
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="t">Entity</param>    
        /// <param name="prefix">前缀</param>
        /// <returns>转换是否成功</returns>
        public static bool TryUpdateModel<T>(T t, string prefix)
             where T : Entity
        {
            return TryUpdateModel<T>(t, HttpContext.Current.Request.Form, prefix);
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="t">Entity</param>
        /// <param name="form">Request</param>
        /// <returns>转换是否成功</returns>
        public static bool TryUpdateModel<T>(T t, NameValueCollection form)
             where T : Entity
        {
            return TryUpdateModel<T>(t, form, string.Empty);
        }

        /// <summary>
        /// 为Entity赋值
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="t">Entity</param>
        /// <param name="form">Request</param>
        /// <param name="prefix">前缀</param>
        /// <returns>转换是否成功</returns>
        public static bool TryUpdateModel<T>(T t, NameValueCollection form, string prefix)
             where T : Entity
        {
            bool success = true;
            Field[] fields = EntityCache.GetFields<T>();
            string temp = prefix ?? string.Empty;
            foreach (Field f in fields)
            {
                string tempValue = form[temp + f.PropertyName];
                if (null != tempValue)
                {
                    try
                    {
                        string propertyName = f.PropertyName;
                        PropertyInfo property = t.GetType().GetProperty(propertyName);
                        if (null == property)
                        {
                            propertyName = string.Concat(propertyName.Substring(0, 1).ToUpper(), propertyName.Substring(1));
                            property = t.GetType().GetProperty(propertyName);
                        }
                        if (null != property)
                        {
                            DataUtils.SetPropertyValue(t, property, tempValue);
                        }
                    }
                    catch
                    {
                        success = false;
                    }
                }
            }
            return success;
        }


        /// <summary>
        /// 设置Web页面的值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public static void SetDocumentValue<TEntity>(TEntity entity)
             where TEntity : Entity
        {
            SetDocumentValue<TEntity>(entity, null);
        }

        /// <summary>
        /// 设置Web页面的值
        /// </summary>
        /// <param name="fieldvalues"></param>
        public static void SetDocumentValue(Dictionary<Field, object> fieldvalues)
        {
            SetDocumentValue(fieldvalues, null);
        }

        /// <summary>
        /// 设置Web页面的值
        /// </summary>
        /// <param name="fieldvalues"></param>
        /// <param name="prefix"></param>
        public static void SetDocumentValue(Dictionary<Field, object> fieldvalues, string prefix)
        {
            if (null == fieldvalues || fieldvalues.Count == 0)
                return;

            Page page = HttpContext.Current.Handler as Page;
            if (null != page)
            {
                page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(), SetDocumentValueString(fieldvalues, prefix), true);
            }
        }

        /// <summary>
        /// 返回赋值的脚本
        /// </summary>
        /// <param name="fieldvalues"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string SetDocumentValueString(Dictionary<Field, object> fieldvalues, string prefix)
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine();
            foreach (KeyValuePair<Field, object> fieldvalue in fieldvalues)
            {
                string varname = prefix + fieldvalue.Key.PropertyName;
                script.Append("var ");
                script.Append(varname);
                script.Append("=document.getElementById('");
                script.Append(varname);
                script.Append("');if(");
                script.Append(varname);
                script.Append(")");
                if (null == fieldvalue.Value || Convert.IsDBNull(fieldvalue.Value))
                {
                    script.Append(varname);
                    script.Append(".value='';");
                }
                else
                {
                    Type valueType = fieldvalue.Value.GetType();
                    if (valueType == typeof(bool))
                    {
                        script.Append("{try{");
                        script.Append(varname);
                        script.Append(".checked=");
                        script.Append((bool)fieldvalue.Value ? "true" : "false");
                        script.Append(";}catch(err){");
                        script.Append(varname);
                        script.Append(".value='");
                        script.Append((bool)fieldvalue.Value ? "1" : "0");
                        script.AppendLine("'}}");
                    }
                    else
                    {
                        script.Append(varname);
                        script.Append(".value='");
                        if (valueType == typeof(string))
                        {
                            if (!string.IsNullOrEmpty(fieldvalue.Value.ToString()))
                                script.Append(fieldvalue.Value.ToString().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\'", "\\'"));

                        }
                        else if (valueType == typeof(DateTime))
                        {
                            script.Append(fieldvalue.Value.ToString().Replace(" 0:00:00", ""));
                        }
                        else
                        {
                            script.Append(fieldvalue.Value);
                        }
                        script.AppendLine("';");
                    }
                }
            }
            return script.ToString();
        }

        /// <summary>
        /// 返回赋值的脚本
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="prefix"></param>
        /// <param name="entity"></param>
        public static string SetDocumentValueString<TEntity>(TEntity entity, string prefix)
             where TEntity : Entity
        {
            if (null == entity)
                return string.Empty;

            return SetDocumentValueString(entity.GetFields(), entity.GetValues(), prefix);
        }

        /// <summary>
        /// 返回赋值的脚本
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string SetDocumentValueString(Field[] fields, object[] values, string prefix)
        {
            return SetDocumentValueString(DataUtils.FieldValueToDictionary(fields, values), prefix);
        }


        /// <summary>
        /// 设置Web页面的值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="prefix"></param>
        /// <param name="entity"></param>
        public static void SetDocumentValue<TEntity>(TEntity entity, string prefix)
             where TEntity : Entity
        {
            if (null == entity)
                return;

            SetDocumentValue(entity.GetFields(), entity.GetValues(), prefix);
        }

        /// <summary>
        /// 设置Web页面值
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        public static void SetDocumentValue(Field[] fields, object[] values)
        {
            SetDocumentValue(fields, values, null);
        }


        /// <summary>
        /// 设置Web页面值
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="prefix"></param>
        public static void SetDocumentValue(Field[] fields, object[] values, string prefix)
        {
            SetDocumentValue(DataUtils.FieldValueToDictionary(fields, values), prefix);
        }


        /// <summary>
        /// 清空Web页面的值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="prefix"></param>
        public static void SetDocumentValueClear<TEntity>(string prefix)
             where TEntity : Entity
        {
            Page page = HttpContext.Current.Handler as Page;
            if (null != page)
            {
                page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(), SetDocumentValueClearString<TEntity>(prefix), true);
            }
        }

        /// <summary>
        /// 返回清空Web页面的值的脚本
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="prefix"></param>
        public static string SetDocumentValueClearString<TEntity>(string prefix)
             where TEntity : Entity
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine();
            TEntity t = DataUtils.Create<TEntity>();
            Dictionary<Field, object> fieldvalues = DataUtils.FieldValueToDictionary(t.GetFields(), t.GetValues());
            foreach (KeyValuePair<Field, object> fieldvalue in fieldvalues)
            {
                string varname = prefix + fieldvalue.Key.PropertyName;
                script.Append("var ");
                script.Append(varname);
                script.Append("=document.getElementById('");
                script.Append(varname);
                script.Append("');if(");
                script.Append(varname);
                script.Append(")");
                if (null == fieldvalue.Value || Convert.IsDBNull(fieldvalue.Value))
                {
                    script.Append(varname);
                    script.Append(".value='';");
                }
                else
                {
                    Type valueType = fieldvalue.Value.GetType();
                    if (valueType == typeof(bool))
                    {
                        script.Append("{try{");
                        script.Append(varname);
                        script.Append(".checked=false");
                        script.Append(";}catch(err){");
                        script.Append(varname);
                        script.Append(".value='0'}}");
                    }
                    else
                    {
                        script.Append(varname);
                        script.Append(".value='';");
                    }
                }
            }
            return script.ToString();
        }



        /// <summary>
        /// 根据字段名返回实体字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns>返回null 就表示字段不存在</returns>
        public static Field GetField<TEntity>(string fieldName)
             where TEntity : Entity
        {
            Field[] fields = EntityCache.GetFields<TEntity>();

            foreach (Field field in fields)
            {
                if (string.Compare(fieldName, field.PropertyName, true) == 0)
                    return field;
            }

            return null;
        }


        /// <summary>
        /// 实体赋值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="toEntity">被赋值实体</param>
        /// <param name="fromEntity">来源实体</param>
        /// <returns>返回null 就表示fromEntity==null</returns>
        public static void SetValue<TEntity>(TEntity toEntity, TEntity fromEntity)
             where TEntity : Entity
        {
            if (null == fromEntity)
                toEntity = null;
            else
            {

                if (null == toEntity)
                    toEntity = DataUtils.Create<TEntity>();

                toEntity.Attach();

                PropertyInfo[] pis = toEntity.GetType().GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    DataUtils.SetPropertyValue(toEntity, pi, DataUtils.GetPropertyValue<TEntity>(fromEntity, pi.Name));
                }

            }

        }
    }
}
