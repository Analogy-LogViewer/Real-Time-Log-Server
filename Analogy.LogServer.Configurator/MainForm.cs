using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analogy.LogServer.Configurator
{
    public partial class MainForm : Form
    {
        private static ServerConfiguration ServiceConfiguration => ServerConfigurationManager.ConfigurationManager.ServerConfiguration;
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
            if (DesignMode)
            {
                return;
            }

            EventLogsSettings els = new EventLogsSettings(MainForm.ServiceConfiguration);
            gbWindowsEventsLog.Controls.Add(els);
            els.Dock = DockStyle.Fill;
            LoadSettings();
        }

        private void LoadSettings()
        {
            chLogToFile.Checked = MainForm.ServiceConfiguration.ServiceConfiguration.LogAlsoToLogFile;
            txtbLogFileLocation.Text = MainForm.ServiceConfiguration.Serilog.WriteTo[1].Args.PathFormat;
            nudHoursToKeepHistory.Value = MainForm.ServiceConfiguration.ServiceConfiguration.HoursToKeepHistory;
            nudMemoryUsageInMB.Value = MainForm.ServiceConfiguration.ServiceConfiguration.MemoryUsageInMB;
            nudCleanUpIntervalMinutes.Value = MainForm.ServiceConfiguration.ServiceConfiguration.CleanUpIntervalMinutes;
            chkbEnableWindowsEventLogs.Checked = MainForm.ServiceConfiguration.ServiceConfiguration.WindowsEventLogsConfiguration.EnableMonitoring;
        }

        private void SaveSettings()
        {
            MainForm.ServiceConfiguration.ServiceConfiguration.LogAlsoToLogFile = chLogToFile.Checked;
            MainForm.ServiceConfiguration.Serilog.WriteTo[1].Args.PathFormat = txtbLogFileLocation.Text;
            MainForm.ServiceConfiguration.ServiceConfiguration.HoursToKeepHistory = (int)nudHoursToKeepHistory.Value;
            MainForm.ServiceConfiguration.ServiceConfiguration.MemoryUsageInMB = (int)nudMemoryUsageInMB.Value;
            MainForm.ServiceConfiguration.ServiceConfiguration.CleanUpIntervalMinutes = (int)nudCleanUpIntervalMinutes.Value;
            MainForm.ServiceConfiguration.ServiceConfiguration.WindowsEventLogsConfiguration.EnableMonitoring = chkbEnableWindowsEventLogs.Checked;
        }
    }
}