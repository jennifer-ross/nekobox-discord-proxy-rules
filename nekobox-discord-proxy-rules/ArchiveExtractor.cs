using System.IO;
using System.IO.Compression;
using Console = Colorful.Console;

namespace nekobox_discord_proxy_rules
{
    public class ArchiveExtractor
    {
        public static string ExtractArchive(string archivePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            
            ZipFile.ExtractToDirectory(archivePath, tempDir);

            return tempDir;
        }
    }
}