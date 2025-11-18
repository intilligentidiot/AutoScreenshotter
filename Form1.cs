using System.Drawing.Imaging;
using System.IO;

namespace AutoScreenshotter
{
    public partial class Form1 : Form
    {
        private bool isRunning = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                // --- STOP THE APP ---
                screenshotTimer.Stop();
                isRunning = false;

                // Update UI
                lblStatus.Text = "Status: Stopped";
                btnStartStop.Text = "Start";
                btnStartStop.BackColor = Color.LightGreen;

                // Re-enable the settings
                groupBox1.Enabled = true;
            }
            else
            {
                // --- START THE APP ---

                // 1. Validate settings first
                if (string.IsNullOrWhiteSpace(txtLocalPath.Text) && string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("You must enter at least one save path (Local or Network).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Set the timer's interval from the NumericUpDown
                // The value is in seconds, so we multiply by 1000 to get milliseconds
                screenshotTimer.Interval = (int)numInterval.Value * 1000;

                // 3. Start the timer
                screenshotTimer.Start();
                isRunning = true;

                // 4. Update UI
                lblStatus.Text = "Status: Running...";
                btnStartStop.Text = "Stop";
                btnStartStop.BackColor = Color.LightCoral;

                // Disable the settings group so user can't change them while running
                groupBox1.Enabled = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            TakeandSaveScreenshots();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblResDisp.Text = trackBar1.Value.ToString() + " %";
            // Load the saved settings and put them into the text boxes
            txtLocalPath.Text = Properties.Settings.Default.LastLocalPath;

            // Using the name from your designer file for the network path
            textBox1.Text = Properties.Settings.Default.LastNetworkPath;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtLocalPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lblResDisp.Text = trackBar1.Value.ToString() + " %";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save the current text from the boxes into our settings
            Properties.Settings.Default.LastLocalPath = txtLocalPath.Text;
            Properties.Settings.Default.LastNetworkPath = textBox1.Text;
            Properties.Settings.Default.LastNetworkUser = txtNetworkUser.Text;
            Properties.Settings.Default.LastNetworkPass = txtNetworkPass.Text;

            // Tell the settings to save themselves
            Properties.Settings.Default.Save();
        }

        private void btnBrowseLocal_Click(object sender, EventArgs e)
        {
            // Show the folder browser dialog
            DialogResult result = folderBrowserDialog.ShowDialog();

            // Check if the user clicked "OK"
            if (result == DialogResult.OK)
            {
                // Put the selected path into the local path text box
                txtLocalPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnBrowseNetwork_Click(object sender, EventArgs e)
        {
            // Show the folder browser dialog
            DialogResult result = folderBrowserDialog.ShowDialog();

            // Check if the user clicked "OK"
            if (result == DialogResult.OK)
            {
                // Put the selected path into the network path text box
                textBox1.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void screenshotTimer_Tick(object sender, EventArgs e)
        {
            TakeandSaveScreenshots();
        }

        private void TakeandSaveScreenshots()
        {
            try
            {
                // --- 1. GATHER SETTINGS ---
                string localPath = txtLocalPath.Text;
                string networkPath = textBox1.Text;
                string username = Environment.UserName;

                string netUser = txtNetworkUser.Text;
                string netPass = txtNetworkPass.Text;
                bool useCreds = !string.IsNullOrWhiteSpace(netUser) && !string.IsNullOrWhiteSpace(netPass);

                ImageFormat saveFormat = comboBox1.Text == "JPG" ? ImageFormat.Jpeg : ImageFormat.Png;
                string fileExtension = saveFormat == ImageFormat.Jpeg ? ".jpg" : ".png";
                double scale = trackBar1.Value / 100.0;

                // --- 2. LOOP MONITORS ---
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    Screen screen = Screen.AllScreens[i];

                    // Capture & Resize
                    Bitmap originalBitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(originalBitmap))
                    {
                        g.CopyFromScreen(screen.Bounds.Location, Point.Empty, screen.Bounds.Size);
                    }

                    Bitmap finalBitmap;
                    if (scale < 1.0)
                    {
                        int newWidth = (int)(originalBitmap.Width * scale);
                        int newHeight = (int)(originalBitmap.Height * scale);
                        finalBitmap = new Bitmap(newWidth, newHeight);
                        using (Graphics g = Graphics.FromImage(finalBitmap)) { g.DrawImage(originalBitmap, 0, 0, newWidth, newHeight); }
                        originalBitmap.Dispose();
                    }
                    else { finalBitmap = originalBitmap; }

                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string filename = $"d{i + 1}_{timestamp}{fileExtension}";

                    // --- 3. SAVE LOCAL ---
                    if (!string.IsNullOrWhiteSpace(localPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(localPath);
                            finalBitmap.Save(Path.Combine(localPath, filename), saveFormat);
                        }
                        catch { lblStatus.Text = "Status: Local Save Error"; }
                    }

                    // --- 4. SAVE NETWORK (Modern Implementation) ---
                    if (!string.IsNullOrWhiteSpace(networkPath))
                    {
                        try
                        {
                            // Define the "Action" we want to run (The saving logic)
                            Action saveAction = () =>
                            {
                                string fullNetworkPath = Path.Combine(networkPath, username);
                                Directory.CreateDirectory(fullNetworkPath);
                                finalBitmap.Save(Path.Combine(fullNetworkPath, filename), saveFormat);
                            };

                            if (useCreds)
                            {
                                // Run the action AS the network user
                                ImpersonationHelper.ExecuteActionAsUser(netUser, ".", netPass, saveAction);
                            }
                            else
                            {
                                // Run the action AS the normal user
                                saveAction();
                            }

                            lblStatus.Text = "Status: Running...";
                        }
                        catch (Exception)
                        {
                            lblStatus.Text = "Status: Network Error";
                        }
                    }

                    finalBitmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                screenshotTimer.Stop();
                isRunning = false;
                MessageBox.Show($"Fatal Error: {ex.Message}");
            }
        }

        private void btnTogglePass_Click(object sender, EventArgs e)
        {
            if (txtNetworkPass.PasswordChar == '*')
            {
                txtNetworkPass.PasswordChar = '\0';
            }
            else
            {
                txtNetworkPass.PasswordChar = '*';
            }
        }
    }
}
