using Ninject.Modules;

namespace NoteTakingApp
{
    public class NinjectDependencyResolver: NinjectModule
    {
        public override void Load()
        {
            Bind<IConsoleManager>().To<ConsoleManager>();
            Bind<ITimeManager>().To<TimeManager>();
            Bind<INoteManager>().To<NoteManagerConsole>();            
        }
    }
}
