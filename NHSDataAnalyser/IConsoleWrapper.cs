using System;

namespace NHSDataAnalyser
{
    /// <summary>
    /// Interface for Wrapper methods around System.Console APIs
    /// </summary>
    public interface IConsoleWrapper
    {
        string ReadLine();
        void WriteLine(string message);
        ConsoleKeyInfo ReadKey();
    }
}