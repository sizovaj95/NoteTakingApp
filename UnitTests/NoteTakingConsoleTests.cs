using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NoteTakingApp;
using Moq;
using System;
using FluentAssertions;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
 
namespace UnitTests
{
    [TestClass]
    public class NoteTakingConsoleTests 
    {
        Mock<IConsoleManager> consoleManagerMock;
        Mock<ITimeManager> timeManagerMock;
        MockFileSystem mockFileSystem;
        NoteManagerConsole noteManager;

        static List<NoteInfo> expectedNotes;
        static DateTime dateTimeNow;

        public string saveDir = Path.Combine(Directory.GetCurrentDirectory(), "notes.txt");

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            expectedNotes = new List<NoteInfo>();
            expectedNotes.Add(new NoteInfo { Note = "Buy milk", Date = "Thu, 17 November 2022", Time = "07:14" });
            expectedNotes.Add(new NoteInfo { Note = "Pay bills", Date = "Thu, 17 November 2022", Time = "07:15" });
            expectedNotes.Add(new NoteInfo { Note = "Feed capybara", Date = "Thu, 17 November 2022", Time = "07:16" });

            dateTimeNow = new DateTime(2022, 11, 17, 7, 14, 50);
        }
        
        [TestInitialize]
        public void Setup()
        {
            consoleManagerMock = new Mock<IConsoleManager>();
            timeManagerMock = new Mock<ITimeManager>();
            mockFileSystem = new MockFileSystem();
            noteManager = new NoteManagerConsole(consoleManagerMock.Object, timeManagerMock.Object,
                                          saveDir, mockFileSystem);
        }

        [TestMethod]
        public void GetNotesIdealCase()
        {
            consoleManagerMock.SetupSequence(p => p.WriteLine(It.IsAny<string>()));
            consoleManagerMock.SetupSequence(p => p.ReadLine())
                .Returns("Buy milk")
                .Returns("Pay bills")
                .Returns("Feed capybara");
            consoleManagerMock.SetupSequence(p => p.ReadKey(true))
                .Returns(ConsoleKey.Enter)
                .Returns(ConsoleKey.Enter)
                .Returns(ConsoleKey.Escape);

            timeManagerMock.SetupSequence(p => p.DateTimeNow())
                .Returns(dateTimeNow)
                .Returns(dateTimeNow.AddMinutes(1))
                .Returns(dateTimeNow.AddMinutes(2));

            timeManagerMock.Setup(p => p.DateToStringWeek(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("ddd, dd MMMM yyyy"));
            timeManagerMock.Setup(p => p.TimeToString(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("HH:mm"));

            List<NoteInfo> actualNotes = noteManager.GetNotes();
            expectedNotes.Should().BeEquivalentTo(actualNotes);
            
        }

        [TestMethod]
        public void GetNotesEmptyNote()
        {
            consoleManagerMock.SetupSequence(p => p.WriteLine(It.IsAny<string>()));
            consoleManagerMock.SetupSequence(p => p.ReadLine())
                .Returns("Buy milk")
                .Returns("Pay bills")
                .Returns("")
                .Returns("Feed capybara");
            consoleManagerMock.SetupSequence(p => p.ReadKey(true))
                .Returns(ConsoleKey.Enter)
                .Returns(ConsoleKey.Enter)
                .Returns(ConsoleKey.Enter)
                .Returns(ConsoleKey.Escape);

            timeManagerMock.SetupSequence(p => p.DateTimeNow())
                .Returns(dateTimeNow)
                .Returns(dateTimeNow.AddMinutes(1))
                .Returns(dateTimeNow.AddMinutes(2));

            timeManagerMock.Setup(p => p.DateToStringWeek(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("ddd, dd MMMM yyyy"));
            timeManagerMock.Setup(p => p.TimeToString(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("HH:mm"));

            List<NoteInfo> actualNotes = noteManager.GetNotes();
            expectedNotes.Should().BeEquivalentTo(actualNotes);
        }

        [TestMethod]
        public void SaveNotesFileExists()
        {
            string mockString = "Note,Date,Time\nKill all humans!,\"Wed, 23 November 2022\",19:00\n";
        
            var mockInputFile = new MockFileData(mockString);
            mockFileSystem.AddFile(saveDir, mockInputFile);
            noteManager.SaveNotes(expectedNotes);
            MockFileData mockOutputFile = mockFileSystem.GetFile(saveDir);
            string[] outputLines = mockOutputFile.TextContents.Split(new String[] { "\r\n",  "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(5, outputLines.Length);
        }

        [TestMethod]
        public void SaveNotesFileNotExist()
        {
            mockFileSystem.AddDirectory(Directory.GetCurrentDirectory());
            noteManager.SaveNotes(expectedNotes);
            MockFileData mockOutputFile = mockFileSystem.GetFile(saveDir);
            string[] outputLines = mockOutputFile.TextContents.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(4, outputLines.Length);
        }

        [TestMethod]
        public void ReadNotesExistsCase()
        {
            string header = "Note,Date,Time\n";
            string mockString = "";
            foreach (NoteInfo note in expectedNotes)
            {
                mockString += $"{note.Note},\"{note.Date}\",{note.Time}\n"; 
                consoleManagerMock.Setup(p => p.WriteLine($"Note {note.Note}, taken on {note.Date} at {note.Time}")).Verifiable();
            }
            string fullMockString = header + mockString;
            var mockInputFile = new MockFileData(fullMockString);
            mockFileSystem.AddFile(saveDir, mockInputFile);

            noteManager.ReadNotes();
            consoleManagerMock.VerifyAll();
        }

        [TestMethod]
        public void ReadNotesNotExistCase()
        {
            try
            {
                noteManager.ReadNotes();
            }
            catch (FileNotFoundException) { }
            
        }

    }
}		
