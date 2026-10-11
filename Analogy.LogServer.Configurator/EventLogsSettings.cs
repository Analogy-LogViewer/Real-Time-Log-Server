#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Analogy.LogServer.Configurator
{
    public partial class EventLogsSettings : UserControl
    {
        private ServerConfiguration? Configuration { get; }

        public EventLogsSettings()
        {
            InitializeComponent();
        }
        public EventLogsSettings(ServerConfiguration configuration) : this()
        {
            Configuration = configuration;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> selected = Enumerable.Cast<string>(lstAvailable.SelectedItems).ToList();
            lstSelected.Items.AddRange(selected.ToArray());
            foreach (var log in selected)
            {
                lstAvailable.Items.Remove(log);
            }
            UpdateUserSettingList();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<string> selected = Enumerable.Cast<string>(lstSelected.SelectedItems).ToList();
            lstAvailable.Items.AddRange(selected.ToArray());
            foreach (var log in selected)
            {
                lstSelected.Items.Remove(log);
            }
            UpdateUserSettingList();
        }
        private void UpdateUserSettingList()
        {
            Configuration?.ServiceConfiguration.WindowsEventLogsConfiguration.LogsToMonitor = Enumerable.Cast<string>(lstSelected.Items).ToList();
        }

        private void EventLogsSettings_Load(object sender, EventArgs e)
        {
            lstSelected.Items.AddRange(Configuration?.ServiceConfiguration.WindowsEventLogsConfiguration.LogsToMonitor.ToArray());
            try
            {
                var all = System.Diagnostics.Eventing.Reader.EventLogSession.GlobalSession.GetLogNames().Where(EventLog.Exists).ToList().Except(Configuration?.ServiceConfiguration.WindowsEventLogsConfiguration.LogsToMonitor, StringComparer.Ordinal).ToArray();
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