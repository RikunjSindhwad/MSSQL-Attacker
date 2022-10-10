using MSSQLAttackerV2.Banner;
using System;


namespace MSSQLAttackerV2.Modules
{
    internal class ArgumentChecker
    {
        public bool checkArgs(string[] args, String val)
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
        public bool checkSubargs(string[] args, String arg)
        {
            var stdout = new Helpwrite();
            var help = new Help();
            string[] attacks = { "checkimpersonate", "checklinkedservers", "checklinkedserverVersion", "uncpathinject", "getinfo", "togglecmdshell", "togglelinkedcmdshell", "execlinkedcmd", "execcmd", "runCustomQuery" };
            if (!Array.Exists(attacks, element => element == arg)) { help.printAttacks(attacks); return false; }
            if (!checkArgs(args, "-t")) { stdout.doWrite(0, "Missing -t [TARGETSERVER]", 0); return false; }
            if (arg.Contains("linked") && !arg.Contains("checklinkedservers"))
            {
                if (!checkArgs(args, "-ls")) { stdout.doWrite(0, "Missing -ls [LinkedServer]", 0); return false; }

            }
            if (arg.Contains("cmd") && !arg.Contains("toggle"))
            {
                if (!checkArgs(args, "-c")) { stdout.doWrite(0, "Missing -c [Command to execute]", 0); return false; }

            }
            if (arg.Contains("Custom"))
            {
                if (!checkArgs(args, "-query")) { stdout.doWrite(0, "Missing -query \"Custom SQL Query\"", 0); return false; }

            }
            if (arg.Contains("uncpathinject"))
            {
                if (!checkArgs(args, "-l")) { stdout.doWrite(0, "Missing -l [ LHOST || Attacker IP ]", 0); return false; }
                arg = args[Array.IndexOf(args, "-l") + 1];
            }
            return true;
        }
    }
}
