using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AspNetCoreSwaggerDemo.Utils
{
    public class MysqlHelper
    {
       public static MySqlConnection con = new MySqlConnection("server=gz-cdb-pevcn8oq.sql.tencentcdb.com;port=62543;uid=root;pwd=gfz39323537;database=DLCDB;SslMode = none;");
        public static int Insert()
        { 
           
            int count = con.Execute(@"insert into User 
	(
	UserName, 
	Password, 
	Gender, 
	Birthday, 
	CreateUserId, 
	CreateDate, 
	UpdateUserId, 
	UpdateDate, 
	IsDeleted
	)
	values
	( 
	'UserName', 
	'Password', 
	1, 
	sysdate(), 
	1, 
	sysdate(), 
	'1', 
	sysdate(), 
	1
	)");
            return count;
         
            
        }
        public static string Search()
        { 
           return con.QueryFirst<string>("select UserName from User where Id=1");          
        }

        public static List<User> GetJson()        {
           
            return con.Query<User>("select * from User").ToList();
        }

        public static IEnumerable<dynamic> GetJson2()
        {

            return con.Query("select * from User");
        }

    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Gender { get; set; }
        public DateTime Birthday  { get; set; }
        public int CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int IsDeleted { get; set; }
    }

}
