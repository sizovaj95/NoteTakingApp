using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NoteTakingApp;
using Moq;
using System;
using FluentAssertions;

namespace UnitTests
{
    [TestClass]
    public class NoteTakingTests 
    {
        Mock<IConsoleManager> consoleManagerMock;
        Mock<ITimeManager> timeManagerMock;

        static List<NoteInfo> expectedNotes;
        static DateTime dateTimeNow;

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

            timeManagerMock.Setup(p => p.DateToString(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("ddd, dd MMMM yyyy"));
            timeManagerMock.Setup(p => p.TimeToString(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("HH:mm"));

            NoteManager noteManager = new NoteManager(consoleManagerMock.Object, timeManagerMock.Object);
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

            timeManagerMock.Setup(p => p.DateToString(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("ddd, dd MMMM yyyy"));
            timeManagerMock.Setup(p => p.TimeToString(It.IsAny<DateTime>())).Returns((DateTime x) => x.ToString("HH:mm"));

            NoteManager noteManager = new NoteManager(consoleManagerMock.Object, timeManagerMock.Object);
            List<NoteInfo> actualNotes = noteManager.GetNotes();
            expectedNotes.Should().BeEquivalentTo(actualNotes);

        }
    }
}		
