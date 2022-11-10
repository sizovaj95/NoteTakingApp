using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ninject.Modules;

namespace NoteTakingApp
{
    public class NinjectDependencyResolver: NinjectModule
    {
        public override void Load()
        {
            Bind<IConsoleManager>().To<ConsoleManager>();
            Bind<ITimeManager>().To<TimeManager>();
            Bind<INoteManager>().To<NoteManager>();            
        }
    }
}
