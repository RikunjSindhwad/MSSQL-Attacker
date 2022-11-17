using MSSQLAttackerV2.Modules;
using System;

namespace MSSQLAttackerV2.Banner
{
    public class Help
    {

        public void printCliHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "MSSQL Attacker";
            Console.WriteLine("[*] MSSQL Attacker - V2 by Rikunj Sindhwad [CLI MODE] [*]");
            Console.ResetColor();
            var Helpwrite = new Helpwrite();

            Helpwrite.doWrite(2, "\t\tHELP MENU", 0);
            Helpwrite.doWrite(2, " USAGE: binary.exe cli -a AttackName -t DatabaseServer -d DatabaseName\n", 0);
            Helpwrite.doWrite(2, " -a\t\t\tAttack Mode [Add -a to get list of attacks]", 0);
            Helpwrite.doWrite(2, " -t\t\t\tTarget Server", 0);
            Helpwrite.doWrite(2, " -d\t\t\tTarget DatabaseName [Optional]", 0);
            Helpwrite.doWrite(2, " -u\t\t\tTarget Username [Optional]", 0);
            Helpwrite.doWrite(2, " -p\t\t\tTarget Password [Optional]", 0);
            Helpwrite.doWrite(2, " -dbo\t\t\tDatabaseNmae for DBO impersonation [Optional]", 0);
            Helpwrite.doWrite(2, " -ls\t\t\tLinked MSSQLServer Name", 0);
            Helpwrite.doWrite(2, " -l\t\t\tAttacker IP for UNC Path Injection", 0);
            Helpwrite.doWrite(2, " -query\t\t\tCustom SQL Query", 0);
            Helpwrite.doWrite(2, " -iuser\t\t\tUserName to impersonate", 0);
            Helpwrite.doWrite(2, " -impersonate\t\tImpersonateSA before execution of any attack", 0);
            Helpwrite.doWrite(2, " -impersonateSA\t\tImpersonateSA before execution of any attack", 0);
            Helpwrite.doWrite(2, " -impersonateDBO\tImpersonateDBO before execution of any attack", 0);
        }
        public void printGUIHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "MSSQL Attacker";
            Console.WriteLine("[*] MSSQL Attacker - V2 by Rikunj Sindhwad [GUI MODE] [*]");
            Console.ResetColor();
            var Helpwrite = new Helpwrite();
            Helpwrite.doWrite(2, "\t\tHELP MENU", 0);
            Helpwrite.doWrite(2, " USAGE: binary.exe GUI DatabaseServer [Optional] DatabaseName [Optional] Username [Optional] Password", 0);
            Helpwrite.doWrite(2, " USAGE: binary.exe GUI dc01.corp1.com masters SA SecretPassword", 0);

        }
        public void printAttacks(string[] attacks)
        {
            var stdout = new Helpwrite();
            stdout.doWrite(0, "Invelid AttackName", 0);
            stdout.doWrite(1, "Available Attacks", 0);
            foreach (string attack in attacks) { stdout.doWrite(2, "\t" + attack); }
        }

    }
}
