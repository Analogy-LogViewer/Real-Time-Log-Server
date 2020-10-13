using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Analogy.LogServer.Configurator
{
    public class ServerConfigurationManager
    {
        private static readonly Lazy<ServerConfigurationManager> _instance =
            new Lazy<ServerConfigurationManager>(() => new ServerConfigurationManager());
        public static ServerConfigurationManager ConfigurationManager { get; set; } = _instance.Value;
        private string ServerSettingFile { get; } = "appsettings_LogServer.json";

        public ServiceConfiguration ServiceConfiguration { get; }

        public ServerConfigurationManager()
        {

            try
            {
                string data = File.ReadAllText(ServerSettingFile);
                ServiceConfiguration = JsonConvert.DeserializeObject<ServiceConfiguration>(data);
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
                File.WriteAllText(ServerSettingFile, JsonConvert.SerializeObject(ServiceConfiguration));
            }
            catch (Exception ex)
            {
                MessageBox.Show( $"Unable to save file {ServerSettingFile}: {ex}");

            }
        }
    }
}
