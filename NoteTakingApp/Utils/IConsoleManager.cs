using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp
{
    public interface IConsoleManager
    {
        void WriteLine(string message);
        ConsoleKey ReadKey(bool boolean);
        string ReadLine();
    }
}
