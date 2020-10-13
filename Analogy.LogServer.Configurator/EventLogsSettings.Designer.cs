namespace Analogy.LogViewer.WindowsEventLogs
{
    partial class EventLogsSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl = new System.Windows.Forms.Label();
            this.spltColumns = new System.Windows.Forms.SplitContainer();
            this.lstSelected = new System.Windows.Forms.ListBox();
            this.lstAvailable = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.spltColumns)).BeginInit();
            this.spltColumns.Panel1.SuspendLayout();
            this.spltColumns.Panel2.SuspendLayout();
            this.spltColumns.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl
            // 
            this.lbl.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl.Location = new System.Drawing.Point(0, 0);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(693, 34);
            this.lbl.TabIndex = 13;
            this.lbl.Text = "Select logs from the right list or remove selected log from the left list:";
            // 
            // spltColumns
            // 
            this.spltColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltColumns.Location = new System.Drawing.Point(0, 34);
            this.spltColumns.Name = "spltColumns";
            // 
            // spltColumns.Panel1
            // 
            this.spltColumns.Panel1.Controls.Add(this.lstSelected);
            this.spltColumns.Panel1.Controls.Add(this.btnRemove);
            this.spltColumns.Panel1.Text = "Panel1";
            // 
            // spltColumns.Panel2
            // 
            this.spltColumns.Panel2.Controls.Add(this.lstAvailable);
            this.spltColumns.Panel2.Controls.Add(this.btnAdd);
            this.spltColumns.Panel2.Text = "Panel2";
            this.spltColumns.Size = new System.Drawing.Size(693, 359);
            this.spltColumns.SplitterDistance = 324;
            this.spltColumns.TabIndex = 14;
            // 
            // lstSelected
            // 
            this.lstSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSelected.ItemHeight = 18;
            this.lstSelected.Location = new System.Drawing.Point(0, 47);
            this.lstSelected.Name = "lstSelected";
            this.lstSelected.Size = new System.Drawing.Size(324, 312);
            this.lstSelected.TabIndex = 0;
            // 
            // lstAvailable
            // 
            this.lstAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAvailable.ItemHeight = 18;
            this.lstAvailable.Location = new System.Drawing.Point(0, 47);
            this.lstAvailable.Name = "lstAvailable";
            this.lstAvailable.Size = new System.Drawing.Size(365, 312);
            this.lstAvailable.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAdd.Location = new System.Drawing.Point(0, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(365, 47);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "<-- Add to selected logs";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRemove.Location = new System.Drawing.Point(0, 0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(324, 47);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.Text = "remove from selected logs ->";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // EventLogsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spltColumns);
            this.Controls.Add(this.lbl);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "EventLogsSettings";
            this.Size = new System.Drawing.Size(693, 393);
            this.Load += new System.EventHandler(this.EventLogsSettings_Load);
            this.spltColumns.Panel1.ResumeLayout(false);
            this.spltColumns.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltColumns)).EndInit();
            this.spltColumns.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.SplitContainer spltColumns;
        private System.Windows.Forms.ListBox lstSelected;
        private System.Windows.Forms.ListBox lstAvailable;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
    }
}
