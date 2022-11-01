
// See https://aka.ms/new-console-template for more information

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;


string NotesFilePath = @"D:\Julia\Documents\Learning\notes\notes.txt";


List<NoteInfo> Notes = GetNotes();
SaveNotes(Notes);
ReadNotes(NotesFilePath);


List<NoteInfo> GetNotes()
{
    List<NoteInfo> Notes = new List<NoteInfo>();
    ConsoleKeyInfo cki;
    do
    {
        Console.WriteLine("Type your note: ");
        string note = Console.ReadLine();
        DateTime dtNow = DateTime.Now;
        string date = dtNow.ToString("ddd, dd MMMM yyyy");
        string time = dtNow.ToString("HH:mm");
        NoteInfo noteInfo = new NoteInfo { Note = note, Date = date, Time = time };

        Notes.Add(noteInfo);

        Console.WriteLine("Press any key to add another note or Esc to exit.");
        cki = Console.ReadKey(true);
        Console.WriteLine();


    } while (cki.Key != ConsoleKey.Escape);
    return Notes;
}


void SaveNotes(List<NoteInfo> notes)
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
        csv.WriteRecords(Notes);
    }
}


void ReadNotes(string filePath)
{
    TextReader streamReader = null;
    try
    {
        streamReader = File.OpenText(filePath);        
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine(ex.Message);
        return;
    }
    
    using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
    var notes = csvReader.GetRecords<NoteInfo>();

    foreach (NoteInfo note in notes)
    {
        Console.WriteLine($"Note {note.Note}, taken on {note.Date} at {note.Time}");
    }

}

public class NoteInfo
{
    public string Note { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
}

// Add read notes functionality
// tests?