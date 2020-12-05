namespace Analogy.LogServer.Tests
{
    partial class TestForm
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
            this.btnProducer = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnConsumer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnProducer
            // 
            this.btnProducer.Location = new System.Drawing.Point(12, 22);
            this.btnProducer.Name = "btnProducer";
            this.btnProducer.Size = new System.Drawing.Size(166, 27);
            this.btnProducer.TabIndex = 0;
            this.btnProducer.Text = "Produce Messages";
            this.btnProducer.UseVisualStyleBackColor = true;
            this.btnProducer.Click += new System.EventHandler(this.btnProducer_Click);
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(199, 22);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(368, 27);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "localhost:7000";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(199, 65);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(373, 333);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // btnConsumer
            // 
            this.btnConsumer.Location = new System.Drawing.Point(12, 65);
            this.btnConsumer.Name = "btnConsumer";
            this.btnConsumer.Size = new System.Drawing.Size(166, 27);
            this.btnConsumer.TabIndex = 3;
            this.btnConsumer.Text = "Consume Messages";
            this.btnConsumer.UseVisualStyleBackColor = true;
            this.btnConsumer.Click += new System.EventHandler(this.btnConsumer_Click_1);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 407);
            this.Controls.Add(this.btnConsumer);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.btnProducer);
            this.Name = "TestForm";
            this.Text = "Analogy Log Server Message producer Example";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnProducer;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnConsumer;
    }
}

