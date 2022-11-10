using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace NoteTakingApp
{
    public class NoteManager: INoteManager
    {
        private readonly IConsoleManager consoleManager;
        private readonly ITimeManager timeManager;
        string NotesFilePath = @"D:\Julia\Documents\Learning\notes\notes.txt";

        public NoteManager(IConsoleManager consoleManager, ITimeManager timeManager)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
        }

        public List<NoteInfo> GetNotes()
        {
            List<NoteInfo> Notes = new List<NoteInfo>();
            ConsoleKey cki;
            do
            {
                consoleManager.WriteLine("Type your note: ");
                string note = consoleManager.ReadLine();
                DateTime dtNow = timeManager.DateTimeNow();
                string date = timeManager.DateToString(dtNow);
                string time = timeManager.TimeToString(dtNow);
                NoteInfo noteInfo = new NoteInfo { Note = note, Date = date, Time = time };

                Notes.Add(noteInfo);

                consoleManager.WriteLine("Press any key to add another note or Esc to exit.");
                cki = consoleManager.ReadKey(true);

            } while (cki != ConsoleKey.Escape);
            return Notes;
        }
        public void SaveNotes(List<NoteInfo> notes)
        {
            bool includeHeader = true;
            if (File.Exists(NotesFilePath)) includeHeader = false;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = includeHeader
            };

            using (var stream = File.Open(NotesFilePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(notes);
            }
        }
        public void ReadNotes()
        {
            TextReader streamReader = null;
            try
            {
                streamReader = File.OpenText(NotesFilePath);
            }
            catch (FileNotFoundException ex)
            {
                consoleManager.WriteLine(ex.Message);
                return;
            }

            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            var notes = csvReader.GetRecords<NoteInfo>();

            foreach (NoteInfo note in notes)
            {
                consoleManager.WriteLine($"Note {note.Note}, taken on {note.Date} at {note.Time}");
            }
        }
    }
}
