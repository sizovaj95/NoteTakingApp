using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp
{
    public abstract class NoteManagerBase: INoteManager
    {
        public abstract List<NoteInfo> GetNotes();
        public abstract void SaveNotes(List<NoteInfo> notes);
        public abstract void ReadNotes();
    }
}
