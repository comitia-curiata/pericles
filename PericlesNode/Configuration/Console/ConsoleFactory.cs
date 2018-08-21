using Pericles.Configuration.Console.Interfaces;

namespace Pericles.Configuration.Console
{
    public class ConsoleFactory
    {
        public IConsole Build(bool isMiningConsole)
        {
            if (isMiningConsole)
            {
                return new MinerConsole();
            }

            return new DoNothingConsole();
        }
    }
}