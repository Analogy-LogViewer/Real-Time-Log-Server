
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            this.gbLogging.SuspendLayout();
            this.gbCleanup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbCleanup);
            this.panel1.Controls.Add(this.gbLogging);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 191);
            this.panel1.TabIndex = 0;
            // 
            // PanelWindowsEventLogs
            // 
            this.PanelWindowsEventLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelWindowsEventLogs.Location = new System.Drawing.Point(0, 191);
            this.PanelWindowsEventLogs.Name = "PanelWindowsEventLogs";
            this.PanelWindowsEventLogs.Size = new System.Drawing.Size(800, 297);
            this.PanelWindowsEventLogs.TabIndex = 1;
            // 
            // chLogToFile
            // 
            this.chLogToFile.AutoSize = true;
            this.chLogToFile.Location = new System.Drawing.Point(15, 21);
            this.chLogToFile.Name = "chLogToFile";
            this.chLogToFile.Size = new System.Drawing.Size(213, 21);
            this.chLogToFile.TabIndex = 0;
            this.chLogToFile.Text = "Log Messages To Serilog file";
            this.chLogToFile.UseVisualStyleBackColor = true;
            // 
            // gbLogging
            // 
            this.gbLogging.Controls.Add(this.txtbLogFileLocation);
            this.gbLogging.Controls.Add(this.chLogToFile);
            this.gbLogging.Location = new System.Drawing.Point(12, 12);
            this.gbLogging.Name = "gbLogging";
            this.gbLogging.Size = new System.Drawing.Size(474, 97);
            this.gbLogging.TabIndex = 1;
            this.gbLogging.TabStop = false;
            this.gbLogging.Text = "Logging Settings";
            // 
            // txtbLogFileLocation
            // 
            this.txtbLogFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbLogFileLocation.Location = new System.Drawing.Point(15, 58);
            this.txtbLogFileLocation.Name = "txtbLogFileLocation";
            this.txtbLogFileLocation.Size = new System.Drawing.Size(454, 22);
            this.txtbLogFileLocation.TabIndex = 1;
            // 
            // gbCleanup
            // 
            this.gbCleanup.Controls.Add(this.numericUpDown1);
            this.gbCleanup.Location = new System.Drawing.Point(492, 12);
            this.gbCleanup.Name = "gbCleanup";
            this.gbCleanup.Size = new System.Drawing.Size(297, 96);
            this.gbCleanup.TabIndex = 2;
            this.gbCleanup.TabStop = false;
            this.gbCleanup.Text = "Cleanup Settings";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(202, 50);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(80, 22);
            this.numericUpDown1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 488);
            this.Controls.Add(this.PanelWindowsEventLogs);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Analogy Log Configurator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.gbLogging.ResumeLayout(false);
            this.gbLogging.PerformLayout();
            this.gbCleanup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PanelWindowsEventLogs;
        private System.Windows.Forms.CheckBox chLogToFile;
        private System.Windows.Forms.GroupBox gbLogging;
        private System.Windows.Forms.TextBox txtbLogFileLocation;
        private System.Windows.Forms.GroupBox gbCleanup;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}

