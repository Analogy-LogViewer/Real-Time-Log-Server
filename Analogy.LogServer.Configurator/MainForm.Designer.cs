
namespace Analogy.LogServer.Configurator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.PanelWindowsEventLogs = new System.Windows.Forms.Panel();
            this.chLogToFile = new System.Windows.Forms.CheckBox();
            this.gbLogging = new System.Windows.Forms.GroupBox();
            this.txtbLogFileLocation = new System.Windows.Forms.TextBox();
            this.gbCleanup = new System.Windows.Forms.GroupBox();
            this.nudHoursToKeepHistory = new System.Windows.Forms.NumericUpDown();
            this.lblHoursToKeepHistory = new System.Windows.Forms.Label();
            this.lblCleanUpIntervalMinutes = new System.Windows.Forms.Label();
            this.nudCleanUpIntervalMinutes = new System.Windows.Forms.NumericUpDown();
            this.lblMemoryUsageInMB = new System.Windows.Forms.Label();
            this.nudMemoryUsageInMB = new System.Windows.Forms.NumericUpDown();
            this.gbWindowsEventsLog = new System.Windows.Forms.GroupBox();
            this.chkbEnableWindowsEventLogs = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.PanelWindowsEventLogs.SuspendLayout();
            this.gbLogging.SuspendLayout();
            this.gbCleanup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoursToKeepHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCleanUpIntervalMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMemoryUsageInMB)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbCleanup);
            this.panel1.Controls.Add(this.gbLogging);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(785, 170);
            this.panel1.TabIndex = 0;
            // 
            // PanelWindowsEventLogs
            // 
            this.PanelWindowsEventLogs.Controls.Add(this.chkbEnableWindowsEventLogs);
            this.PanelWindowsEventLogs.Controls.Add(this.gbWindowsEventsLog);
            this.PanelWindowsEventLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelWindowsEventLogs.Location = new System.Drawing.Point(0, 170);
            this.PanelWindowsEventLogs.Name = "PanelWindowsEventLogs";
            this.PanelWindowsEventLogs.Size = new System.Drawing.Size(785, 318);
            this.PanelWindowsEventLogs.TabIndex = 1;
            // 
            // chLogToFile
            // 
            this.chLogToFile.AutoSize = true;
            this.chLogToFile.Location = new System.Drawing.Point(15, 21);
            this.chLogToFile.Name = "chLogToFile";
            this.chLogToFile.Size = new System.Drawing.Size(388, 21);
            this.chLogToFile.TabIndex = 0;
            this.chLogToFile.Text = "Log Messages To Serilog file (beside real time streaming";
            this.chLogToFile.UseVisualStyleBackColor = true;
            // 
            // gbLogging
            // 
            this.gbLogging.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLogging.Controls.Add(this.txtbLogFileLocation);
            this.gbLogging.Controls.Add(this.chLogToFile);
            this.gbLogging.Location = new System.Drawing.Point(12, 12);
            this.gbLogging.Name = "gbLogging";
            this.gbLogging.Size = new System.Drawing.Size(463, 151);
            this.gbLogging.TabIndex = 1;
            this.gbLogging.TabStop = false;
            this.gbLogging.Text = "Logging Settings";
            // 
            // txtbLogFileLocation
            // 
            this.txtbLogFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbLogFileLocation.Location = new System.Drawing.Point(15, 55);
            this.txtbLogFileLocation.Name = "txtbLogFileLocation";
            this.txtbLogFileLocation.Size = new System.Drawing.Size(443, 22);
            this.txtbLogFileLocation.TabIndex = 1;
            // 
            // gbCleanup
            // 
            this.gbCleanup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCleanup.Controls.Add(this.lblMemoryUsageInMB);
            this.gbCleanup.Controls.Add(this.nudMemoryUsageInMB);
            this.gbCleanup.Controls.Add(this.lblCleanUpIntervalMinutes);
            this.gbCleanup.Controls.Add(this.nudCleanUpIntervalMinutes);
            this.gbCleanup.Controls.Add(this.lblHoursToKeepHistory);
            this.gbCleanup.Controls.Add(this.nudHoursToKeepHistory);
            this.gbCleanup.Location = new System.Drawing.Point(481, 12);
            this.gbCleanup.Name = "gbCleanup";
            this.gbCleanup.Size = new System.Drawing.Size(297, 151);
            this.gbCleanup.TabIndex = 2;
            this.gbCleanup.TabStop = false;
            this.gbCleanup.Text = "Cleanup Settings";
            // 
            // nudHoursToKeepHistory
            // 
            this.nudHoursToKeepHistory.Location = new System.Drawing.Point(211, 25);
            this.nudHoursToKeepHistory.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudHoursToKeepHistory.Name = "nudHoursToKeepHistory";
            this.nudHoursToKeepHistory.Size = new System.Drawing.Size(80, 22);
            this.nudHoursToKeepHistory.TabIndex = 0;
            this.nudHoursToKeepHistory.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblHoursToKeepHistory
            // 
            this.lblHoursToKeepHistory.AutoSize = true;
            this.lblHoursToKeepHistory.Location = new System.Drawing.Point(6, 27);
            this.lblHoursToKeepHistory.Name = "lblHoursToKeepHistory";
            this.lblHoursToKeepHistory.Size = new System.Drawing.Size(156, 17);
            this.lblHoursToKeepHistory.TabIndex = 1;
            this.lblHoursToKeepHistory.Text = "Hours To Keep History:";
            // 
            // lblCleanUpIntervalMinutes
            // 
            this.lblCleanUpIntervalMinutes.AutoSize = true;
            this.lblCleanUpIntervalMinutes.Location = new System.Drawing.Point(6, 55);
            this.lblCleanUpIntervalMinutes.Name = "lblCleanUpIntervalMinutes";
            this.lblCleanUpIntervalMinutes.Size = new System.Drawing.Size(185, 17);
            this.lblCleanUpIntervalMinutes.TabIndex = 3;
            this.lblCleanUpIntervalMinutes.Text = "Run cleanup Every minutes:";
            // 
            // nudCleanUpIntervalMinutes
            // 
            this.nudCleanUpIntervalMinutes.Location = new System.Drawing.Point(211, 53);
            this.nudCleanUpIntervalMinutes.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudCleanUpIntervalMinutes.Name = "nudCleanUpIntervalMinutes";
            this.nudCleanUpIntervalMinutes.Size = new System.Drawing.Size(80, 22);
            this.nudCleanUpIntervalMinutes.TabIndex = 2;
            this.nudCleanUpIntervalMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblMemoryUsageInMB
            // 
            this.lblMemoryUsageInMB.AutoSize = true;
            this.lblMemoryUsageInMB.Location = new System.Drawing.Point(6, 83);
            this.lblMemoryUsageInMB.Name = "lblMemoryUsageInMB";
            this.lblMemoryUsageInMB.Size = new System.Drawing.Size(168, 17);
            this.lblMemoryUsageInMB.TabIndex = 5;
            this.lblMemoryUsageInMB.Text = "ProcessMemory limit [Mb]";
            // 
            // nudMemoryUsageInMB
            // 
            this.nudMemoryUsageInMB.Location = new System.Drawing.Point(211, 81);
            this.nudMemoryUsageInMB.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudMemoryUsageInMB.Name = "nudMemoryUsageInMB";
            this.nudMemoryUsageInMB.Size = new System.Drawing.Size(80, 22);
            this.nudMemoryUsageInMB.TabIndex = 4;
            this.nudMemoryUsageInMB.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbWindowsEventsLog
            // 
            this.gbWindowsEventsLog.Location = new System.Drawing.Point(12, 39);
            this.gbWindowsEventsLog.Name = "gbWindowsEventsLog";
            this.gbWindowsEventsLog.Size = new System.Drawing.Size(764, 271);
            this.gbWindowsEventsLog.TabIndex = 0;
            this.gbWindowsEventsLog.TabStop = false;
            this.gbWindowsEventsLog.Text = "Windows Event logs";
            // 
            // chkbEnableWindowsEventLogs
            // 
            this.chkbEnableWindowsEventLogs.AutoSize = true;
            this.chkbEnableWindowsEventLogs.Location = new System.Drawing.Point(12, 6);
            this.chkbEnableWindowsEventLogs.Name = "chkbEnableWindowsEventLogs";
            this.chkbEnableWindowsEventLogs.Size = new System.Drawing.Size(388, 21);
            this.chkbEnableWindowsEventLogs.TabIndex = 1;
            this.chkbEnableWindowsEventLogs.Text = "Log Messages To Serilog file (beside real time streaming";
            this.chkbEnableWindowsEventLogs.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 488);
            this.Controls.Add(this.PanelWindowsEventLogs);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Analogy Log Configurator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.PanelWindowsEventLogs.ResumeLayout(false);
            this.PanelWindowsEventLogs.PerformLayout();
            this.gbLogging.ResumeLayout(false);
            this.gbLogging.PerformLayout();
            this.gbCleanup.ResumeLayout(false);
            this.gbCleanup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoursToKeepHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCleanUpIntervalMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMemoryUsageInMB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PanelWindowsEventLogs;
        private System.Windows.Forms.CheckBox chLogToFile;
        private System.Windows.Forms.GroupBox gbLogging;
        private System.Windows.Forms.TextBox txtbLogFileLocation;
        private System.Windows.Forms.GroupBox gbCleanup;
        private System.Windows.Forms.NumericUpDown nudHoursToKeepHistory;
        private System.Windows.Forms.Label lblHoursToKeepHistory;
        private System.Windows.Forms.Label lblCleanUpIntervalMinutes;
        private System.Windows.Forms.NumericUpDown nudCleanUpIntervalMinutes;
        private System.Windows.Forms.Label lblMemoryUsageInMB;
        private System.Windows.Forms.NumericUpDown nudMemoryUsageInMB;
        private System.Windows.Forms.GroupBox gbWindowsEventsLog;
        private System.Windows.Forms.CheckBox chkbEnableWindowsEventLogs;
    }
}

