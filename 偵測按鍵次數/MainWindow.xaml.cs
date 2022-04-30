using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace 偵測按鍵次數
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Hide();
            icon = new NotifyIcon()
            {
                Icon = new Icon(path),
                Visible = true,
            };
            icon.MouseClick += icon_MouseClick;
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("按 ~ 來截圖", null, printScreen_OnClick);
            contextMenuStrip.Items.Add("開啓路徑", null, openCurrentDirectory);
            contextMenuStrip.Items.Add("離開", null, exit);
            icon.ContextMenuStrip = contextMenuStrip;

            KeyDown += mainWindow_KeyDown;
            Closing += mainWindow_Closing;
            KeyboardHook.GlobalKeyDown += KeyboardHook_GlobalKeyDown;
        }

        NotifyIcon icon;
        Dictionary<string, int> list = new Dictionary<string, int>();
        const string path = @"E:\程序\C#\Windows Form\圖片\icon_keyboard.ico";
        bool printScreen;

        private void exit(object sender, EventArgs e) => Environment.Exit(0);

        private void openCurrentDirectory(object sender, EventArgs e) => Process.Start(Environment.CurrentDirectory);

        private void printScreen_OnClick(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            printScreen = item.Checked = !item.Checked;
        }

        private void KeyboardHook_GlobalKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            string s = e.KeyCode.ToString();
            if (list.Keys.Contains(s))
                list[s] += 1;
            else
                list.Add(s, 1);

            if (printScreen && (int)e.KeyCode == 192)
            {
                SendKeys.SendWait("{PRTSC}");
                /**
                using (Bitmap bit = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height))
                using (Graphics g = Graphics.FromImage(bit))
                {
                    g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), bit.Size);
                    string path = $@"{Environment.CurrentDirectory}\ScreenShots";
                    if (!File.Exists(path))
                        System.Windows.MessageBox.Show($"{path}\n\nnot exists");
                    path += $@"\Screenshot  {DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}  {DateTime.Now.Hour}.{DateTime.Now.Minute}.png";
                    bit.Save(path, ImageFormat.Png);
                }
                */
            }
        }

        private void icon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                icon.ContextMenuStrip.Show();
                return;
                /**
                if (System.Windows.Forms.MessageBox.Show("Are you sure to EXIT?", "EXIT", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;
                icon.Visible = false;
                Environment.Exit(0);
                */
            }

            if (e != null && IsVisible)
                Hide();
            else
            {
                Show();
                box.Document.Blocks.Clear();
                var sortList = (from entry in list orderby entry.Value ascending select entry).Reverse();
                string all = "";
                foreach (var i in sortList)
                    all += $"{i.Key}\t\t\t>{i.Value}\n";
                box.Document.Blocks.Add(new Paragraph(new Run(all)));
            }
        }

        private void mainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => icon_MouseClick(null, null);

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
