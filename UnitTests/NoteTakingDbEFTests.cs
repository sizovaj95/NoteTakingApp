using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using NoteTakingDbEF.Models;
using Moq;
using NoteTakingApp;

namespace UnitTests
{
    [TestClass]
    public class NoteTakingDbEFTests : IDisposable
    {
        Mock<IConsoleManager> consoleManagerMock;
        ITimeManager timeManager;

        private readonly DbConnection _connection;
        private readonly DbContextOptions<NoteAppDbContext> _contextOptions;

        static List<Note> expectedNotes;
        static DateTime dtNow;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            dtNow = new DateTime(2023, 3, 2, 7, 14, 50);

            expectedNotes = new List<Note>();
            expectedNotes.Add(new Note { Note1 = "Buy milk", Date = dtNow.Date, Time = dtNow.TimeOfDay });
            expectedNotes.Add(new Note { Note1 = "Feed capybara", Date = dtNow.Date, Time = dtNow.AddMinutes(1).TimeOfDay });
            expectedNotes.Add(new Note { Note1 = "Pay bills", Date = dtNow.AddDays(7).Date, Time = dtNow.AddDays(7).TimeOfDay });
        }
        
        public NoteTakingDbEFTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<NoteAppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new NoteAppDbContext(_contextOptions);

            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
                    CREATE VIEW AllResources AS
                    SELECT * 
                    FROM Note;";
                viewCommand.ExecuteNonQuery();

            }
        }

        NoteAppDbContext CreateContext() => new NoteAppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        [TestInitialize]
        public void Setup()
        {
            using NoteAppDbContext context = CreateContext();
            
            consoleManagerMock = new Mock<IConsoleManager>();
            timeManager = new TimeManager();
            context.AddRange(expectedNotes);
            context.SaveChanges();

        }

        [TestMethod]
        public void TestReadNotesAll()
        {            
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);

            foreach (Note note in expectedNotes)
            {
                consoleManagerMock.Setup(p => p.WriteLine($"{note.Id}, {note.Note1}, {timeManager.DateToStringWeek(note.Date)}," +
                    $" {timeManager.TimeSpanToString(note.Time)}")).Verifiable();
            }

            noteManagerDbEf.ReadNotes();
            consoleManagerMock.VerifyAll();
        }

        [TestMethod]
        public void TestReadNotesByDate()
        {
            string startDate = "2023-03-02";
            string endDate = "2023-03-03";
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);
     
            for (int i = 0; i < expectedNotes.Count-1; i++)
            {
                Note note = expectedNotes[i];
                consoleManagerMock.Setup(p => p.WriteLine($"{note.Id}, {note.Note1}, {timeManager.DateToStringWeek(note.Date)}," +
                        $" {timeManager.TimeSpanToString(note.Time)}")).Verifiable();
            }

            noteManagerDbEf.ReadNotes(startDate, endDate);
            consoleManagerMock.VerifyAll();
        }

        [TestMethod]
        public void TestReadNotesIncorrectDateFormat()
        {
            string startDate = "02/03/2023";
            string endDate = "2023-03-03";
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);

            consoleManagerMock.Setup(p => p.WriteLine("Incorrect start date or date format!")).Verifiable();

            noteManagerDbEf.ReadNotes(startDate, endDate);
            consoleManagerMock.VerifyAll();
        }

        [TestMethod]
        public void TestReadNotesIncorrectDateOrder()
        {
            string endDate = "2023-03-02";
            string startDate = "2023-03-03";
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);

            consoleManagerMock.Setup(p => p.WriteLine("Start Date is after end date!")).Verifiable();

            noteManagerDbEf.ReadNotes(startDate, endDate);
            consoleManagerMock.VerifyAll();
        }

        [TestMethod]
        public void TestSaveNotes()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);
            DateTime newDtNow = new DateTime(2023, 3, 3, 12, 00, 00);
            Note newNote = new Note { Note1 = "Book taxi", Date = newDtNow, Time = newDtNow.TimeOfDay };

            noteManagerDbEf.SaveNotes(new List<Note> { newNote });

            var note = context.Notes.Single(n => n.Note1 == "Book taxi");

            Assert.AreEqual("Book taxi", note.Note1);
            Assert.AreEqual(4, note.Id);
        }

        [TestMethod]
        public void TestRemoveNotesById()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);


            noteManagerDbEf.RemoveNotes("Id", "1");

            var query = from note in context.Notes
                        where note.Id == 1
                        select note;

            var noteWithId1 = query.ToList();
            var allNotes = context.Notes.ToList();

            Assert.AreEqual(0, noteWithId1.Count);
            Assert.AreEqual(2, allNotes.Count);

        }

        [TestMethod]
        public void TestRemoveNotesByTwoIds()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);

            string[] removeId = { "1", "2" };
            noteManagerDbEf.RemoveNotes("Id", removeId);

            var query = from note in context.Notes
                        where removeId.Contains(note.Id.ToString())
                        select note;

            var noteWithRemoveId = query.ToList();
            var allNotes = context.Notes.ToList();

            Assert.AreEqual(0, noteWithRemoveId.Count);
            Assert.AreEqual(1, allNotes.Count);
        }

        [TestMethod]
        public void TestRemoveNotesByDate()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);

            noteManagerDbEf.RemoveNotes("Date", "2023-03-02");

            var query = from note in context.Notes
                        where note.Date == dtNow.Date
                        select note;

            var noteWithRemoveDate = query.ToList();
            var allNotes = context.Notes.ToList();

            Assert.AreEqual(0, noteWithRemoveDate.Count);
            Assert.AreEqual(1, allNotes.Count);

        }

        [TestMethod]
        public void TestRemoveNotesByDateIncorrectDateFormat()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);


            noteManagerDbEf.RemoveNotes("Date", "2023/03/02");

            var query = from note in context.Notes
                        where note.Date == dtNow.Date
                        select note;

            var noteWithRemoveDate = query.ToList();
            var allNotes = context.Notes.ToList();

            Assert.AreEqual(3, allNotes.Count);

        }

        [TestMethod]
        public void TestRemoveNonExistingNote()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);


            noteManagerDbEf.RemoveNotes("Id", "100");

            var query = from note in context.Notes
                        where note.Id == 1
                        select note;

            var noteWithId1 = query.ToList();
            var allNotes = context.Notes.ToList();

            Assert.AreEqual(3, allNotes.Count);
        }

        [TestMethod]
        public void TestRemoveMixedConditions()
        {
            using NoteAppDbContext context = CreateContext();
            NoteManagerDbEf noteManagerDbEf = new NoteManagerDbEf(consoleManagerMock.Object, timeManager, context);


            noteManagerDbEf.RemoveNotes("Id", "1", "2023-03-02");

            var query = from note in context.Notes
                        where note.Id == 1
                        select note;

            var noteWithId1 = query.ToList();

            query = from note in context.Notes
                        where note.Date == dtNow.Date
                        select note;
            var noteWithRemoveDate = query.ToList();
            var allNotes = context.Notes.ToList();

            Assert.AreEqual(0, noteWithId1.Count);
            Assert.AreEqual(1, noteWithRemoveDate.Count);
            Assert.AreEqual(2, allNotes.Count);
        }
    }
}
