using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HJie.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 选中数据库名
        /// </summary>
        public string _datanName { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            RemoteDataSourceStr.Text = "Server=219.135.182.2;Database=Test.HXCDataPermission;user id=sa;password=<Dgjy@123456>;MultipleActiveResultSets=true";
            LocalityDataSourceStr.Text = "Server=localhost;Database=master;Trusted_Connection=True;";

            DataName.SelectionChanged += DataName_SelectionChanged;

            IDbConnection connection = new SqlConnection("Server=219.135.182.2;Database=Test.HXCDataPermission;user id=sa;password=<Dgjy@123456>;MultipleActiveResultSets=true");
            var result = connection.Query<String>("SELECT Name FROM Master..SysDatabases ;");
            DataName.Items.Clear();
            foreach (var item in result.ToList())
            {
                DataName.Items.Add(item);
            }
        }
        /// <summary>
        /// 选择数据库显示所有的表名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _datanName = DataName.SelectedItem.ToString();
            IDbConnection connection = new SqlConnection("Server=219.135.182.2;Database="+DataName.SelectedItem+";user id=sa;password=<Dgjy@123456>;MultipleActiveResultSets=true");

            var tableNames = connection.Query<String>("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name;");
            TableName.Items.Clear();
            foreach (var item in tableNames.ToList())
            {
                TableName.Items.Add(item);
            }
        }
        /// <summary>
        /// 连接远程数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnRemoteData_Click(object sender, RoutedEventArgs e)
        {
            IDbConnection connection = new SqlConnection("Server=219.135.182.2;Database=Test.HXCDataPermission;user id=sa;password=<Dgjy@123456>;MultipleActiveResultSets=true");
            var result = connection.Query<String>("SELECT Name FROM Master..SysDatabases ;");
            DataName.Items.Clear();
            foreach (var item in result.ToList())
            {
                DataName.Items.Add(item);
            }
        }
        /// <summary>
        /// 生成数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateDataName_Click(object sender, RoutedEventArgs e)
        {
            //1、生成本地数据库
            //2、读取远程数据库表的结构
            //3、并接sql语句生成表
            //4、生成本地表

            //FunCreateDataName();

            IDbConnection connection = new SqlConnection("Server=219.135.182.2;Database=" + _datanName + ";user id=sa;password=<Dgjy@123456>;MultipleActiveResultSets=true");

            var tableNames = connection.Query<TableJieGou>(@"select 
                            TableName=c.Name, 
                            TableDes=isnull(f.[value],''), 
                            SerialNumber=a.Column_id, 
                            ColumnName=a.Name, 
                            ColumnDes=isnull(e.[value],''), 
                            TypeDatabase=b.Name, 
                            ColumnType= case when b.Name = 'image' then 'byte[]'
                            when b.Name in('image','uniqueidentifier','ntext','varchar','ntext','nchar','nvarchar','text','char') then 'string'
                            when b.Name in('tinyint','smallint','int','bigint') then 'int'
                            when b.Name in('datetime','smalldatetime') then 'DateTime'
                            when b.Name in('float','decimal','numeric','money','real','smallmoney') then 'decimal'
                            when b.Name ='bit' then 'bool' else b.name end ,
                            [标识]= case when is_identity=1 then '是' else '' end, 
                            TableKey= case when exists(select 1 from sys.objects x join sys.indexes y on x.Type=N'PK' and x.Name=y.Name 
                            join sysindexkeys z on z.ID=a.Object_id and z.indid=y.index_id and z.Colid=a.Column_id) 
                            then '是' else '' end, 
                            NumberBytes=case when a.[max_length]=-1 and b.Name!='xml' then 'max/2G' 
                            when b.Name='xml' then '2^31-1字节/2G' 
                            else rtrim(a.[max_length]) end, 
                            Length=case when ColumnProperty(a.object_id,a.Name,'Precision')=-1 then '2^31-1' 
                            else rtrim(ColumnProperty(a.object_id,a.Name,'Precision')) end, 
                            DecimalPlace=isnull(ColumnProperty(a.object_id,a.Name,'Scale'),0), 
                            IsEmpty=case when a.is_nullable=1 then '是' else '' end, 
                            [默认值]=isnull(d.text,'')

                            from sys.columns a 
                            left join sys.types b on a.user_type_id=b.user_type_id 
                            inner join sys.objects c on a.object_id=c.object_id and c.Type='U' 
                            left join syscomments d on a.default_object_id=d.ID 
                            left join sys.extended_properties e on e.major_id=c.object_id and e.minor_id=a.Column_id and e.class=1 
                            left join sys.extended_properties f on f.major_id=c.object_id and f.minor_id=0 and f.class=1

                            ORDER BY c.Name,a.Column_id");

            Dictionary<string, List<TableJieGou>> dic = new Dictionary<string, List<TableJieGou>>();
            foreach (var item in tableNames.ToList())
            {
                if(item.TableName== "__EFMigrationsHistory")
                {
                    continue;
                }
                if (dic.ContainsKey(item.TableName))
                {
                    List<TableJieGou> list = dic[item.TableName];
                    list.Add(item);
                    dic[item.TableName] = list;
                }
                else
                {
                    List<TableJieGou> tableJieGous = new List<TableJieGou>();
                    tableJieGous.Add(item);
                    dic[item.TableName] = tableJieGous;
                }
            }
            //
            IDbConnection lconnection = new SqlConnection("Server=localhost;Database=DGCN_HXCDataPermission;Trusted_Connection=True;");
           
            foreach (var keyValue in dic)
            {
                string sql = "CREATE TABLE "+keyValue.Key+" (";
                foreach (var item in keyValue.Value)
                {
                    if (item.TableKey == "是")
                    {
                        var isnull = item.IsEmpty == "是" ? "  null" : "  NOT NULL";
                        sql += item.ColumnName + " " + item.TypeDatabase + " IDENTITY(1,1) " + isnull + ",";
                    }
                    else
                    {
                        //[Body] [nvarchar](max) NULL,
                        if (item.TypeDatabase == "uniqueidentifier")
                        {
                            var isnull = item.IsEmpty == "是" ? "  null" : "  NOT NULL";
                            sql += item.ColumnName + " " + item.TypeDatabase + " " +isnull+",";
                        }
                        else if(item.TypeDatabase== "bigint")
                        {
                            var isnull = item.IsEmpty == "是" ? "  null" : "  NOT NULL";
                            string max = item.NumberBytes.Contains("max") == true ? "max" : item.Length;
                            sql += item.ColumnName + " " + item.TypeDatabase + isnull+",";

                        }
                        else if (item.TypeDatabase == "datetime2")
                        {
                            var isnull = item.IsEmpty == "是" ? "  null" : "  NOT NULL";
                            string max = item.NumberBytes.Contains("max") == true ? "max" : item.DecimalPlace;
                            sql += item.ColumnName + " " + item.TypeDatabase + " (" + max + ")" + isnull + ",";

                        }
                        else if (item.TypeDatabase == "int")
                        {
                            var isnull = item.IsEmpty == "是" ? "  null" : "  NOT NULL";
                            string max = item.NumberBytes.Contains("max") == true ? "max" : item.Length;
                            sql += item.ColumnName + " " + item.TypeDatabase + isnull + ",";

                        }
                        else 
                        {
                            var isnull = item.IsEmpty == "是" ? "  null" : "  NOT NULL";
                            string max = item.NumberBytes.Contains("max") == true ? "max" : item.Length;
                            sql += item.ColumnName + " " + item.TypeDatabase + " (" + max + ")" + isnull + ",";

                        }
                    }
                }
                sql=sql.Substring(0, sql.Length - 1);
                //替换调最后一个，
                sql += "); ";
                var result = lconnection.Execute(sql);

            }



            //var tableNames = connection.Query<String>("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name;");

        }

        private void ConnLocalityData_Click(object sender, RoutedEventArgs e)
        {
            IDbConnection connection = new SqlConnection("Server=localhost;Database=master;Trusted_Connection=True;");
            if(connection.State== ConnectionState.Closed)
            {
                MessageBox.Show("连接成功");
            }
        }
        public string _loDataName { get; set; }
        public void FunCreateDataName()
        {
            if (_datanName == null)
            {
                return;
            }
            _loDataName = _datanName.Replace('.', '_');
            IDbConnection connection = new SqlConnection("Server=localhost;Database=master;Trusted_Connection=True;");
            var result = connection.Execute("create database " + _loDataName + ";");
        }


    }
}
