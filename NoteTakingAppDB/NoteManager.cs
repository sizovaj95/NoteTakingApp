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
                StringBuilder builder = new StringBuilder("INSERT INTO Note (Note, Date, Time) VALUES ");
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
                QueryDB(query);
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
                QueryDB(query);
            }
            catch (Exception ex)
            {
                consoleManager.WriteLine(ex.ToString());
            }
        }

        public void QueryDB(string query)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            using (adapter)
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count == 0)
                {
                    consoleManager.WriteLine("There is no data.");
                }
                else
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DateTime date = (DateTime)row[2];
                        consoleManager.WriteLine($"{row[0]}, {row[1]}, {timeManager.DateToStringWeek(date)}, {row[3]}");
                    }
                }                
            }
        }
        public void RemoveNotes(string by, params string[] conditions)
        {
            StringBuilder builder = new StringBuilder("DELETE FROM Note WHERE ");
            string query = "";
            switch (by)
            {
                case "Id":
                    builder.Append("Id IN (");
                    foreach (string cond in conditions)
                    {
                        if (int.TryParse(cond, out int _))
                        {
                            builder.Append($"{cond},");
                        }
                        else
                        {
                            consoleManager.WriteLine($"Incorrect entry: {cond}. Integer Id is expected! Ignored.");
                        }
                    }
                    builder.Remove(builder.Length - 1, 1); // remove last comma
                    builder.Append(')');
                    query = builder.ToString();
                    break;
                case "Date":
                    builder.Append("Date IN (");
                    foreach (string cond in conditions)
                    {
                        if (DateTime.TryParseExact(cond, "yyyy-mm-dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out _))
                        {
                            builder.Append($"\'{cond}\',");
                        }
                        else
                        {
                            consoleManager.WriteLine($"Incorrect entry: {cond}. Date is expected! Ignored.");
                        }
                    }
                    builder.Remove(builder.Length - 1, 1);
                    builder.Append(')');
                    query = builder.ToString();
                    break;               
            }
            conn.Open();
            SqlCommand command = new SqlCommand(query, conn);
            using (command)
            {
                command.ExecuteNonQuery();
            }
            conn.Close();
            
        }
    }
}


// Ctrl+K, C
// Ctrl+K, U