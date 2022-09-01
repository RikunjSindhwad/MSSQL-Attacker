using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;



namespace MSSQLAttackerV2.Modules
{
    public class Helpwrite
    {
        public void doWrite(int state, String content, [Optional, DefaultParameterValue(2000)] int delay)
        {
            switch (state)
            {
                case 0: Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[-] {0}", content); Thread.Sleep(delay); Console.ResetColor(); return;
                case 1: Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("[+] {0}", content); Thread.Sleep(delay); Console.ResetColor(); return;
                case 2: Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("{0}", content); Console.ResetColor(); return;

            }

        }
        public String getFilteredResult(List<string> input)
        {

            if (input.Count > 1)
            {
                var result = string.Join("\r\n", input.ToArray());
                return result;
            }
            if (input.Count == 0) { return ""; }
            return input[0].ToString();
        }
    }
}
