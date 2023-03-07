using NoteTakingDbEF.Models;
using System.Globalization;

namespace NoteTakingApp
{
    public class NoteManagerDbEf
    {
        private readonly IConsoleManager consoleManager;
        private readonly ITimeManager timeManager;
        private readonly NoteAppDbContext noteAppDbContext;

        public NoteManagerDbEf(IConsoleManager consoleManager, ITimeManager timeManager, NoteAppDbContext noteAppDbContext)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
            this.noteAppDbContext = noteAppDbContext;

        }
        public List<Note> GetNotes()
        {
            List<Note> Notes = new List<Note>();
            ConsoleKey cki;
            do
            {
                consoleManager.WriteLine("Type your note: ");
                string note = consoleManager.ReadLine();
                if (!String.IsNullOrEmpty(note))
                {
                    DateTime dtNow = timeManager.DateTimeNow();
                    Note noteInfo = new Note { Note1 = note, Date = dtNow, Time = dtNow.TimeOfDay }; 
                    Notes.Add(noteInfo);
                }

                consoleManager.WriteLine("Press any key to add another note or Esc to exit.");
                cki = consoleManager.ReadKey(true);

            } while (cki != ConsoleKey.Escape);
            return Notes;
        }

        public void SaveNotes(List<Note> notes)
        {
            noteAppDbContext.Notes.AddRange(notes);
            noteAppDbContext.SaveChanges();
        }

        public void ReadNotes(string startDate = "", string endDate = "")
        {
            if (string.IsNullOrEmpty(startDate) & string.IsNullOrEmpty(endDate))
            {
                var notes = noteAppDbContext.Notes.ToList();
                PrintNotes(notes);
            }

            else if (!string.IsNullOrEmpty(startDate) & !string.IsNullOrEmpty(endDate))
            {
                DateTime start;
                DateTime end;
                if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out start))
                {
                    consoleManager.WriteLine("Incorrect start date or date format!");
                    return;
                }
                if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out end))
                {
                    consoleManager.WriteLine("Incorrect end date or date format!");
                    return;
                }

                if (!(end >= start))
                {
                    consoleManager.WriteLine("Start Date is after end date!");
                    return;
                }

                var query = from note in noteAppDbContext.Notes
                            where (note.Date >= start && note.Date <= end)
                            select note;
                var notes = query.ToList();
                PrintNotes(notes);
            }            
        }

        public void RemoveNotes(string by, params string[] conditions)
        {
            switch (by)
            {
                case "Id":

                    var notesToRemove = from note in noteAppDbContext.Notes
                                        where conditions.Contains(note.Id.ToString())
                                        select note;
                    foreach (var note in notesToRemove)
                    {
                        noteAppDbContext.Notes.Remove(note);
                    }
                    noteAppDbContext.SaveChanges();
                    break;
                case "Date":

                    DateTime dateRemove;
                    List<DateTime> datesToRemove = new List<DateTime>();
                    foreach (string date in conditions)
                    {
                        if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out dateRemove))
                        {
                            datesToRemove.Add(dateRemove);
                        }
                        else
                        {
                            consoleManager.WriteLine($"Incorrect entry: {date}. Date is expected! Ignored.");
                        }
                    }
                    notesToRemove = from note in noteAppDbContext.Notes
                                        where datesToRemove.Contains(note.Date)
                                        select note;
                    foreach (var note in notesToRemove)
                    {
                        noteAppDbContext.Notes.Remove(note);
                    }
                    noteAppDbContext.SaveChanges();
                    break;
            }
        }

        public void PrintNotes(List<Note> notes)
        {
            foreach (var note in notes)
            {
                consoleManager.WriteLine($"{note.Id}, {note.Note1}," +
                    $" {timeManager.DateToStringWeek(note.Date)}, {timeManager.TimeSpanToString(note.Time)}");
            }
        }
    }
}
