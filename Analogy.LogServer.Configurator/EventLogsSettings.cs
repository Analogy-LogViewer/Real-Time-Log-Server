using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Analogy.LogServer.Configurator;

namespace Analogy.LogViewer.WindowsEventLogs
{
    public partial class EventLogsSettings : UserControl
    {
        private ServerConfiguration Configuration { get; }

        public EventLogsSettings()
        {
            InitializeComponent();
        }
         public EventLogsSettings(ServerConfiguration configuration):this()
         {
             Configuration = configuration;
         }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> selected = lstAvailable.SelectedItems.Cast<string>().ToList();
            lstSelected.Items.AddRange(selected.ToArray());
            foreach (var log in selected)
            {
                lstAvailable.Items.Remove(log);
            }
            UpdateUserSettingList();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<string> selected = lstSelected.SelectedItems.Cast<string>().ToList();
            lstAvailable.Items.AddRange(selected.ToArray());
            foreach (var log in selected)
            {
                lstSelected.Items.Remove(log);
            }
            UpdateUserSettingList();
        }
        private void UpdateUserSettingList()
        {
            Configuration.ServiceConfiguration.WindowsEventLogsConfiguration.LogsToMonitor= lstSelected.Items.Cast<string>().ToList();
        }

        private void EventLogsSettings_Load(object sender, EventArgs e)
        {
            lstSelected.Items.AddRange(Configuration.ServiceConfiguration.WindowsEventLogsConfiguration.LogsToMonitor.ToArray());
            try
            {
                var all = System.Diagnostics.Eventing.Reader.EventLogSession.GlobalSession.GetLogNames().Where(EventLog.Exists).ToList().Except(Configuration.ServiceConfiguration.WindowsEventLogsConfiguration.LogsToMonitor).ToArray();
                lstAvailable.Items.AddRange(all);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error loading all logs. Make sure you are running as administrator. Error:" + exception.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
