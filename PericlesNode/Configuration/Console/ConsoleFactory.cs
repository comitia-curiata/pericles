using Pericles.Configuration.Console.Interfaces;

namespace Pericles.Configuration.Console
{
    public static class ConsoleFactory
    {
        public static IConsole Build(bool isMiningConsole)
        {
            if (isMiningConsole)
            {
                return new MinerConsole();
            }

            return new DoNothingConsole();
        }
    }
}