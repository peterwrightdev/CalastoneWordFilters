using System;
using System.Collections.Generic;
using System.Text;

namespace CalastoneWordFilterer.Consoles
{
    public class StandardConsole : IConsole
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
