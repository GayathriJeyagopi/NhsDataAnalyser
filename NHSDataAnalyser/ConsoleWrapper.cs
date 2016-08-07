using System;

namespace NHSDataAnalyser
{
    /// <summary>
    /// Wrapper class around System.Console APIs
    /// </summary>
    public class ConsoleWrapper : IConsoleWrapper
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();

        }
    }
}