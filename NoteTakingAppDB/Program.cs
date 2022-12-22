using System.Data.SqlClient;
using System.Text;
using System.IO;


namespace NoteTakingApp
{

    public class Program
    {
        static void Main()
        {
            ConsoleManager consoleManager = new ConsoleManager();
            TimeManager timeManager = new TimeManager();
            string connectionString = GetConnectionString();
            NoteManager noteManager = new NoteManager(consoleManager, timeManager, connectionString);
            ConsoleKey action;
            do
            {
                Console.Clear();
                MainMenu();
                action = Console.ReadKey(true).Key;
                Console.Clear();
                switch (action)
                {
                    case ConsoleKey.D1:
                        List<NoteInfo> notes = noteManager.GetNotes();
                        noteManager.SaveNotes(notes);
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine(
                            "a - print all notes\n" +
                            "d - enter date range\n" +
                            "Esc - exit to main menu\n" );
                        ConsoleKey readOption = Console.ReadKey(true).Key;
                        switch (readOption)
                        {
                            case ConsoleKey.A:
                                noteManager.ReadNotes();
                                Console.ReadLine();
                                break;
                            case ConsoleKey.D:
                                // WHY THIS TEXT APPEARS ON SAME LINE ON THE SECOND ATTEMPT
                                Console.Write("Please enter start date (yyyy-mm-dd): ");
                                string startDate = Console.ReadLine();
                                Console.Write("Please enter end date (yyyy-mm-dd): ");
                                string endDate = Console.ReadLine();
                                noteManager.ReadNotes(startDate, endDate);
                                Console.ReadLine();
                                break;                                
                        }
                        break;
                    case ConsoleKey.D3:
                        throw new NotImplementedException();
                    case ConsoleKey.Escape:
                        break;
                    
                    default:
                        Console.WriteLine($"There is not such option: {action}");
                        break;
                }

            } while (action != ConsoleKey.Escape);
            
            Console.ReadLine();
          
        }

        static void MainMenu()
        {
            Console.WriteLine("Please state what you want to do:\n" +
                    "1 - Add and save notes\n" +
                    "2 - Read notes\n" +
                    "3 - Remove notes\n" +
                    "or press Esc to exit");
        }

        static string GetConnectionString()
        {
            var root = Path.GetPathRoot(Directory.GetCurrentDirectory());
            var path = Path.Combine(root, "Julia/Documents/Learning/connectionString.txt");
            string connString = File.ReadAllText(path);
            return connString;
        }
    }
}
