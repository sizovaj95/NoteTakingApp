
namespace NoteTakingApp
{
    public class ConsoleManager : IConsoleManager
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public ConsoleKey ReadKey(bool boolean)
        {
            return Console.ReadKey(boolean).Key;
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

    }
}
