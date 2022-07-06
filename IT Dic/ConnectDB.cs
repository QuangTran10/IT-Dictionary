using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Dic
{
    public class ConnectDB
    {
        public static MySqlConnection Connect()
        {
            MySqlConnection conn = null;
            string ip = "", user = "", pass = "", database = "";
            int port = 3306;
            string[] db = File.ReadAllLines("dic.txt");
            ip = db[0];
            port = Convert.ToInt32(db[1]);
            user = db[2];
            pass = db[3];
            database = db[4];

            return conn = Connection.getDBConnection(ip, port, database, user, pass);
        }
    }
}
