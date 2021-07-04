using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;

namespace Colder.Common
{
    /// <summary>
    /// NPOI帮助类
    /// </summary>
    public static class NPOIHelper
    {
        /// <summary>
        /// 将excel导入到datatable
        /// 注：行号从0开始
        /// </summary>
        /// <param name="columnNameIndex">列名行号</param>
        /// <param name="dataIndex">数据开始行号</param>
        /// <param name="fileBytes">excel文件字节</param>
        /// <param name="xlsx">是否为xlsx格式（即2007+版本）</param>
        /// <returns>返回datatable</returns>
        public static DataTable ReadExcel(byte[] fileBytes, int dataIndex = 1, int columnNameIndex = 0, bool xlsx = true)
        {
            using var fs = new MemoryStream(fileBytes);

            IWorkbook workbook = xlsx ? (IWorkbook)new XSSFWorkbook(fs) : new HSSFWorkbook(fs);

            if (workbook == null)
            {
                throw new Exception("读取失败");
            }

            var sheet = workbook.GetSheetAt(0);
            var dataTable = new DataTable();

            if (sheet == null)
            {
                throw new Exception("读取失败");
            }

            int rowCount = sheet.LastRowNum;//总行数

            if (rowCount == 0)
            {
                throw new Exception("无数据");
            }

            IRow columnNameRow = sheet.GetRow(columnNameIndex);//列名行
            int cellCount = columnNameRow.LastCellNum;//列数

            ICell cell;
            DataColumn column;
            //构建datatable的列
            if (columnNameIndex >= 0)
            {
                for (int i = columnNameRow.FirstCellNum; i < cellCount; ++i)
                {
                    cell = columnNameRow.GetCell(i);
                    if (cell != null)
                    {
                        if (cell.StringCellValue != null)
                        {
                            column = new DataColumn(cell.StringCellValue);
                            dataTable.Columns.Add(column);
                        }
                    }
                }
            }
            else
            {
                for (int i = columnNameRow.FirstCellNum; i < cellCount; ++i)
                {
                    column = new DataColumn("column" + (i + 1));
                    dataTable.Columns.Add(column);
                }
            }

            //填充行
            for (int i = dataIndex; i <= rowCount; ++i)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }

                var dataRow = dataTable.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; ++j)
                {
                    cell = row.GetCell(j);
                    if (cell == null)
                    {
                        dataRow[j] = "";
                    }
                    else
                    {
                        //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                        switch (cell.CellType)
                        {
                            case CellType.Blank:
                                dataRow[j] = "";
                                break;
                            case CellType.Numeric:
                                short format = cell.CellStyle.DataFormat;
                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                {
                                    dataRow[j] = cell.DateCellValue;
                                }
                                else
                                {
                                    dataRow[j] = cell.NumericCellValue;
                                }

                                break;
                            case CellType.String:
                                dataRow[j] = cell.StringCellValue;
                                break;
                        }
                    }
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// 写入excel
        /// </summary>
        /// <param name="dt">数据</param>
        /// <param name="xlsx">是否为xlsx格式（即2007+版本）</param>
        /// <returns>excel字节</returns>
        public static byte[] WriteExcel(DataTable dt, bool xlsx = true)
        {
            using MemoryStream stream = new MemoryStream();

            if (dt != null && dt.Rows.Count > 0)
            {
                IWorkbook workbook = xlsx ? (IWorkbook)new XSSFWorkbook() : new HSSFWorkbook();
                var sheet = workbook.CreateSheet("Sheet0");
                int rowCount = dt.Rows.Count;//行数
                int columnCount = dt.Columns.Count;//列数

                //设置列头
                var row = sheet.CreateRow(0);
                ICell cell;
                for (int c = 0; c < columnCount; c++)
                {
                    cell = row.CreateCell(c);
                    cell.SetCellValue(dt.Columns[c].ColumnName);
                }

                //设置每行每列的单元格,
                for (int i = 0; i < rowCount; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < columnCount; j++)
                    {
                        cell = row.CreateCell(j);//excel第二行开始写入数据
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }

                workbook.Write(stream);
            }

            return stream.ToArray();
        }

        /// <summary>
        /// 通过模板写入excel
        /// </summary>
        /// <param name="dt">数据</param>
        /// <param name="xlsx">是否为xlsx格式（即2007+版本）</param>
        /// <returns>excel字节</returns>
        public static byte[] WriteExcelByTemplate(DataTable dt, bool xlsx = true)
        {
            //TODO 待实现，实现方式可参考Aspose.cells

            using MemoryStream stream = new MemoryStream();

            return stream.ToArray();
        }
    }
}
