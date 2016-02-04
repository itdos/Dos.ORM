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
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Data;
using System.Threading;
using System.Web.UI;
using Dos.ORM;
using Dos.ORM.Common;

namespace Dos.ORM
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
        //public static TResult EntityCopy<TResult>(object input)
        //{
        //    if (input == null)
        //    {
        //        return default(TResult);
        //    }
        //    if (input.GetType() == typeof(TResult))
        //    {
        //        return (TResult)input;
        //    }
        //    return (TResult)EntityCopy(input, typeof(TResult));
        //}

        //public static List<TResult> EntityCopy<TEntity, TResult>(IList<TEntity> input)
        //{
        //    return input.Select(entity => EntityCopy<TResult>(entity)).ToList();
        //}

        //private static object EntityCopy(object input, Type targetType)
        //{
        //    var objResult = Activator.CreateInstance(targetType);
        //    var properties = targetType.GetProperties();
        //    var type = input.GetType();
        //    foreach (var info in properties)
        //    {
        //        if (!info.CanWrite) continue;
        //        var property = type.GetProperty(info.Name);
        //        if (property == null) continue;
        //        var objTemp = property.GetValue(input, null);
        //        info.SetValue(objResult, objTemp, null);
        //    }
        //    return objResult;
        //}
        //        internal class Mapper
        //        {
        //            private static readonly MethodInfo Object_ToString = typeof(object).GetMethod("ToString");
        //            private static readonly MethodInfo Reader_Read = typeof(IDataReader).GetMethod("Read");
        //            private static readonly MethodInfo Reader_GetValues = typeof(IDataRecord).GetMethod("GetValues", new Type[] { typeof(object[]) });
        //            private static readonly MethodInfo Convert_IsDBNull = typeof(DataUtils.DBConvert).GetMethod("IsDBNull", new Type[] { typeof(object) });

        //            private static readonly MethodInfo Convert_ToInt16 = typeof(DataUtils.DBConvert).GetMethod("ToInt16", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToInt32 = typeof(DataUtils.DBConvert).GetMethod("ToInt32", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToInt64 = typeof(DataUtils.DBConvert).GetMethod("ToInt64", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToBoolean = typeof(DataUtils.DBConvert).GetMethod("ToBoolean", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToDateTime = typeof(DataUtils.DBConvert).GetMethod("ToDateTime", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToDecimal = typeof(DataUtils.DBConvert).GetMethod("ToDecimal", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToDouble = typeof(DataUtils.DBConvert).GetMethod("ToDouble", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToFloat = typeof(DataUtils.DBConvert).GetMethod("ToFloat", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToGuid = typeof(DataUtils.DBConvert).GetMethod("ToGuid", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToByteArr = typeof(DataUtils.DBConvert).GetMethod("ToByteArr", new Type[] { typeof(object) });

        //            private static readonly MethodInfo Convert_ToNullInt16 = typeof(DataUtils.DBConvert).GetMethod("ToNInt16", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullInt32 = typeof(DataUtils.DBConvert).GetMethod("ToNInt32", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullInt64 = typeof(DataUtils.DBConvert).GetMethod("ToNInt64", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullBoolean = typeof(DataUtils.DBConvert).GetMethod("ToNBoolean", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullDateTime = typeof(DataUtils.DBConvert).GetMethod("ToNDateTime", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullDecimal = typeof(DataUtils.DBConvert).GetMethod("ToNDecimal", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullDouble = typeof(DataUtils.DBConvert).GetMethod("ToNDouble", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullFloat = typeof(DataUtils.DBConvert).GetMethod("ToNFloat", new Type[] { typeof(object) });
        //            private static readonly MethodInfo Convert_ToNullGuid = typeof(DataUtils.DBConvert).GetMethod("ToNGuid", new Type[] { typeof(object) });

        //            private delegate T ReadEntityInvoker<T>(IDataReader dr);
        //            private static Dictionary<string, DynamicMethod> m_CatchMethod;
        //            public static List<T> Map<T>(IDataReader reader)
        //            {
        //                if (reader == null || reader.IsClosed)
        //                {
        //                    throw new Exception("连接已关闭！");
        //                }
        //                #region  Not use the cache for the time being  2015-06-14
        //                //if (m_CatchMethod == null)
        //                //{
        //                //    m_CatchMethod = new Dictionary<string, DynamicMethod>();
        //                //}
        //                #endregion
        //                Type itemType = typeof(T);
        //                T t = DataUtils.Create<T>();
        //                var key = itemType.FullName;
        //                //if (!m_CatchMethod.ContainsKey(key))//Not use the cache for the time being  2015-06-14
        //                //{
        //#if WRITE_FILE
        //                AssemblyName aName = new AssemblyName("DynamicAssembly");
        //                AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
        //                ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
        //                TypeBuilder tb = mb.DefineType("DynamicType", TypeAttributes.Public);
        //#endif
        //                Type listType = typeof(List<T>);
        //                Type objectType = typeof(object);
        //                Type objArrayType = typeof(object[]);
        //                Type boolType = typeof(bool);
        //                Type[] methodArgs = { typeof(IDataReader) };
        //                MethodInfo LAdd = listType.GetMethod("Add");
        //                PropertyInfo[] properties = null;
        //                getMapped(t, itemType, reader, out properties);
        //#if WRITE_FILE
        //                MethodBuilder dm = tb.DefineMethod("ReadEntities", MethodAttributes.Public| MethodAttributes.Static, listType, methodArgs);
        //#else
        //                DynamicMethod dm = new DynamicMethod("ReadEntities", listType, methodArgs, typeof(Mapper));
        //#endif
        //                ILGenerator ilg = dm.GetILGenerator();
        //                //List<T> list;
        //                LocalBuilder list = ilg.DeclareLocal(listType);
        //                //T item;
        //                LocalBuilder item = ilg.DeclareLocal(itemType);
        //                //object[] values;
        //                LocalBuilder values = ilg.DeclareLocal(objArrayType);
        //                //object objValue;
        //                LocalBuilder objValue = ilg.DeclareLocal(objectType);
        //                //type nulls
        //                LocalBuilder[] typeNulls = null;
        //                initNulls(properties, ilg, out typeNulls);

        //                Label exit = ilg.DefineLabel();
        //                Label loop = ilg.DefineLabel();
        //                Label[] lblArray = new Label[properties.Length * 2];
        //                for (int i = 0; i < lblArray.Length; i++)
        //                {
        //                    lblArray[i] = ilg.DefineLabel();
        //                }
        //                //list = new List<T>();
        //                ilg.Emit(OpCodes.Newobj, listType.GetConstructor(Type.EmptyTypes));
        //                ilg.Emit(OpCodes.Stloc_S, list);

        //                //values=new object[FieldCount];
        //                ilg.Emit(OpCodes.Ldc_I4, reader.FieldCount);
        //                ilg.Emit(OpCodes.Newarr, objectType);
        //                ilg.Emit(OpCodes.Stloc_S, values);

        //                // while (arg.Read()) {
        //                ilg.MarkLabel(loop);
        //                ilg.Emit(OpCodes.Ldarg_0);
        //                ilg.Emit(OpCodes.Callvirt, Reader_Read);
        //                ilg.Emit(OpCodes.Brfalse, exit);

        //                //reader.GetValues(values);
        //                ilg.Emit(OpCodes.Ldarg_0);
        //                ilg.Emit(OpCodes.Ldloc_S, values);
        //                ilg.Emit(OpCodes.Callvirt, Reader_GetValues);
        //                ilg.Emit(OpCodes.Pop);

        //                //item=new T(); 
        //                ilg.Emit(OpCodes.Newobj, itemType.GetConstructor(Type.EmptyTypes));
        //                ilg.Emit(OpCodes.Stloc_S, item);

        //                //item.Property=Convert(values[index]);
        //                for (int index = 0; index < properties.Length; index++)
        //                {
        //                    PropertyInfo pi = properties[index];
        //                    if (pi == null)
        //                    {
        //                        continue;
        //                    }

        //                    //objValue=value[index];
        //                    ilg.Emit(OpCodes.Ldloc_S, values);
        //                    ilg.Emit(OpCodes.Ldc_I4, index);
        //                    ilg.Emit(OpCodes.Ldelem_Ref);
        //                    ilg.Emit(OpCodes.Stloc_S, objValue);

        //                    //tmpBool=Convert.IsDBNull(objValue);
        //                    ilg.Emit(OpCodes.Ldloc_S, objValue);
        //                    ilg.Emit(OpCodes.Call, Convert_IsDBNull);

        //                    //if (!tmpBool){
        //                    ilg.Emit(OpCodes.Brtrue_S, lblArray[index * 2]);

        //                    //item.Field=Convert(objValue).ToXXX();
        //                    ilg.Emit(OpCodes.Ldloc_S, item);
        //                    ilg.Emit(OpCodes.Ldloc_S, objValue);

        //                    convertValue(ilg, pi);

        //                    ilg.Emit(OpCodes.Callvirt, pi.GetSetMethod());
        //                    //}
        //                    ilg.Emit(OpCodes.Br_S, lblArray[index * 2 + 1]);
        //                    //else {
        //                    ilg.MarkLabel(lblArray[index * 2]);
        //                    //item.Field=objValue;
        //                    ilg.Emit(OpCodes.Ldloc_S, item);
        //                    ilg.Emit(OpCodes.Ldloc_S, typeNulls[index]);
        //                    ilg.Emit(OpCodes.Callvirt, pi.GetSetMethod());
        //                    //}
        //                    ilg.MarkLabel(lblArray[index * 2 + 1]);
        //                }

        //                //list.Add(item);
        //                ilg.Emit(OpCodes.Ldloc_S, list);
        //                ilg.Emit(OpCodes.Ldloc_S, item);
        //                ilg.Emit(OpCodes.Callvirt, LAdd);
        //                //}
        //                ilg.Emit(OpCodes.Br, loop);
        //                ilg.MarkLabel(exit);

        //                // return list;
        //                ilg.Emit(OpCodes.Ldloc_S, list);
        //                ilg.Emit(OpCodes.Ret);
        //#if WRITE_FILE
        //                Type t = tb.CreateType();
        //                ab.Save(aName.Name + ".dll");
        //#else
        //                //m_CatchMethod.Add(key, dm);//Not use the cache for the time being  2015-06-14
        //#endif
        //                //if (m_CatchMethod.Count > 100)Not use the cache for the time being  2015-06-14
        //                {

        //                }
        //                //}

        //                //if (m_CatchMethod.ContainsKey(key))Not use the cache for the time being  2015-06-14
        //                //{
        //                //DynamicMethod dm = m_CatchMethod[key];//Not use the cache for the time being  2015-06-14
        //                ReadEntityInvoker<List<T>> invoker = dm.CreateDelegate(typeof(ReadEntityInvoker<List<T>>)) as ReadEntityInvoker<List<T>>;
        //                try// 2015-07-02
        //                {
        //                    return invoker.Invoke(reader);
        //                }
        //                catch (Exception)
        //                {
        //                    reader.Close();
        //                    throw;
        //                }
        //                //}
        //                throw new Exception("没有找到对应类型的处理方法。");
        //            }
        //            private static void initNulls(PropertyInfo[] properties, ILGenerator ilg, out LocalBuilder[] typeNulls)
        //            {
        //                typeNulls = new LocalBuilder[properties.Length];
        //                for (int index = 0; index < properties.Length; index++)
        //                {
        //                    PropertyInfo pi = properties[index];
        //                    if (pi != null)
        //                    {
        //                        typeNulls[index] = ilg.DeclareLocal(pi.PropertyType);
        //                        if (pi.PropertyType.IsValueType)
        //                        {
        //                            ilg.Emit(OpCodes.Ldloca_S, typeNulls[index]);
        //                            ilg.Emit(OpCodes.Initobj, pi.PropertyType);
        //                        }
        //                        else
        //                        {
        //                            ilg.Emit(OpCodes.Ldnull);
        //                            ilg.Emit(OpCodes.Stloc_S, typeNulls[index]);
        //                        }
        //                    }
        //                }
        //            }
        //            private static void convertValue(ILGenerator ilg, PropertyInfo pi)
        //            {
        //                TypeCode code = Type.GetTypeCode(pi.PropertyType);
        //                switch (code)
        //                {
        //                    case TypeCode.Int16:
        //                        ilg.Emit(OpCodes.Call, Convert_ToInt16);
        //                        return;
        //                    case TypeCode.Int32:
        //                        ilg.Emit(OpCodes.Call, Convert_ToInt32);
        //                        return;
        //                    case TypeCode.Int64:
        //                        ilg.Emit(OpCodes.Call, Convert_ToInt64);
        //                        return;
        //                    case TypeCode.Boolean:
        //                        ilg.Emit(OpCodes.Call, Convert_ToBoolean);
        //                        return;
        //                    case TypeCode.String:
        //                        ilg.Emit(OpCodes.Callvirt, Object_ToString);
        //                        return;
        //                    case TypeCode.DateTime:
        //                        ilg.Emit(OpCodes.Call, Convert_ToDateTime);
        //                        return;
        //                    case TypeCode.Decimal:
        //                        ilg.Emit(OpCodes.Call, Convert_ToDecimal);
        //                        return;
        //                    case TypeCode.Double:
        //                        ilg.Emit(OpCodes.Call, Convert_ToDouble);
        //                        return;
        //                    case TypeCode.Single:
        //                        ilg.Emit(OpCodes.Call, Convert_ToFloat);
        //                        return;
        //                }
        //                Type type = Nullable.GetUnderlyingType(pi.PropertyType);
        //                if (type != null)
        //                {
        //                    code = Type.GetTypeCode(type);
        //                    switch (code)
        //                    {
        //                        case TypeCode.Int16:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullInt16);
        //                            return;
        //                        case TypeCode.Int32:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullInt32);
        //                            return;
        //                        case TypeCode.Int64:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullInt64);
        //                            return;
        //                        case TypeCode.Boolean:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullBoolean);
        //                            return;
        //                        case TypeCode.DateTime:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullDateTime);
        //                            return;
        //                        case TypeCode.Decimal:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullDecimal);
        //                            return;
        //                        case TypeCode.Double:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullDouble);
        //                            return;
        //                        case TypeCode.Single:
        //                            ilg.Emit(OpCodes.Call, Convert_ToNullFloat);
        //                            return;
        //                    }
        //                    if (type.Name == "Guid")
        //                    {
        //                        ilg.Emit(OpCodes.Call, Convert_ToNullGuid);
        //                        return;
        //                    }
        //                }
        //                if (pi.PropertyType.Name == "Guid")
        //                {
        //                    ilg.Emit(OpCodes.Call, Convert_ToGuid);
        //                    return;
        //                }
        //                else if (pi.PropertyType.Name == "Byte[]")
        //                {
        //                    ilg.Emit(OpCodes.Call, Convert_ToByteArr);
        //                    return;
        //                }
        //                throw new Exception(string.Format("不支持\"{0}\"类型的转换！", pi.PropertyType.Name));
        //            }
        //            private static void getMapped<T>(T tt, Type type, IDataReader reader, out PropertyInfo[] mappedProerties)
        //            {
        //                mappedProerties = new PropertyInfo[reader.FieldCount];
        //                string[] fields = new string[reader.FieldCount];
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    fields[i] = reader.GetName(i);
        //                }
        //                List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    var isOk = false;
        //                    foreach (PropertyInfo pt in properties)
        //                    {
        //                        FieldAttribute fa = Attribute.GetCustomAttribute(pt, typeof(FieldAttribute)) as FieldAttribute;
        //                        if ((fa != null && string.Compare(fa.Field, fields[i], true) == 0) ||
        //                            string.Compare(pt.Name, fields[i], true) == 0)
        //                        {
        //                            properties.Remove(pt);
        //                            mappedProerties[i] = pt;
        //                            isOk = true;
        //                            break;
        //                        }
        //                    }
        //                    #region 2015-09-14新增
        //                    if (!isOk && tt is Entity)
        //                    {
        //                        if (properties.Any(d => d.Name == "F" + fields[i]))
        //                        {
        //                            var tempItem = properties.First(d => d.Name == "F" + fields[i]);
        //                            properties.Remove(tempItem);
        //                            mappedProerties[i] = tempItem;
        //                        }
        //                        else if (properties.Any(d => d.Name == "_" + fields[i]))
        //                        {
        //                            var tempItem = properties.First(d => d.Name == "_" + fields[i]);
        //                            properties.Remove(tempItem);
        //                            mappedProerties[i] = tempItem;
        //                        }
        //                        //var aa = (tt as Entity).GetFields().Where(d => d.Name == fields[i]);
        //                        //if (aa.Any())
        //                        //{

        //                        //}
        //                    }
        //                    #endregion
        //                }
        //            }
        //        }
        static readonly Dictionary<Type, DbType> typeMap;
        static EntityUtils()
        {
            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;

            FastExpandoDescriptionProvider provider = new FastExpandoDescriptionProvider();
            TypeDescriptor.AddProvider(provider, typeof(FastExpando));
        }
        private const string LinqBinary = "System.Data.Linq.Binary";
        public static Func<IDataReader, object> GetDeserializer(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            if (type == typeof(object)
                || type == typeof(FastExpando))
            {
                return GetDynamicDeserializer(reader, startBound, length, returnNullIfFirstMissing);
            }

            if (!(typeMap.ContainsKey(type) || type.FullName == LinqBinary))
            {
                return GetTypeDeserializer(type, reader, startBound, length, returnNullIfFirstMissing);
            }
            return GetStructDeserializer(type, startBound);
        }
        private static Func<IDataReader, object> GetStructDeserializer(Type type, int index)
        {
            if (type == typeof(char))
            {
                return r => DataUtils.ReadChar(r.GetValue(index));
            }
            if (type == typeof(char?))
            {
                return r => DataUtils.ReadNullableChar(r.GetValue(index));
            }
            if (type.FullName == LinqBinary)
            {
                return r => Activator.CreateInstance(type, r.GetValue(index));
            }
            if (type == typeof(bool))
            {
                return r =>
                {
                    var val = r.GetValue(index);
                    return val == DBNull.Value ? false : (val.GetType() == type ? val : Convert.ToBoolean(val));
                };
            }
            if (type == typeof(bool?))
            {
                return r =>
                {
                    var val = r.GetValue(index);
                    return val == DBNull.Value ? null : (val.GetType() == type ? val : Convert.ToBoolean(val));
                };
            }
            return r =>
            {
                var val = r.GetValue(index);
                return val is DBNull ? null : val;
            };
        }
        class PropInfo
        {
            public string Name { get; set; }
            public MethodInfo Setter { get; set; }
            public Type Type { get; set; }
        }
        static List<PropInfo> GetSettableProps(Type t)
        {
            return t
                  .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                  .Select(p => new PropInfo
                  {
                      Name = p.Name,
                      Setter = p.DeclaringType == t ? p.GetSetMethod(true) : p.DeclaringType.GetProperty(p.Name).GetSetMethod(true),
                      Type = p.PropertyType
                  })
                  .Where(info => info.Setter != null)
                  .ToList();
        }
        static List<FieldInfo> GetSettableFields(Type t)
        {
            return t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        }
        private static void EmitInt32(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }
        static readonly MethodInfo
                enumParse = typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.GetIndexParameters().Any() && p.GetIndexParameters()[0].ParameterType == typeof(int))
                    .Select(p => p.GetGetMethod()).First();
        public static Func<IDataReader, object> GetTypeDeserializer(Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false)
        {
            var dm = new DynamicMethod(string.Format("Deserialize{0}", Guid.NewGuid()), typeof(object), new[] { typeof(IDataReader) }, true);

            var il = dm.GetILGenerator();
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(type);
            bool haveEnumLocal = false;
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_0);
            var properties = GetSettableProps(type);
            var fields = GetSettableFields(type);
            if (length == -1)
            {
                length = reader.FieldCount - startBound;
            }

            if (reader.FieldCount <= startBound)
            {
                throw new ArgumentException("reader.FieldCount <= startBound", "splitOn");
            }

            var names = new List<string>();

            for (int i = startBound; i < startBound + length; i++)
            {
                names.Add(reader.GetName(i));
            }

            var setters = (
                            from n in names
                            let prop = properties.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.Ordinal))
                                  ?? properties.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.OrdinalIgnoreCase))
                            let field = prop != null ? null : (fields.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.Ordinal))
                                ?? fields.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.OrdinalIgnoreCase)))
                            select new { Name = n, Property = prop, Field = field }
                          ).ToList();

            int index = startBound;

            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)1);
                il.Emit(OpCodes.Initobj, type);
            }
            else
            {
                il.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
                il.Emit(OpCodes.Stloc_1);
            }
            il.BeginExceptionBlock();
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)1);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_1);
            }
            bool first = true;
            var allDone = il.DefineLabel();
            foreach (var item in setters)
            {
                if (item.Property != null || item.Field != null)
                {
                    il.Emit(OpCodes.Dup);
                    Label isDbNullLabel = il.DefineLabel();
                    Label finishLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_0);
                    EmitInt32(il, index);
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Callvirt, getItem);

                    Type memberType = item.Property != null ? item.Property.Type : item.Field.FieldType;

                    if (memberType == typeof(char) || memberType == typeof(char?))
                    {
                        il.EmitCall(OpCodes.Call, typeof(DataUtils).GetMethod(
                            memberType == typeof(char) ? "ReadChar" : "ReadNullableChar", BindingFlags.Static | BindingFlags.Public), null);
                    }
                    //else if (memberType == typeof(bool) || memberType == typeof(bool?))
                    //{
                    //    il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(
                    //        memberType == typeof(bool) ? "ReadBoolean" : "ReadNullableBoolean", BindingFlags.Static | BindingFlags.Public), null);
                    //}
                    else
                    {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Isinst, typeof(DBNull));
                        il.Emit(OpCodes.Brtrue_S, isDbNullLabel);
                        var nullUnderlyingType = Nullable.GetUnderlyingType(memberType);
                        var unboxType = nullUnderlyingType != null && nullUnderlyingType.IsEnum ? nullUnderlyingType : memberType;
                        if (unboxType.IsEnum)
                        {
                            if (!haveEnumLocal)
                            {
                                il.DeclareLocal(typeof(string));
                                haveEnumLocal = true;
                            }

                            Label isNotString = il.DefineLabel();
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Isinst, typeof(string));
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Stloc_2);
                            il.Emit(OpCodes.Brfalse_S, isNotString);
                            il.Emit(OpCodes.Pop);
                            il.Emit(OpCodes.Ldtoken, unboxType);
                            il.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
                            il.Emit(OpCodes.Ldloc_2);
                            il.Emit(OpCodes.Ldc_I4_1);
                            il.EmitCall(OpCodes.Call, enumParse, null);
                            il.Emit(OpCodes.Unbox_Any, unboxType);
                            if (nullUnderlyingType != null)
                            {
                                il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType }));
                            }
                            if (item.Property != null)
                            {
                                il.Emit(OpCodes.Callvirt, item.Property.Setter);
                            }
                            else
                            {
                                il.Emit(OpCodes.Stfld, item.Field);
                            }
                            il.Emit(OpCodes.Br_S, finishLabel);
                            il.MarkLabel(isNotString);
                        }
                        if (memberType.FullName == LinqBinary)
                        {
                            il.Emit(OpCodes.Unbox_Any, typeof(byte[]));
                            il.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[] { typeof(byte[]) }));
                        }
                        else if (memberType == typeof(bool) || memberType == typeof(bool?))
                        {
                            il.EmitCall(OpCodes.Call, typeof(Convert).GetMethod("ToBoolean", new Type[] { typeof(object) }), null);
                        }
                        else
                        {
                            il.EmitCall(OpCodes.Call, typeof(DataUtils).GetMethod("ConvertObj", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType), null);
                        }
                        if (nullUnderlyingType != null && (nullUnderlyingType.IsEnum || nullUnderlyingType == typeof(bool)))
                        {
                            il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType }));
                        }
                    }
                    if (item.Property != null)
                    {
                        if (type.IsValueType)
                        {
                            il.Emit(OpCodes.Call, item.Property.Setter);
                        }
                        else
                        {
                            il.Emit(OpCodes.Callvirt, item.Property.Setter);
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Stfld, item.Field);
                    }
                    il.Emit(OpCodes.Br_S, finishLabel);
                    il.MarkLabel(isDbNullLabel);
                    il.Emit(OpCodes.Pop);
                    il.Emit(OpCodes.Pop);

                    if (first && returnNullIfFirstMissing)
                    {
                        il.Emit(OpCodes.Pop);
                        il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Stloc_1);
                        il.Emit(OpCodes.Br, allDone);
                    }
                    il.MarkLabel(finishLabel);
                    first = false;
                }
                //first = false;
                index += 1;
            }
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Pop);
            }
            else
            {
                il.Emit(OpCodes.Stloc_1);
            }
            il.MarkLabel(allDone);
            il.BeginCatchBlock(typeof(Exception));
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Call, typeof(DataUtils).GetMethod("ThrowDataException"), null);
            il.EndExceptionBlock();
            il.Emit(OpCodes.Ldloc_1);
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
            il.Emit(OpCodes.Ret);
            return (Func<IDataReader, object>)dm.CreateDelegate(typeof(Func<IDataReader, object>));
        }

        public class FastExpando : System.Dynamic.DynamicObject, IDictionary<string, object>
        {
            IDictionary<string, object> data;
            public IDictionary<string, object> Data
            {
                get { return data; }
                set { data = value; }
            }
            public static FastExpando Attach(IDictionary<string, object> data)
            {
                return new FastExpando { data = data };
            }
            public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
            {
                data[binder.Name] = value;
                return true;
            }
            public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
            {
                return data.TryGetValue(binder.Name, out result);
            }
            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return data.Keys;
            }
            void IDictionary<string, object>.Add(string key, object value)
            {
                throw new NotImplementedException();
            }
            bool IDictionary<string, object>.ContainsKey(string key)
            {
                return data.ContainsKey(key);
            }
            ICollection<string> IDictionary<string, object>.Keys
            {
                get { return data.Keys; }
            }
            bool IDictionary<string, object>.Remove(string key)
            {
                throw new NotImplementedException();
            }
            bool IDictionary<string, object>.TryGetValue(string key, out object value)
            {
                return data.TryGetValue(key, out value);
            }
            ICollection<object> IDictionary<string, object>.Values
            {
                get { return data.Values; }
            }
            object IDictionary<string, object>.this[string key]
            {
                get
                {
                    return data[key];
                }
                set
                {
                    if (!data.ContainsKey(key))
                    {
                        throw new NotImplementedException();
                    }
                    data[key] = value;
                }
            }
            void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }
            void ICollection<KeyValuePair<string, object>>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            {
                return data.Contains(item);
            }

            void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                data.CopyTo(array, arrayIndex);
            }
            int ICollection<KeyValuePair<string, object>>.Count
            {
                get { return data.Count; }
            }
            bool ICollection<KeyValuePair<string, object>>.IsReadOnly
            {
                get { return true; }
            }
            bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }
            IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
            {
                return data.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return data.GetEnumerator();
            }
        }

        class FastExpandoDescriptionProvider : TypeDescriptionProvider
        {
            public FastExpandoDescriptionProvider() : base() { }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return new FastExpandoCustomTypeDescriptor(objectType, instance);
            }
        }

        class FastExpandoCustomTypeDescriptor : CustomTypeDescriptor
        {
            public FastExpandoCustomTypeDescriptor(Type objectType, object instance)
                : base()
            {
                if (instance != null)
                {
                    var tmp = (FastExpando)instance;
                    var names = tmp.GetDynamicMemberNames();
                    foreach (var name in names)
                    {
                        customFields.Add(new DynamicPropertyDescriptor(name, instance));
                    }
                }
            }
            List<PropertyDescriptor> customFields = new List<PropertyDescriptor>();
            public override PropertyDescriptorCollection GetProperties()
            {
                return new PropertyDescriptorCollection(customFields.ToArray());
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return new PropertyDescriptorCollection(customFields.ToArray());
            }
        }
        class DynamicPropertyDescriptor : PropertyDescriptor
        {
            Type propertyType = typeof(object);
            public DynamicPropertyDescriptor(string name, object instance)
                : base(name, null)
            {
                var obj = (IDictionary<string, object>)instance;
                propertyType = obj[name].GetType();
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get
                {
                    return typeof(FastExpando);
                }
            }

            public override object GetValue(object component)
            {
                IDictionary<string, object> obj = (IDictionary<string, object>)component;
                return obj[Name];
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return propertyType;
                }
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                IDictionary<string, object> obj = (IDictionary<string, object>)component;
                obj[Name] = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }

        private static Func<IDataReader, object> GetDynamicDeserializer(IDataRecord reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            var fieldCount = reader.FieldCount;
            if (length == -1)
            {
                length = fieldCount - startBound;
            }

            if (fieldCount <= startBound)
            {
                throw new ArgumentException("fieldCount <= startBound", "splitOn");
            }
            return
                 r =>
                 {
                     IDictionary<string, object> row = new Dictionary<string, object>(length);
                     for (var i = startBound; i < startBound + length; i++)
                     {
                         var tmp = r.GetValue(i);
                         tmp = tmp == DBNull.Value ? null : tmp;
                         row[r.GetName(i)] = tmp;
                         if (returnNullIfFirstMissing && i == startBound && tmp == null)
                         {
                             return null;
                         }
                     }
                     return FastExpando.Attach(row);
                 };
        }
        private static int collect;
        private const int COLLECT_PER_ITEMS = 1000, COLLECT_HIT_COUNT_MIN = 0;
        public class CacheInfo
        {
            public Func<IDataReader, object> Deserializer { get; set; }
            public Func<IDataReader, object>[] OtherDeserializers { get; set; }
            public Action<IDbCommand, object> ParamReader { get; set; }
            private int hitCount;
            public int GetHitCount() { return Interlocked.CompareExchange(ref hitCount, 0, 0); }
            public void RecordHit() { Interlocked.Increment(ref hitCount); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> ReaderToEnumerable<T>(IDataReader reader)
        {
            var info = new CacheInfo
            {
                Deserializer = GetDeserializer(typeof(T), reader, 0, -1, false)
            };
            while (reader.Read())
            {
                dynamic next = info.Deserializer(reader);
                yield return (T)next;
            }
        }
    }
}
