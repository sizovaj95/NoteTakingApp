using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NoteTakingApp;

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
            NoteManager noteManager = new NoteManager();
            List<NoteInfo> actualNotes = noteManager.GetNotes();
            Assert.AreEqual(expectedNotes.Count, actualNotes.Count);
        }
    }
}