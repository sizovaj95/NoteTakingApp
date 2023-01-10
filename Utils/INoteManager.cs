
namespace NoteTakingApp
{
    public interface INoteManager
    {
        List<NoteInfo> GetNotes();
        void SaveNotes(List<NoteInfo> notes);
        void ReadNotes();
    }
}
