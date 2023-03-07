using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
 
namespace NoteTakingApp
{
    public class NoteManagerDb
    {
        private readonly IConsoleManager consoleManager;
        private readonly ITimeManager timeManager;
        private readonly IDbConnection conn;


        public NoteManagerDb(IConsoleManager consoleManager, ITimeManager timeManager,
                           string connectionString)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
            try
            {
                conn = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                consoleManager.WriteLine(ex.Message);
            }            
        }

        public NoteManagerDb(IConsoleManager consoleManager, ITimeManager timeManager,
            IDbConnection connection)
        {
            this.consoleManager = consoleManager;
            this.timeManager = timeManager;
            conn = connection;
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
                // Protect from sql injection
                StringBuilder builder = new StringBuilder("INSERT INTO Note (Note, Date, Time) VALUES ");
                foreach (NoteInfo note in notes)
                {
                    builder.Append($"(N'{note.Note}', N'{note.Date}', N'{note.Time}'),");
                }
                string query = builder.ToString();
                query = query.TrimEnd(',');
                using (IDbCommand command = conn.CreateCommand())
                {
                    command.CommandText = query;
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
            conn.Open();
            IDbCommand command = conn.CreateCommand();
            command.CommandText = query;
            IDataReader reader = command.ExecuteReader();
            if (!reader.Read()) 
            {
                consoleManager.WriteLine("There is no data.");
             }
            else
            {
                while (reader.Read())
                {
                    DateTime date = (DateTime)reader.GetValue(2);
                    consoleManager.WriteLine($"{reader.GetValue(0)}, {reader.GetValue(1)}, {timeManager.DateToStringWeek(date)}, {reader.GetValue(3)}");
                    
                }
            }            
            reader.Close();
            command.Dispose();
            conn.Close();
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
            IDbCommand command = conn.CreateCommand();
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