using Microsoft.Extensions.Configuration;


namespace NoteTakingApp
{

    public class Program
    {
        static void Main()
        {
            ConsoleManager consoleManager = new ConsoleManager();
            TimeManager timeManager = new TimeManager();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            string connectionString = config["ConnectionStrings:SqlServerConn"];

            NoteManagerDb noteManager = new NoteManagerDb(consoleManager, timeManager, connectionString);
            ConsoleKey action;
            ConsoleKey readOption;
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
                        readOption = Console.ReadKey(true).Key;
                        switch (readOption)
                        {
                            case ConsoleKey.A:
                                noteManager.ReadNotes();
                                break;
                            case ConsoleKey.D:
                                Console.Write("Please enter start date (yyyy-mm-dd): ");
                                string startDate = Console.ReadLine();
                                Console.Write("Please enter end date (yyyy-mm-dd): ");
                                string endDate = Console.ReadLine();
                                noteManager.ReadNotes(startDate, endDate);
                                break;
                            case ConsoleKey.Escape:
                                break;
                            default:
                                consoleManager.WriteLine($"There is not such option: {readOption}");
                                break;
                        }
                        consoleManager.WriteLine("\nPress enter to exit");
                        Console.ReadLine();
                        
                        break;
                    case ConsoleKey.D3:
                        consoleManager.WriteLine(
                            "i - remove by Id\n" +
                            "d - remove by Date\n" +
                            "Esc - exit to main menu\n");
                        readOption = Console.ReadKey(true).Key;
                        switch (readOption)
                        {
                            case ConsoleKey.I:
                                consoleManager.WriteLine("IDs to remove (separated by commas): ");
                                string ids = Console.ReadLine();
                                string[] idSplit = ids.Split(new char[] { ',', ' '});
                                noteManager.RemoveNotes("Id", idSplit);
                                break;
                            case ConsoleKey.D:
                                consoleManager.WriteLine("Dates to remove in \"yyyy-mm-dd\" format (separated by commas): ");
                                string dates = Console.ReadLine();
                                string[] datesSplit = dates.Split(new char[] { ',', ' ' });
                                noteManager.RemoveNotes("Date", datesSplit);
                                break;
                            case ConsoleKey.Escape:
                                break;
                            default:
                                consoleManager.WriteLine($"There is not such option: {readOption}");
                                break;
                        }
                        break;
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
    }
}
