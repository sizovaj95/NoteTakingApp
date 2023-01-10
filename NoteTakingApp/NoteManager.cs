using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO.Abstractions;

namespace NoteTakingApp
{
    public class NoteManager: INoteManager
    {
        private readonly IConsoleManager consoleManager;
        private readonly ITimeManager timeManager;
        public string notesFilePath;
        private readonly IFileSystem fileSystem;

        public NoteManager(IConsoleManager consoleManager, ITimeManager timeManager)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
            notesFilePath = @"D:\Julia\Documents\Learning\notes\notes.txt";
            fileSystem = new FileSystem();
        }
        public NoteManager(IConsoleManager consoleManager, ITimeManager timeManager,
                           string notesFilePath, IFileSystem fileSystem)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
            this.notesFilePath = notesFilePath;
            this.fileSystem = fileSystem;
        }

        public List<NoteInfo> GetNotes()
        {
            List<NoteInfo> Notes = new List<NoteInfo>();
            ConsoleKey cki;
            do
            {
                consoleManager.WriteLine("Type your note: ");
                string note = consoleManager.ReadLine();
                if (!String.IsNullOrEmpty(note))
                {
                    DateTime dtNow = timeManager.DateTimeNow();
                    string date = timeManager.DateToStringWeek(dtNow);
                    string time = timeManager.TimeToString(dtNow);
                    NoteInfo noteInfo = new NoteInfo { Note = note, Date = date, Time = time };
                    Notes.Add(noteInfo);
                }         

                consoleManager.WriteLine("Press any key to add another note or Esc to exit.");
                cki = consoleManager.ReadKey(true);

            } while (cki != ConsoleKey.Escape);
            return Notes;
        }
        public void SaveNotes(List<NoteInfo> notes)
        {
            bool includeHeader = true;
            if (fileSystem.File.Exists(notesFilePath)) includeHeader = false;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = includeHeader
            };

            using (var stream = fileSystem.File.Open(notesFilePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(notes);
            }
        }
        public void ReadNotes()
        {
            TextReader streamReader = null;
            streamReader = fileSystem.File.OpenText(notesFilePath);
            try
            {
                streamReader = fileSystem.File.OpenText(notesFilePath);
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
