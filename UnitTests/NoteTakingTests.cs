using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NoteTakingApp;
using Moq;
using System;

namespace UnitTests
{
    [TestClass]
    public class NoteTakingTests
    {
        [TestMethod]
        public void GetNotes1()
        {
            List<string> expectedNotes = new List<string>()
            {
                "Buy milk",
                "Pay bills",
                "Feed capybara"
            };
            var consoleManagerMock = new Mock<IConsoleManager>();
            var timeManagerMock = new Mock<ITimeManager>();
            consoleManagerMock.Setup(p => p.ReadLine()).Returns("Buy milk");
            consoleManagerMock.Setup(p => p.WriteLine("Buy milk"));
            consoleManagerMock.Setup(p => p.ReadKey(true)).Returns(ConsoleKey.Enter);

            NoteManager noteManager = new NoteManager(consoleManagerMock.Object, timeManagerMock.Object);
            List<NoteInfo> actualNotes = noteManager.GetNotes();
            Assert.AreEqual(expectedNotes.Count, actualNotes.Count);
        }
    }
}