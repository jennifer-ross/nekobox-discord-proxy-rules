using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace nekobox_discord_proxy_rules
{
    public class Config
    {
        public int Version { get; private set; } // Property to store the config version
        public string RulesConfigUrl { get; private set; } // Property to store the RulesConfigUrl field
        public string DiscordDroverUrl { get; private set; } // Property to store the DiscordDroverUrl field
        public string Content { get; private set; } // Property to store the raw JSON content
        public string RulesContent { get; private set; } // Property to store the raw rules JSON content

        // Constructor to initialize Config object from JSON content
        public Config(string jsonContent)
        {
            this.Content = jsonContent;
            this.Version = ParseVersionFromJson(jsonContent);
            this.RulesConfigUrl = ParseRulesConfigFromJson(jsonContent);
            this.DiscordDroverUrl = ParseDiscordDroverUrlFromJson(jsonContent);
        }
        
        // Method to parse the DiscordDroverUrl field from the JSON content
        private string ParseDiscordDroverUrlFromJson(string jsonContent)
        {
            JObject json = JObject.Parse(jsonContent);
            return (string)json["DiscordDroverUrl"];
        }
        
        // Method to parse the RulesConfigUrl field from the JSON content
        private string ParseRulesConfigFromJson(string jsonContent)
        {
            JObject json = JObject.Parse(jsonContent);
            return (string)json["RulesConfigUrl"];
        }

        // Method to parse the version field from the JSON content
        private int ParseVersionFromJson(string jsonContent)
        {
            JObject json = JObject.Parse(jsonContent);
            return (int)json["Version"];
        }

        // Static method to load a config from a local file
        public static Config LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                Config config =new Config(fileContent);
                config.RulesContent = DownloadRulesFromUrl(config.RulesConfigUrl);
                return config;
            }
            else
            {
                throw new FileNotFoundException($"Config file not found at {filePath}");
            }
        }

        public static string DownloadRulesFromUrl(string url)
        {
            return FileDownloader.DownloadString(url);
        }

        // Static method to download a config from a remote URL
        public static Config DownloadFromUrl(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            
            string jsonContent = FileDownloader.DownloadString(url);
            
            Config config =  config = new Config(jsonContent);;
            config.RulesContent = DownloadRulesFromUrl(config.RulesConfigUrl);

            return config;
        }

        // Method to save the current config to a local file
        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, this.Content);
        }

        // Method to compare the version of this config with another config
        public bool IsOutdatedComparedTo(Config otherConfig)
        {
            return this.Version != otherConfig.Version;
        }
    }
}