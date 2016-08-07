namespace NHSDataAnalyser
{
    /// <summary>
    /// Collects the file name for the given domain
    /// </summary>
    public interface IInputFileNameCollector
    {
        /// <summary>
        /// Collects the file name from command line.<para> please note if the file does not exist or the name is invalid,
        /// this will prompt the user whether to enter the correct name again. if the user chooses Yes, by pressing
        /// ENTER key, the user will be allowed to enter the correct file name, if user presses any other key, the application
        /// will be exited.
        /// </para>
        /// </summary>
        string Collect(string dataFor);
    }
}