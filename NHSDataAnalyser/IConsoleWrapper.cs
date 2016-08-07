using System;

namespace NHSDataAnalyser
{
    public interface IConsoleWrapper
    {
        string ReadLine();
        void WriteLine(string message);
        ConsoleKeyInfo ReadKey();
    }
}