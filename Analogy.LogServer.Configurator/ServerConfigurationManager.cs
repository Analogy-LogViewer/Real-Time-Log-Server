#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Analogy.LogServer.Configurator
{
    public class ServerConfigurationManager
    {
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
        private static readonly Lazy<ServerConfigurationManager> instance =
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
            new Lazy<ServerConfigurationManager>(() => new ServerConfigurationManager());
        public static ServerConfigurationManager ConfigurationManager { get; set; } = instance.Value;
        private string ServerSettingFile { get; } = "appsettings_LogServer.json";

        public ServerConfiguration ServerConfiguration { get; }

        public ServerConfigurationManager()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                string data = File.ReadAllText(ServerSettingFile);
                ServerConfiguration = JsonSerializer.Deserialize<ServerConfiguration>(data, options)!;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error loading configuration file: {e.Message}");
                ServerConfiguration = new();
            }
        }

        public void Save()
        {
            try
            {
                // serialize JSON directly to a file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                string jsonString = JsonSerializer.Serialize(ServerConfiguration, options);
                File.WriteAllText(ServerSettingFile, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save file {ServerSettingFile}: {ex}");
            }
        }
    }
}