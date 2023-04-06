using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using NoteTakingApp;
using NoteTakingDbEF.Models;


namespace NoteTakingAppWPF6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NoteAppDbContext dbContext = new NoteAppDbContext();
        ConsoleManager consoleManager = new ConsoleManager();
        TimeManager timeManager = new TimeManager();
        NoteManagerDbEf noteManager;

        public MainWindow()
        {
            InitializeComponent();
            ShowAllNotes();

            noteManager = new NoteManagerDbEf(consoleManager, timeManager, dbContext);

        }

        private void ShowAllNotes()
        {
            var notes = dbContext.Notes.ToList();
            List<NoteDetails> notesConverted = new List<NoteDetails>();
            
            foreach (Note note in notes)
            {
                string date = timeManager.DateToStringWeek(note.Date);
                string time = string.Format("{0:hh\\:mm}", note.Time);                
                NoteDetails nd = new NoteDetails { Id=note.Id, Note=note.Note1, Date=date, Time=time};
                notesConverted.Add(nd);
            }
            notesDataGrid.ItemsSource = notesConverted;
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            string newNote = newNoteBox.Text;
            if (newNote != null)
            {
                newNote = newNote[0].ToString().ToUpper() + newNote.Substring(1);
                DateTime dtNow = DateTime.Now;
                Note noteInfo = new Note { Note1= newNote, Date = dtNow, Time=dtNow.TimeOfDay };
                List<Note> notes = new List<Note> { noteInfo };
                noteManager.SaveNotes(notes);
                newNoteBox.Text = "";
                ShowAllNotes();
            }
        }

        private void RemoveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            NoteDetails nd = (NoteDetails)notesDataGrid.SelectedValue;
            if (nd != null)
            {
               
                int noteId = nd.Id;
                string noteIdString = noteId.ToString();
                noteManager.RemoveNotes("Id", new string[] { noteIdString });
            }
            else
            {
                MessageBox.Show("Select a note to remove!");
            }
            ShowAllNotes();
        }
    }

    public class NoteDetails
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

}
