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
        //ConsoleKeyInfo ReadKey();

        ConsoleKey ReadKey(bool boolean);
        string ReadLine();
    }
}
