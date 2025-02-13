using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace SytemCleaner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            List<int> ramlist = new List<int>();
            List<string> processlist = new List<string>();
            foreach (Process process in Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero).ToArray())
            {
                if (process.ProcessName != "explorer" && process.ProcessName != "devenv")
                {
                    list.Add(process.ProcessName.ToString() + " :" + process.Id.ToString() + " :" + (process.PrivateMemorySize64 / 1024 / 1024 + "MB").ToString());
                    int ram = ((int)(process.PrivateMemorySize64 / 1024 / 1024));
                    processlist.Add(process.ProcessName.ToString());
                    ramlist.Add(ram);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                list.Add(ramlist.Max().ToString() + "MB");
                ramlist.Remove(ramlist.Max());
            }
            DisableAllStartupEntries();
            foreach (string ProcName in processlist)
            {
                Process? process = Process.GetProcessesByName(ProcName).FirstOrDefault();
                process.Kill();
            }
            Process.GetCurrentProcess().Kill();

        }
        private void DisableAllStartupEntries()
        {
            DisableStartupEntries(Registry.CurrentUser);
            DisableStartupEntries(Registry.LocalMachine);
            MessageBox.Show("Автозагрузка отключена!");
        }

        private void DisableStartupEntries(RegistryKey root)
        {
            string path = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
            using (var key = root.OpenSubKey(path, true))
            {
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        // Установим значение, чтобы оно не запускалось сразу (например, добавив "_disabled")
                        if (name != "jusched")
                        {
                            key.DeleteValue(name);
                        }
                    }
                }
            }
        }
    }
}
