using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;

namespace AgapeApp
{
    public class Connection
    {
        MySqlConnection conn =
            new MySqlConnection("server=localhost;user id=Chrys;pwd=chris414;database=discipledata;allowuservariables=True;persistsecurityinfo=True");
        public MySqlConnection ActiveCon()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }
    }
}
