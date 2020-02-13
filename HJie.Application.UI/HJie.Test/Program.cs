using Dapper;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;

namespace HJie.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IDbConnection connection = new SqlConnection("Server=219.135.182.2;Database=Test.HXCDataPermission;user id=sa;password=<Dgjy@123456>;MultipleActiveResultSets=true");

            //var result = connection.Execute<object>("SELECT Name FROM Master..SysDatabases  WHERE Name='Test.HXCDataPermission' ORDER BY Name;");
            var dataname= connection.Query<String>("SELECT Name FROM Master..SysDatabases  WHERE Name='Test.HXCDataPermission' ORDER BY Name;");

            var tablenames = connection.Query<String>("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name;");

            
            var tableJieGuos= connection.Query<object>(@"SELECT  c.TABLE_SCHEMA ,
                                                        c.TABLE_NAME,
                                                        c.COLUMN_NAME,
                                                        c.DATA_TYPE,
                                                        c.CHARACTER_MAXIMUM_LENGTH,
                                                        c.COLUMN_DEFAULT,
                                                        c.IS_NULLABLE,
                                                        c.NUMERIC_PRECISION,
                                                        c.NUMERIC_SCALE
                                                FROM[INFORMATION_SCHEMA].[COLUMNS] c
                                                WHERE   TABLE_NAME = 'ApiLog'; ");
            CreateDb("HXCDataPermission") ;

            Console.Read();
        }
        static void CreateDb(string dataBaseName)
        {
            IDbConnection connection = new SqlConnection("Server=localhost;Database=master;Trusted_Connection=True;");

            var result = connection.Execute("create database "+ dataBaseName + ";");
            var a = connection.Execute("EXEC sp_renamedb 'HXCDataPermission', 'Test.HXCDataPermission' ; ");
            //var dataname = connection.Query<String>("SELECT Name FROM Master..SysDatabases  WHERE Name='Test.HXCDataPermission' ORDER BY Name;");

            var tablenames = connection.Query<String>("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name;");
            CreateTable();
            Console.Read();
        }
        static void CreateTable()
        {
            IDbConnection connection = new SqlConnection("Server=localhost;Database=Test.HXCDataPermission;Trusted_Connection=True;");

            var result = connection.Execute(@"CREATE TABLE ApiLog (
	                            [ALgID] [int] IDENTITY(1,1) NOT NULL,
	                            [ClientIP] [nvarchar](max) NULL,
	                            [ResponseTime] [bigint] NOT NULL,
	                            [AccessToken] [nvarchar](max) NULL,
	                            [AccessTime] [datetime2](7) NOT NULL,
	                            [AccessApiUrl] [nvarchar](max) NULL,
	                            [AccessAction] [nvarchar](max) NULL,
	                            [QueryString] [nvarchar](max) NULL,
	                            [Body] [nvarchar](max) NULL,
	                            [HttpStatus] [int] NOT NULL
                            ); ");


            var tablenames = connection.Query<String>("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name;");

            Console.Read();
        }
    }
}
