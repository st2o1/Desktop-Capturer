using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Desktop_Capture
{
    public partial class Form1 : Form
    {
        private int selectedIndex = 0;
        private Timer screenshotTimer = new Timer();
        private bool isSavingScreenshots = false;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;


            foreach (Screen screen in Screen.AllScreens)
            {
                comboBox1.Items.Add(screen.DeviceName);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            timer1.Interval = 33; // it is set to about 30 frames per seconds change it if you wan't
            timer1.Tick += timer1_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
                button1.Text = "Start";
            }
            else
            {
                timer1.Start();
                button1.Text = "Stop";
            }
        }


        private Bitmap screenshot = null;
        private Bitmap CaptureScreen(Screen screen)
        {
            if (screenshot == null || screenshot.Size != screen.Bounds.Size)
            {
                screenshot?.Dispose();
                screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
            }

            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(screen.Bounds.Location, Point.Empty, screen.Bounds.Size);
                Cursors.Default.Draw(graphics, new Rectangle(Cursor.Position, Cursors.Default.Size));
            }

            return screenshot;
        }


        private IntPtr GetCursorHandle()
        {
            CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            GetCursorInfo(out pci);

            return pci.hCursor;
        }


        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < Screen.AllScreens.Length)
            {
                Screen selectedScreen = Screen.AllScreens[selectedIndex];
                Bitmap screenshot = CaptureScreen(selectedScreen);
                pictureBox1.Image = screenshot;
            }
        }


        private void SaveScreenshot(Bitmap screenshot)
        {
            try
            {
                string folderPath = Path.Combine(Application.StartupPath, "images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(folderPath, fileName);
                screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving screenshot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Image != null)
                {
                    string folderPath = Path.Combine(Application.StartupPath, "images");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    string filePath = Path.Combine(folderPath, fileName);
                    pictureBox1.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    MessageBox.Show("No image in pictureBox1 to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving screenshot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
