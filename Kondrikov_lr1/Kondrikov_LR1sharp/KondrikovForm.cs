using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Kondrikov_LR1sharp
{
    public partial class KondrikovForm : Form
    {
        private Process ChildProcess = null;
        System.Threading.EventWaitHandle StopEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "StopEvent");
        System.Threading.EventWaitHandle StartEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "StartEvent");
        System.Threading.EventWaitHandle ConfirmEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ConfirmEvent");
        // private int ThreadCount = 0;
        // private bool ConsoleClose = false;

        public KondrikovForm()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (ChildProcess == null)
            {
                ChildProcess = Process.Start("Kondrikov_LR1plus.exe");
                this.Timer.Start();
                this.ThreadListBox.Items.Add("Main thread");
                this.ThreadListBox.Items.Add("All threads");
                for (int i = 0; i < this.NumericUpDown.Value - 1; ++i)
                {
                    StartEvent.Set();
                    ConfirmEvent.WaitOne();
                    this.ThreadListBox.Items.Add($"thread {this.ThreadListBox.Items.Count - 2}");
                }
            }
            else if (ChildProcess.HasExited)
            {
                ChildProcess = Process.Start("Kondrikov_LR1plus.exe");
                this.Timer.Start();
                this.ThreadListBox.Items.Add("Main thread");
                this.ThreadListBox.Items.Add("All threads");
                for (int i = 0; i < this.NumericUpDown.Value - 1; ++i)
                {
                    StartEvent.Set();
                    ConfirmEvent.WaitOne();
                    this.ThreadListBox.Items.Add($"thread {this.ThreadListBox.Items.Count - 2}");
                }
            }
            else
            {
                for (int i = 0; i < this.NumericUpDown.Value; ++i)
                {
                    StartEvent.Set();
                    ConfirmEvent.WaitOne();
                    this.ThreadListBox.Items.Add($"thread {this.ThreadListBox.Items.Count - 2}");
                }
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (ChildProcess != null && !ChildProcess.HasExited)
            {
                StopEvent.Set();
                ConfirmEvent.WaitOne();
                this.ThreadListBox.Items.RemoveAt(this.ThreadListBox.Items.Count - 1);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            if (this.ChildProcess.HasExited && this.ThreadListBox.Items.Count != 0)
            {
                this.Timer.Stop();
                // this.ThreadCount = this.ThreadListBox.Items.Count - 1;
                this.ThreadListBox.Items.Clear();
                // this.ConsoleClose = true;
            }
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (ChildProcess != null && !ChildProcess.HasExited)
            {
                for (int i = 0; i < this.ThreadListBox.Items.Count - 2; ++i)
                {
                    StopEvent.Set();
                    ConfirmEvent.WaitOne();
                }
                StopEvent.Set();
                ConfirmEvent.WaitOne();
                this.ThreadListBox.Items.Clear();
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {

        }

        private void KondrikovForm_Load(object sender, EventArgs e)
        {

        }
    }
}
