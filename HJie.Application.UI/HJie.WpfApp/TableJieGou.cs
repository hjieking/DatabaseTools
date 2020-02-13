using System;
using System.Collections.Generic;
using System.Text;

namespace HJie.WpfApp
{
    public class TableJieGou
    {
        public string TableName { get; set; }
        public string TableDes { get; set; }
        public string SerialNumber { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDes { get; set; }
        public string ColumnType { get; set; }

        public string TableKey { get; set; }
        public string Length { get; set; }

        public string IsEmpty { get; set; }

        public string TypeDatabase { get; set; }

        public string NumberBytes { get; set; }
        /// <summary>
        /// 小数位
        /// </summary>
        public string DecimalPlace { get; set; }

    }
}
