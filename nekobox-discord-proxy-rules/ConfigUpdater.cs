using System;
using System.Drawing;
using System.IO;
using Colorful;
using Console = Colorful.Console;

namespace nekobox_discord_proxy_rules
{
   public class ConfigUpdater
    {
        private string localFilePath { get; set; }
        private string remoteUrl { get; set; }

        // Constructor to initialize the updater with local file path and remote URL
        public ConfigUpdater(string localFilePath, string remoteUrl)
        {
            this.localFilePath = localFilePath;
            this.remoteUrl = remoteUrl;
        }

        // Method to check if the local config is outdated and update it if necessary
        public void CheckAndUpdateConfig()
        {
            try
            {
                // Load the local config
                Config localConfig = Config.LoadFromFile(localFilePath);
                Console.WriteLineFormatted("Loaded local config with version: {0}", Color.Blue, new Formatter(localConfig.Version, Color.Green));

                // Download the remote config
                Config remoteConfig = Config.DownloadFromUrl(remoteUrl);
                Console.WriteLineFormatted("Downloaded remote config with version: {0}", Color.Blue, new Formatter(remoteConfig.Version, Color.Green));

                // Compare versions and update if necessary
                if (localConfig.IsOutdatedComparedTo(remoteConfig))
                {
                    Console.WriteLineFormatted("Local configuration is outdated. Updating...", Color.Blue);
                    remoteConfig.SaveToFile(localFilePath);
                    State.AppConfig = remoteConfig;
                    Console.WriteLineFormatted("Configuration updated successfully.", Color.Green);
                }
                else
                {
                    State.AppConfig = localConfig;
                    Console.WriteLineFormatted("Local configuration is up-to-date.", Color.Blue);
                }
            }
            catch (FileNotFoundException ex)
            {
                // If the local config doesn't exist, save the remote config
                Console.WriteLineFormatted(ex.Message, Color.White);
                Console.WriteLineFormatted("Saving the remote configuration as the new local config...", Color.White);
                Config remoteConfig = Config.DownloadFromUrl(remoteUrl);
                remoteConfig.SaveToFile(localFilePath);
                State.AppConfig = remoteConfig;
                Console.WriteLineFormatted("Configuration saved successfully.", Color.Green);
            }
        }
    }
}