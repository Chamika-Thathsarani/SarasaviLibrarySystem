using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SarasaviLibrarySystem
{
    internal class DBConnection
    {
        public static SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection(
                "Data Source=DESKTOP-B1PQJIO\\SQLEXPRESS;Initial Catalog=SarasaviLibrary;Integrated Security=True");

            return con;
        }
    }
}