
// See https://aka.ms/new-console-template for more information


using Ninject;
using System.Reflection;

namespace NoteTakingApp
{

    public class Program
    {
        static void Main()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var programManager = kernel.Get<INoteManager>();

            List<NoteInfo> Notes = programManager.GetNotes();
            programManager.SaveNotes(Notes);
            programManager.ReadNotes();
        }      
    }
}


// tests?

// remove notes