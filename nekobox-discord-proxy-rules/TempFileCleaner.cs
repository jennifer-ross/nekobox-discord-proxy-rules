using System.IO;

namespace nekobox_discord_proxy_rules
{
    public class TempFileCleaner
    {
        public static void CleanUp(string tempDir)
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            if (File.Exists(tempDir))
            {
                File.Delete(tempDir);
            }
        }
    }
}