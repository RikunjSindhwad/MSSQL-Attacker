using System;
using System.Data.SqlClient;

namespace MSSQLAttackerV2.Modules
{
    internal class GetInfo
    {
        public void loginInfo(SqlConnection con)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();
            String queryUser = "SELECT SYSTEM_USER;";
            String user = helpWrite.getFilteredResult(runQuery.getQueryResult(con, queryUser));
            helpWrite.doWrite(1, "User: " + user);
            String querySysadmin = "SELECT IS_SRVROLEMEMBER('sysadmin');";
            String role = helpWrite.getFilteredResult(runQuery.getQueryResult(con, querySysadmin));
            if (role == "1")
            {
                helpWrite.doWrite(1, "User is a member of sysadmin role");
            }
            else
            {
                helpWrite.doWrite(0, "User is NOT a member of sysadmin role");
            }
        }
    }
}
