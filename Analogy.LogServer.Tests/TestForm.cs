using Analogy.Interfaces;
using Analogy.LogServer.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analogy.LogServer.Tests
{
    public partial class TestForm : Form
    {
        private bool producing;
        private bool consuming;
        public TestForm()
        {
            InitializeComponent();
        }

        private async void btnProducer_Click(object sender, EventArgs e)
        {
            if (producing)
            {
                return;
            }

            producing = true;
            btnProducer.Enabled = false;
            var p = new AnalogyMessageProducer($"http://{txtIP.Text}");
            var ai = new Dictionary<string, string> { { "some key", "some value" } };
            for (int i = 0; i < 100000; i++)
            {
                await p.Log(text: "test " + i, source: "none", additionalInformation: ai, level: AnalogyLogLevel.Information).ConfigureAwait(false);
                await Task.Delay(500).ConfigureAwait(false);
            }

            producing = false;
            btnProducer.Enabled = true;
        }


        private async void btnConsumer_Click_1(object sender, EventArgs e)
        {
            if (consuming)
            {
                return;
            }

            consuming = true;
            btnConsumer.Enabled = false;

            var c = new AnalogyMessageConsumer($"http://{txtIP.Text}");
            await foreach (var m in c.GetMessages().ConfigureAwait(false))
            {
                richTextBox1.Text += Environment.NewLine + m;
            }

            consuming = false;
            btnConsumer.Enabled = true;
        }
    }
}
