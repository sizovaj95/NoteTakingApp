using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace NoteTakingApp
{
    public class NoteManager: INoteManager
    {
        private readonly IConsoleManager consoleManager;
        private readonly ITimeManager timeManager;
        private string connectionString;
        readonly SqlConnection conn;


        public NoteManager(IConsoleManager consoleManager, ITimeManager timeManager,
                           string connectionString)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
            this.connectionString = connectionString;
            try
            {
                conn = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                consoleManager.WriteLine(ex.Message);
            }
            
        }

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
                if (!String.IsNullOrEmpty(note))
                {
                    DateTime dtNow = timeManager.DateTimeNow();
                    string date = timeManager.DateToStringDash(dtNow);
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
            try
            {
                conn.Open();
                StringBuilder builder = new StringBuilder();
                builder.Append("INSERT INTO Note (Note, Date, Time) VALUES ");
                foreach (NoteInfo note in notes)
                {
                    builder.Append($"(N'{note.Note}', N'{note.Date}', N'{note.Time}'),");
                }
                string query = builder.ToString();
                query = query.TrimEnd(',');
                SqlCommand command = new SqlCommand(query, conn);
                using (command)
                {
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                consoleManager.WriteLine(ex.ToString());
            }
            
        }
        public void ReadNotes()
        {
            try
            {
                string query = "SELECT * FROM Note";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        DateTime date = (DateTime)row[2];
                        consoleManager.WriteLine($"{row[0]}, {row[1]}, {timeManager.DateToStringWeek(date)}, {row[3]}");
                    }
                }
            }
            catch (Exception ex)
            {
                consoleManager.WriteLine(ex.ToString());
            }
        }

        public void ReadNotes(string startDate, string endDate)
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
            try
            {
                string query = $"SELECT * FROM Note WHERE Date between \'{startDate}\' and \'{endDate}\'";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                using (adapter)
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        DateTime date = (DateTime)row[2];
                        Console.WriteLine($"{row[0]}, {row[1]}, {timeManager.DateToStringWeek(date)}, {row[3]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
