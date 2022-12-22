
namespace NoteTakingApp
{
    public interface INoteManager
    {
        List<NoteInfo> GetNotes();
        void SaveNotes(List<NoteInfo> notes);
        void ReadNotes();
        void ReadNotes(string startDate, string endDate);
    }
}
