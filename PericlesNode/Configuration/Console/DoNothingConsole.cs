using Pericles.Configuration.Console.Interfaces;

namespace Pericles.Configuration.Console
{
    public class DoNothingConsole : IConsole
    {
        public void WriteLine(string line)
        {
            return;
        }
    }
}