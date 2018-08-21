using Pericles.Configuration.Console.Interfaces;

namespace Pericles.Configuration.Console
{
    public class MinerConsole : IConsole
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }
    }
}