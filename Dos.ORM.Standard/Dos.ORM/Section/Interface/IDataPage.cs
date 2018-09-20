/*
 * Copyright 2010 www.Kernel.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System;
using System.Collections.Generic;

namespace Dos.ORM
{

    public interface IDataPage<T>
    {

        /// <summary>
        /// 当前页的数据列表
        /// </summary>
        List<T> list { get; set; }

        /// <summary>
        /// 所有记录数
        /// </summary>
        int total { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        int pageCurrent { get; set; }

        int pageSize { get; set; }

        int pageCount { get; }

        bool isLastPage { get; }

    }




    /// <summary>
    /// 封装了 ORM 分页查询的结果集
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [Serializable]
    public class DataPage<T> : IDataPage<T>
    {

        /// <summary>
        /// 当前页码
        /// </summary>

        public int pageCurrent { get; set; }


        /// <summary>
        /// 所有记录数
        /// </summary>
        public int total { get; set; }



        public int pageSize { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int pageCount { get { return pageSize == 0 ? 0 : (total + pageSize - 1) / pageSize; } }



        /// <summary>
        /// 查询结果：对象的列表
        /// </summary>
        public List<T> list { get; set; }
        /// <summary>
        /// 返回空的分页结果集
        /// </summary>
        /// <returns></returns>
        public static DataPage<T> GetEmpty()
        {
            DataPage<T> p = new DataPage<T>();
            p.list = new List<T>();
            p.pageCurrent = 1;
            return p;
        }


        public bool isLastPage
        {
            get
            {
                return pageCount == pageCurrent || total == 0;
            }

        }
    }


}
