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

using System.Reflection.Emit;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Data;
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
        internal class Mapper
        {
            private static readonly MethodInfo Object_ToString = typeof(object).GetMethod("ToString");
            private static readonly MethodInfo Reader_Read = typeof(IDataReader).GetMethod("Read");
            private static readonly MethodInfo Reader_GetValues = typeof(IDataRecord).GetMethod("GetValues", new Type[] { typeof(object[]) });
            private static readonly MethodInfo Convert_IsDBNull = typeof(DataUtils.DBConvert).GetMethod("IsDBNull", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToInt16 = typeof(DataUtils.DBConvert).GetMethod("ToInt16", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToInt32 = typeof(DataUtils.DBConvert).GetMethod("ToInt32", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToInt64 = typeof(DataUtils.DBConvert).GetMethod("ToInt64", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToBoolean = typeof(DataUtils.DBConvert).GetMethod("ToBoolean", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToDateTime = typeof(DataUtils.DBConvert).GetMethod("ToDateTime", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToDecimal = typeof(DataUtils.DBConvert).GetMethod("ToDecimal", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToDouble = typeof(DataUtils.DBConvert).GetMethod("ToDouble", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToGuid = typeof(DataUtils.DBConvert).GetMethod("ToGuid", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullInt16 = typeof(DataUtils.DBConvert).GetMethod("ToNInt16", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullInt32 = typeof(DataUtils.DBConvert).GetMethod("ToNInt32", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullInt64 = typeof(DataUtils.DBConvert).GetMethod("ToNInt64", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullBoolean = typeof(DataUtils.DBConvert).GetMethod("ToNBoolean", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullDateTime = typeof(DataUtils.DBConvert).GetMethod("ToNDateTime", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullDecimal = typeof(DataUtils.DBConvert).GetMethod("ToNDecimal", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullDouble = typeof(DataUtils.DBConvert).GetMethod("ToNDouble", new Type[] { typeof(object) });
            private static readonly MethodInfo Convert_ToNullGuid = typeof(DataUtils.DBConvert).GetMethod("ToNGuid", new Type[] { typeof(object) });
            private delegate T ReadEntityInvoker<T>(IDataReader dr);
            private static Dictionary<string, DynamicMethod> m_CatchMethod;
            public static List<T> Map<T>(IDataReader reader)
            {
                if (reader == null || reader.IsClosed)
                {
                    throw new Exception("连接已关闭！");
                }
                #region  Not use the cache for the time being  2015-06-14
                //if (m_CatchMethod == null)
                //{
                //    m_CatchMethod = new Dictionary<string, DynamicMethod>();
                //}
                #endregion
                Type itemType = typeof(T);
                T t = DataUtils.Create<T>();
                var key = itemType.FullName;
                //if (!m_CatchMethod.ContainsKey(key))//Not use the cache for the time being  2015-06-14
                //{
#if WRITE_FILE
				AssemblyName aName = new AssemblyName("DynamicAssembly");
				AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
				ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
				TypeBuilder tb = mb.DefineType("DynamicType", TypeAttributes.Public);
#endif
                Type listType = typeof(List<T>);
                Type objectType = typeof(object);
                Type objArrayType = typeof(object[]);
                Type boolType = typeof(bool);
                Type[] methodArgs = { typeof(IDataReader) };
                MethodInfo LAdd = listType.GetMethod("Add");
                PropertyInfo[] properties = null;
                getMapped(t, itemType, reader, out properties);
#if WRITE_FILE
				MethodBuilder dm = tb.DefineMethod("ReadEntities", MethodAttributes.Public| MethodAttributes.Static, listType, methodArgs);
#else
                DynamicMethod dm = new DynamicMethod("ReadEntities", listType, methodArgs, typeof(Mapper));
#endif
                ILGenerator ilg = dm.GetILGenerator();
                //List<T> list;
                LocalBuilder list = ilg.DeclareLocal(listType);
                //T item;
                LocalBuilder item = ilg.DeclareLocal(itemType);
                //object[] values;
                LocalBuilder values = ilg.DeclareLocal(objArrayType);
                //object objValue;
                LocalBuilder objValue = ilg.DeclareLocal(objectType);
                //type nulls
                LocalBuilder[] typeNulls = null;
                initNulls(properties, ilg, out typeNulls);

                Label exit = ilg.DefineLabel();
                Label loop = ilg.DefineLabel();
                Label[] lblArray = new Label[properties.Length * 2];
                for (int i = 0; i < lblArray.Length; i++)
                {
                    lblArray[i] = ilg.DefineLabel();
                }
                //list = new List<T>();
                ilg.Emit(OpCodes.Newobj, listType.GetConstructor(Type.EmptyTypes));
                ilg.Emit(OpCodes.Stloc_S, list);

                //values=new object[FieldCount];
                ilg.Emit(OpCodes.Ldc_I4, reader.FieldCount);
                ilg.Emit(OpCodes.Newarr, objectType);
                ilg.Emit(OpCodes.Stloc_S, values);

                // while (arg.Read()) {
                ilg.MarkLabel(loop);
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Callvirt, Reader_Read);
                ilg.Emit(OpCodes.Brfalse, exit);

                //reader.GetValues(values);
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldloc_S, values);
                ilg.Emit(OpCodes.Callvirt, Reader_GetValues);
                ilg.Emit(OpCodes.Pop);

                //item=new T(); 
                ilg.Emit(OpCodes.Newobj, itemType.GetConstructor(Type.EmptyTypes));
                ilg.Emit(OpCodes.Stloc_S, item);

                //item.Property=Convert(values[index]);
                for (int index = 0; index < properties.Length; index++)
                {
                    PropertyInfo pi = properties[index];
                    if (pi == null)
                    {
                        continue;
                    }

                    //objValue=value[index];
                    ilg.Emit(OpCodes.Ldloc_S, values);
                    ilg.Emit(OpCodes.Ldc_I4, index);
                    ilg.Emit(OpCodes.Ldelem_Ref);
                    ilg.Emit(OpCodes.Stloc_S, objValue);

                    //tmpBool=Convert.IsDBNull(objValue);
                    ilg.Emit(OpCodes.Ldloc_S, objValue);
                    ilg.Emit(OpCodes.Call, Convert_IsDBNull);

                    //if (!tmpBool){
                    ilg.Emit(OpCodes.Brtrue_S, lblArray[index * 2]);

                    //item.Field=Convert(objValue).ToXXX();
                    ilg.Emit(OpCodes.Ldloc_S, item);
                    ilg.Emit(OpCodes.Ldloc_S, objValue);

                    convertValue(ilg, pi);

                    ilg.Emit(OpCodes.Callvirt, pi.GetSetMethod());
                    //}
                    ilg.Emit(OpCodes.Br_S, lblArray[index * 2 + 1]);
                    //else {
                    ilg.MarkLabel(lblArray[index * 2]);
                    //item.Field=objValue;
                    ilg.Emit(OpCodes.Ldloc_S, item);
                    ilg.Emit(OpCodes.Ldloc_S, typeNulls[index]);
                    ilg.Emit(OpCodes.Callvirt, pi.GetSetMethod());
                    //}
                    ilg.MarkLabel(lblArray[index * 2 + 1]);
                }

                //list.Add(item);
                ilg.Emit(OpCodes.Ldloc_S, list);
                ilg.Emit(OpCodes.Ldloc_S, item);
                ilg.Emit(OpCodes.Callvirt, LAdd);
                //}
                ilg.Emit(OpCodes.Br, loop);
                ilg.MarkLabel(exit);

                // return list;
                ilg.Emit(OpCodes.Ldloc_S, list);
                ilg.Emit(OpCodes.Ret);
#if WRITE_FILE
				Type t = tb.CreateType();
				ab.Save(aName.Name + ".dll");
#else
                //m_CatchMethod.Add(key, dm);//Not use the cache for the time being  2015-06-14
#endif
                //if (m_CatchMethod.Count > 100)Not use the cache for the time being  2015-06-14
                {

                }
                //}

                //if (m_CatchMethod.ContainsKey(key))Not use the cache for the time being  2015-06-14
                //{
                //DynamicMethod dm = m_CatchMethod[key];//Not use the cache for the time being  2015-06-14
                ReadEntityInvoker<List<T>> invoker = dm.CreateDelegate(typeof(ReadEntityInvoker<List<T>>)) as ReadEntityInvoker<List<T>>;
                try// 2015-07-02
                {
                    return invoker.Invoke(reader);
                }
                catch (Exception)
                {
                    reader.Close();
                    throw;
                }
                //}
                throw new Exception("没有找到对应类型的处理方法。");
            }
            private static void initNulls(PropertyInfo[] properties, ILGenerator ilg, out LocalBuilder[] typeNulls)
            {
                typeNulls = new LocalBuilder[properties.Length];
                for (int index = 0; index < properties.Length; index++)
                {
                    PropertyInfo pi = properties[index];
                    if (pi != null)
                    {
                        typeNulls[index] = ilg.DeclareLocal(pi.PropertyType);
                        if (pi.PropertyType.IsValueType)
                        {
                            ilg.Emit(OpCodes.Ldloca_S, typeNulls[index]);
                            ilg.Emit(OpCodes.Initobj, pi.PropertyType);
                        }
                        else
                        {
                            ilg.Emit(OpCodes.Ldnull);
                            ilg.Emit(OpCodes.Stloc_S, typeNulls[index]);
                        }
                    }
                }
            }
            private static void convertValue(ILGenerator ilg, PropertyInfo pi)
            {
                TypeCode code = Type.GetTypeCode(pi.PropertyType);
                switch (code)
                {
                    case TypeCode.Int16:
                        ilg.Emit(OpCodes.Call, Convert_ToInt16);
                        return;
                    case TypeCode.Int32:
                        ilg.Emit(OpCodes.Call, Convert_ToInt32);
                        return;
                    case TypeCode.Int64:
                        ilg.Emit(OpCodes.Call, Convert_ToInt64);
                        return;
                    case TypeCode.Boolean:
                        ilg.Emit(OpCodes.Call, Convert_ToBoolean);
                        return;
                    case TypeCode.String:
                        ilg.Emit(OpCodes.Callvirt, Object_ToString);
                        return;
                    case TypeCode.DateTime:
                        ilg.Emit(OpCodes.Call, Convert_ToDateTime);
                        return;
                    case TypeCode.Decimal:
                        ilg.Emit(OpCodes.Call, Convert_ToDecimal);
                        return;
                    case TypeCode.Double:
                        ilg.Emit(OpCodes.Call, Convert_ToDouble);
                        return;
                }
                Type type = Nullable.GetUnderlyingType(pi.PropertyType);
                if (type != null)
                {
                    code = Type.GetTypeCode(type);
                    switch (code)
                    {
                        case TypeCode.Int16:
                            ilg.Emit(OpCodes.Call, Convert_ToNullInt16);
                            return;
                        case TypeCode.Int32:
                            ilg.Emit(OpCodes.Call, Convert_ToNullInt32);
                            return;
                        case TypeCode.Int64:
                            ilg.Emit(OpCodes.Call, Convert_ToNullInt64);
                            return;
                        case TypeCode.Boolean:
                            ilg.Emit(OpCodes.Call, Convert_ToNullBoolean);
                            return;
                        case TypeCode.DateTime:
                            ilg.Emit(OpCodes.Call, Convert_ToNullDateTime);
                            return;
                        case TypeCode.Decimal:
                            ilg.Emit(OpCodes.Call, Convert_ToNullDecimal);
                            return;
                        case TypeCode.Double:
                            ilg.Emit(OpCodes.Call, Convert_ToNullDouble);
                            return;
                    }
                    if (type.Name == "Guid")
                    {
                        ilg.Emit(OpCodes.Call, Convert_ToNullGuid);
                        return;
                    }
                }
                if (pi.PropertyType.Name == "Guid")
                {
                    ilg.Emit(OpCodes.Call, Convert_ToGuid);
                    return;
                }
                throw new Exception(string.Format("不支持\"{0}\"类型的转换！", pi.PropertyType.Name));
            }
            private static void getMapped<T>(T tt, Type type, IDataReader reader, out PropertyInfo[] mappedProerties)
            {
                mappedProerties = new PropertyInfo[reader.FieldCount];
                string[] fields = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    fields[i] = reader.GetName(i);
                }
                List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var isOk = false;
                    foreach (PropertyInfo pt in properties)
                    {
                        FieldAttribute fa = Attribute.GetCustomAttribute(pt, typeof(FieldAttribute)) as FieldAttribute;
                        if ((fa != null && string.Compare(fa.Field, fields[i], true) == 0) ||
                            string.Compare(pt.Name, fields[i], true) == 0)
                        {
                            properties.Remove(pt);
                            mappedProerties[i] = pt;
                            isOk = true;
                            break;
                        }
                    }
                    #region 2015-09-14新增
                    if (!isOk && tt is Entity)
                    {
                        foreach (PropertyInfo pt in properties)
                        {
                            if (pt.Name == "F" + fields[i])
                            {
                                properties.Remove(pt);
                                mappedProerties[i] = pt;
                            }
                            else if (pt.Name == "_" + fields[i])
                            {
                                properties.Remove(pt);
                                mappedProerties[i] = pt;
                            }
                        }
                        
                    }
                    #endregion
                }
            }
        }
    }
}
