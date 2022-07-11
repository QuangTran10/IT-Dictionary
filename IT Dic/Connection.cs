using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Dic
{
    public class Connection
    {
        public static MySqlConnection getDBConnection(string host, int port, string database, string username, string password)
        {
            String connString = "Server=" + host + ";Database=" + database
                + ";Port=" + port + ";User Id=" + username + ";Password=" + password + ";charset=utf8;SSL Mode=None";
            MySqlConnection conn = new MySqlConnection(connString);
            return conn;
        }
    }
}
