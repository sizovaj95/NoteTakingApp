using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp
{
    public interface INoteManager
    {
        List<NoteInfo> GetNotes();
        void SaveNotes(List<NoteInfo> notes);
        void ReadNotes();
    }
}
