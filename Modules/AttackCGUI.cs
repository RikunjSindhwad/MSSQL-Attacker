using System;
using MSSQLAttackerV2.Modules;
using MSSQLAttackerV2.Banner;
using System.Data.SqlClient;


namespace MSSQLAttackerV2.Modules
{
    internal class AttackCGUI
    {
        public void Menu(bool gui)
        {
            if (!gui)
            {

            }
            else
            {
                Console.Clear();
                Console.Title = "MSSQL Attacker";
                Console.WriteLine("\t\t\t\t[*] MSSQL Attacker V2 by Rikunj Sindhwad [*]\n");
                Console.WriteLine("[1] Get Information\t\t\t [2] UNC PATH Injection\t\t [3] Impersonation Check\n[4] ImpersonateSA\t\t\t [5] Impersonate DBO\t\t [6] Enable xp_cmdshell\n[7] Shell_Access\t\t\t [8] Check LinkedServers\t [9] Enumerate LinkedServer Version");
                Console.WriteLine("[10] EnableLinkedServer_xp_cmdshell\t [11] LinkedServer xp_cmdshell\t[12] Custom SQL Query");
                Console.WriteLine("[0] Exit Program\n");
                Console.Write("[INPUT] Enter Value: ");
            }
        }

        public void attack(string[] args)
        {
            var impersonate = new Impersonate();
            var LinkedServer = new LinkedServer();
            var CommanAttacks = new CommonAttacks();
            var runQuery = new RunQuery();
            var Help = new Help();
            var helpWrite = new Helpwrite();
            var Connection = new Connection();
            var getinfo = new GetInfo();
            bool exit = false;
            SqlConnection con;
            switch (args.Length)
            {
                case 1:
                    Help.printGUIHelp();
                    return;
                case 3:
                    con = Connection.connectDb(args[1], args[2]);
                    break;
                case 5:
                    con = Connection.connectDb(args[1], args[2], args[3], args[4]);
                    break;
                default:
                    con = Connection.connectDb(args[1]);
                    break;
            }
            if (con == null) { return; }
            while (!exit)
            {

                Menu(true);
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
                    case 1: getinfo.loginInfo(con); break;
                    case 2: CommanAttacks.uncPathInjection(con); break;
                    case 3: impersonate.checkImpersonation(con); break;
                    case 4: impersonate.abuseImpersonation(con); break;
                    case 5: impersonate.abuseImpersonationDBO(con); break;
                    case 6: CommanAttacks.enableXpCmdShell(con); break;
                    case 7: CommanAttacks.execCMD(con); break;
                    case 8: LinkedServer.enumLinkedServer(con); break;
                    case 9: LinkedServer.enumLinkedServerVersion(con); break;
                    case 10: LinkedServer.enableLinkedCmdShell(con); break;
                    case 11: LinkedServer.linkedServerCmdExec(con); break;
                    case 12: runQuery.customQuery(con); break;

                    default:
                        helpWrite.doWrite(0, "Invelid Key: Exiting..");
                        exit = true; break;

                }
            }

        }
    }
}
