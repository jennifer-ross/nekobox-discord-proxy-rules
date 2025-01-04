using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Colorful;
using Console = Colorful.Console;

namespace nekobox_discord_proxy_rules
{
    public static class State
    {
        public static Config AppConfig { get; set; }
    }
    
    internal class Program
    {
        private static string NekoBoxProgramName { get; } = "nekoray";
        private static string NekoBoxRegex { get; } = @"(-tray|nekoray\.exe|nekoray_core\.exe)";
        static private string NekoBoxPath { get; set; } = "";
        static private string ConfigName { get;} = "config";
        static private string ConfigLocal { get;} = $"{ConfigName}.json";
        static private string ConfigRemote { get;} = $"https://raw.githubusercontent.com/jennifer-ross/nekobox-discord-proxy-rules/refs/heads/master/data/{ConfigLocal}";
        
        public static void Main(string[] args)
        {
            // Get the current directory
            string currentDir = Directory.GetCurrentDirectory();
            
            // Create an instance of ConfigUpdater
            ConfigUpdater updater = new ConfigUpdater($"{currentDir}\\{ConfigLocal}", ConfigRemote);
            
            // Check and update the local configuration if necessary
            updater.CheckAndUpdateConfig();
            
            // Create an instance of StartupManager
            StartupManager startupManager = new StartupManager();
            
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string discordPath = Path.Combine(localAppDataPath, "Discord");

            List<string> discordAppPaths = startupManager.GetProgramsPath(discordPath, "app-");

            if (discordAppPaths.Count > 0)
            {
                Console.WriteLineFormatted("Starting Installation Drover for Discord...", Color.Blue);

                var archivePath = $"{currentDir}\\drover.zip";
                FileDownloader.DownloadFile(State.AppConfig.DiscordDroverUrl, archivePath);
                var tempDir = Path.Combine(ArchiveExtractor.ExtractArchive(archivePath), "drover");
                
                foreach (var discordAppPath in discordAppPaths)
                {
                    Console.WriteLineFormatted("Installing to {0}", Color.Blue, new Formatter(discordAppPath, Color.Green));
                    foreach (var file in Directory.GetFiles(tempDir))
                    {
                        if (file == Path.Combine(tempDir, "drover.exe"))
                        {
                            continue;
                        }      
                        
                        if (file == Path.Combine(tempDir, "drover.ini"))
                        {
                            var fileStream = File.CreateText(Path.Combine(discordAppPath, "drover.ini"));
                            fileStream.Write("[drover]\nuse-nekobox-proxy = 1\nnekobox-proxy = http://127.0.0.1:2080");
                            fileStream.Close();
                            continue;
                        }
                        
                        string destination = Path.Combine(discordAppPath, Path.GetFileName(file));
                        File.Copy(file, destination, true);
                    }
                }
                
                TempFileCleaner.CleanUp(tempDir);
                TempFileCleaner.CleanUp(archivePath);
                
                Console.WriteLineFormatted("Drover for Discord installed successfully.", Color.Green);
            }
            
            Console.WriteLineFormatted("Starting configure NekoBox...", Color.Blue);
            
            // Define the program name to search in startup
            ProgramInfo programInfo = new ProgramInfo(NekoBoxProgramName);
            bool isProgramInStartup = startupManager.IsProgramInStartup(programInfo);
            
            if (!isProgramInStartup)
            {
                programInfo = new ProgramInfo($"{NekoBoxProgramName}_core");
                isProgramInStartup = startupManager.IsProgramInStartup(programInfo);
            } 

            if (!isProgramInStartup)
            {
                // If the program is not found in startup, request the path from the user
                Console.WriteLineFormatted("NekoBox is not found in startup.", Color.Blue);
                startupManager.RequestExecutablePathFromUser(programInfo);

                // Add the program to startup if the user provided a valid path
                if (string.IsNullOrEmpty(programInfo.ExecutablePath))
                {
                    Console.WriteLineFormatted("Error! No valid path was provided.", Color.Red);
                    Console.WriteLineFormatted("Press any key to exit...", Color.White);
                    Console.ReadKey();
                    return;
                }
            }
            
            NekoBoxPath = Regex.Replace(programInfo.ExecutablePath, NekoBoxRegex, "").Replace("\"", "").Trim();
            Console.WriteLineFormatted("NekoBox Path: {0}", Color.Blue, new Formatter(NekoBoxPath, Color.Green));
            string nekoBoxConfigPath = $"{NekoBoxPath}config\\routes_box";
            string nekoBoxConfigFile = $"{nekoBoxConfigPath}\\Default";
            
            if (!Directory.Exists(nekoBoxConfigPath))
            {
                Console.WriteLineFormatted("Error! No valid NekoBox config path.", Color.Red);
                Console.WriteLineFormatted("Press any key to exit...", Color.White);
                Console.ReadKey();
            }
            
            TempFileCleaner.CleanUp(nekoBoxConfigFile);
            var nekoBoxConfigFileStream = File.CreateText(nekoBoxConfigFile);
            nekoBoxConfigFileStream.Write(State.AppConfig.RulesContent);
            nekoBoxConfigFileStream.Close();
            Console.WriteLineFormatted("NekoBox was configured successfully.", Color.Green);
            
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}