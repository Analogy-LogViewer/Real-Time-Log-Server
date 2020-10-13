using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Analogy.LogViewer.WindowsEventLogs;
using Newtonsoft.Json.Linq;

namespace Analogy.LogServer.Configurator
{
    public partial class MainForm : Form
    {
        private ServerConfiguration ServiceConfiguration => ServerConfigurationManager.ConfigurationManager.ServerConfiguration;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            ServerConfigurationManager.ConfigurationManager.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            EventLogsSettings els=new EventLogsSettings(ServiceConfiguration);
            gbWindowsEventsLog.Controls.Add(els);
            els.Dock = DockStyle.Fill;
            LoadSettings();
        }

        private void LoadSettings()
        {
            chLogToFile.Checked = ServiceConfiguration.ServiceConfiguration.LogAlsoToLogFile;
            txtbLogFileLocation.Text = ServiceConfiguration.Serilog.WriteTo[1].Args.pathFormat;
            nudHoursToKeepHistory.Value = ServiceConfiguration.ServiceConfiguration.HoursToKeepHistory;
            nudMemoryUsageInMB.Value = ServiceConfiguration.ServiceConfiguration.MemoryUsageInMB;
            nudCleanUpIntervalMinutes.Value = ServiceConfiguration.ServiceConfiguration.CleanUpIntervalMinutes;
            chkbEnableWindowsEventLogs.Checked = ServiceConfiguration.ServiceConfiguration.WindowsEventLogsConfiguration.EnableMonitoring;
        }

        private void SaveSettings()
        {
            ServiceConfiguration.ServiceConfiguration.LogAlsoToLogFile = chLogToFile.Checked;
            ServiceConfiguration.Serilog.WriteTo[1].Args.pathFormat=txtbLogFileLocation.Text;
            ServiceConfiguration.ServiceConfiguration.HoursToKeepHistory =(int)nudHoursToKeepHistory.Value ;
            ServiceConfiguration.ServiceConfiguration.MemoryUsageInMB =(int)nudMemoryUsageInMB.Value;
            ServiceConfiguration.ServiceConfiguration.CleanUpIntervalMinutes =(int)nudCleanUpIntervalMinutes.Value;
            ServiceConfiguration.ServiceConfiguration.WindowsEventLogsConfiguration.EnableMonitoring=chkbEnableWindowsEventLogs.Checked;
        }
    }
}
