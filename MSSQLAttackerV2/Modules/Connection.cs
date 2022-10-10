using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;


namespace MSSQLAttackerV2.Modules
{
    internal class Connection
    {
        public SqlConnection connectDb(string server, [Optional, DefaultParameterValue("master")] string database, [Optional, DefaultParameterValue(null)] String username, [Optional, DefaultParameterValue(null)] String password)
        {
            String conString;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            { conString = "Server = " + server + "; Database = " + database + "; Integrated Security = True;"; }
            else
            { conString = "Server = " + server + "; Database = " + database + "; User=" + username + ";password=" + password; }
            SqlConnection con = new SqlConnection(conString);
            var stdout = new Helpwrite();
            try
            {
                con.Open();
                stdout.doWrite(1, "Auth success!", 0);
            }
            catch
            {
                stdout.doWrite(0, "Auth failed");
                return null;
            }
            return con;

        }
    }
}
