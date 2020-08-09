using System;
using System.Collections.Generic;
using System.Text;

namespace CalastoneWordFilterer.Consoles
{
    public interface IConsole
    {
        string ReadLine();

        void WriteLine(string line);
    }
}
