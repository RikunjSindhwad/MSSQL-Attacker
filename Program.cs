using System;
using MSSQLAttackerV2.Banner;
using MSSQLAttackerV2.Modules;

namespace MSSQLAttackerV2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length == 0)
            {
                var Help = new Help();
                Help.printCliHelp();
                Help.printGUIHelp();
                return;
            }
            
            try
            {

                if (args[0].ToLower() == "gui") { var AttackCGUI = new AttackCGUI(); AttackCGUI.attack(args); }
                if (args[0].ToLower() == "cli") { var AttackCLI = new AttackCLI(); AttackCLI.attack(args); }

            }
            catch (Exception)
            {
                var helpWrite = new Helpwrite();
                helpWrite.doWrite(0, "Something Wrong!", 0);
                throw;

            }
        }
    }
}
