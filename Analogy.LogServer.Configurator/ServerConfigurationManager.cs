using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Analogy.LogServer.Configurator
{
    public class ServerConfigurationManager
    {
        private static readonly Lazy<ServerConfigurationManager> _instance =
            new Lazy<ServerConfigurationManager>(() => new ServerConfigurationManager());
        public static ServerConfigurationManager ConfigurationManager { get; set; } = _instance.Value;
        private string ServerSettingFile { get; } = "appsettings_LogServer.json";

        public ServerConfiguration ServerConfiguration { get; }

        public ServerConfigurationManager()
        {
            try
            {
                string data = File.ReadAllText(ServerSettingFile);
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver(),
                };
                ServerConfiguration = JsonConvert.DeserializeObject<ServerConfiguration>(data, settings);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error loading configuration file: {e.Message}");
            }
        }

        public void Save()
        {
            try
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(ServerSettingFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, ServerConfiguration);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save file {ServerSettingFile}: {ex}");
            }
        }
    }
}