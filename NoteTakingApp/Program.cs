
// See https://aka.ms/new-console-template for more information


namespace NoteTakingApp
{

    public class Program
    {
        static void Main()
        {
            NoteManager noteManager = new NoteManager();
            List<NoteInfo> Notes = noteManager.GetNotes();
            noteManager.SaveNotes(Notes);
            noteManager.ReadNotes();
        }      
    }
}


// tests?