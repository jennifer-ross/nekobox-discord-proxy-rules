namespace nekobox_discord_proxy_rules
{
    public class ProgramInfo
    {
        public string ProgramName { get; private set; }
        public string ExecutablePath { get; set; } // Path to the executable, which may be set by the user

        // Constructor to initialize program info with the name
        public ProgramInfo(string programName)
        {
            ProgramName = programName;
        }
    }

}