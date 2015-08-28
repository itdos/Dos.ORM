#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：Sys_BaseDataLogic
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
* 电子邮箱：admin@itdos.com
* 创建日期：2014/10/1 11:00:49
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using Dos.ORM;

namespace DataAccess
{
    public interface IRepository<T> where T : Entity
    {
        List<T> GetAll();
        /// <summary>
        /// 通用查询
        /// </summary>
        List<T> Query(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy = null, EnumService.OrderBy ascDesc = EnumService.OrderBy.Asc, int? top = null, int? pageSize = null, int? pageIndex = null);
        /// <summary>
        /// 通用查询
        /// </summary>
        List<T> Query(Where<T> where, Expression<Func<T, object>> orderBy = null, EnumService.OrderBy ascDesc = EnumService.OrderBy.Asc, int? top = null, int? pageSize = null, int? pageIndex = null);
        /// <summary>
        /// 增加单个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert(T entity);
        void Insert(DbTrans context, T entity);
        /// <summary>
        /// 增加多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        int Insert(IEnumerable<T> entities);
        void Insert(DbTrans context, IEnumerable<T> entities);
        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(T entity);
        void Update(DbTrans context, T entity);
        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        int Update(IEnumerable<T> entities);
        void Update(DbTrans context, IEnumerable<T> entities);
        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(Guid? id);
        //void DeleteById(DbContext context, object id);
        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        int Delete(IEnumerable<T> entities);
        //void Delete(DbContext context, IEnumerable<T> entities);
        ///// <summary>
        ///// 根据id获取实体
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        ////T GetById(object key);
    }
}
