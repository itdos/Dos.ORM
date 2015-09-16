#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
* 电子邮箱：admin@itdos.com
* 创建日期：2015/09/10 14:08:52
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
using System.Text;
using System.Data;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Common
{
    public class NpoiHelper
    {
        public static void ExportToFile(DataSet dataSet, string fileFullPath)
        {
            var dts = new List<DataTable>();
            foreach (DataTable dt in dataSet.Tables) dts.Add(dt);
            ExportToFile(dts, fileFullPath);
        }
        public static void ExportToFile(DataTable dataTable, string fileFullPath)
        {
            var dts = new List<DataTable>();
            dts.Add(dataTable);
            ExportToFile(dts, fileFullPath);
        }
        public static void ExportToFile(IEnumerable<DataTable> dataTables, string fileFullPath)
        {
            var workbook = new XSSFWorkbook();
            var i = 0;
            foreach (DataTable dt in dataTables)
            {
                var sheetName = string.IsNullOrEmpty(dt.TableName)
                    ? "Sheet " + (++i).ToString()
                    : dt.TableName;
                var sheet = workbook.CreateSheet(sheetName);

                var headerRow = sheet.CreateRow(0);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var columnName = string.IsNullOrEmpty(dt.Columns[j].ColumnName)
                        ? "Column " + j.ToString()
                        : dt.Columns[j].ColumnName;
                    headerRow.CreateCell(j).SetCellValue(columnName);
                }

                for (var a = 0; a < dt.Rows.Count; a++)
                {
                    var dr = dt.Rows[a];
                    var row = sheet.CreateRow(a + 1);
                    for (var b = 0; b < dt.Columns.Count; b++)
                    {
                        row.CreateCell(b).SetCellValue(dr[b] != DBNull.Value ? dr[b].ToString() : string.Empty);
                    }
                }
            }

            using (var fs = File.Create(fileFullPath))
            {
                workbook.Write(fs);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xlsxFile"></param>
        /// <returns></returns>
        public static List<DataTable> GetDataTablesFrom(string xlsxFile)
        {
            if (!File.Exists(xlsxFile))
                throw new FileNotFoundException("文件不存在");
            var result = new List<DataTable>();
            Stream stream = new MemoryStream(File.ReadAllBytes(xlsxFile));
            return GetDataTablesFrom(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static List<DataTable> GetDataTablesFrom(Stream stream)
        {
            var result = new List<DataTable>();
            var workbook = new XSSFWorkbook(stream);
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var dt = new DataTable();
                var sheet = workbook.GetSheetAt(i);
                var headerRow = sheet.GetRow(0);

                int cellCount = headerRow.LastCellNum;
                for (int j = headerRow.FirstCellNum; j < cellCount; j++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(j).StringCellValue);
                    dt.Columns.Add(column);
                }

                var rowCount = sheet.LastRowNum;
                for (var a = (sheet.FirstRowNum + 1); a < rowCount; a++)
                {
                    var row = sheet.GetRow(a);
                    if (row == null) continue;

                    var dr = dt.NewRow();
                    for (int b = row.FirstCellNum; b < cellCount; b++)
                    {
                        if (row.GetCell(b) == null) continue;
                        dr[b] = row.GetCell(b).ToString();
                    }

                    dt.Rows.Add(dr);
                }
                result.Add(dt);
            }
            stream.Close();
            return result;
        }
    }
}