using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace nekobox_discord_proxy_rules
{
    public class StartupManager
    {
        private const string StartupRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; // Path to the registry key for startup programs

        // Method to check if the program is in startup
        public bool IsProgramInStartup(ProgramInfo program)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryPath, false))
            {
                if (key != null)
                {
                    // Check if the program is listed in the startup registry
                    string startupValue = (string)key.GetValue(program.ProgramName);
                    if (!string.IsNullOrEmpty(startupValue))
                    {
                        program.ExecutablePath = startupValue; // Set the executable path if found
                        return true;
                    }
                }
            }
            return false;
        }

        // Method to add the program to startup
        public void AddProgramToStartup(ProgramInfo program)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryPath, true))
            {
                if (key != null)
                {
                    key.SetValue(program.ProgramName, program.ExecutablePath);
                    Console.WriteLine($"{program.ProgramName} has been added to startup.");
                }
            }
        }

        // Method to ask the user for the executable path
        public void RequestExecutablePathFromUser(ProgramInfo program)
        {
            Console.Write($"Please enter the full path with slashes: ");
            program.ExecutablePath = Console.ReadLine();
        }

        public List<string> GetProgramsPath(string directory, string searchPattern)
        {
            return Directory.GetDirectories(directory)
                .Where(d => d != null && d.Contains(searchPattern))
                .ToList();
        }
    }
}