using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp
{
    public abstract class ConsoleManagerBase : IConsoleManager
    {
        public abstract void WriteLine(string message);
        //public abstract ConsoleKeyInfo ReadKey();
        public abstract ConsoleKey ReadKey(bool boolean);
        public abstract string ReadLine();

    }
}
