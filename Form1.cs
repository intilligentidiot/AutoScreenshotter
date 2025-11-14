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
            try
            {
                // 1. GET THE SAVE PATH
                // Get the path from the text box.
                string localSavePath = txtLocalPath.Text;

                // 2. CHECK IF THE PATH IS VALID
                // If the user left the local path box empty, show an error.
                if (string.IsNullOrWhiteSpace(localSavePath))
                {
                    MessageBox.Show("Please enter a valid local save path first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Stop the function here
                }

                // 3. CREATE THE DIRECTORY (if it doesn't exist)
                // This is a nice, safe way to make sure the folder is ready.
                Directory.CreateDirectory(localSavePath);

                // 4. LOOP THROUGH ALL MONITORS
                // Screen.AllScreens gives us an array of all connected displays.
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    // Get the current screen
                    Screen screen = Screen.AllScreens[i];

                    // 5. CREATE A BITMAP (IMAGE)
                    // Create a blank image in memory that is the exact size of the screen
                    Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);

                    // 6. COPY THE SCREEN TO THE BITMAP
                    // Create a graphics object to draw on our blank image
                    using (Graphics g = Graphics.FromImage(screenshot))
                    {
                        // Copy the screen data from the screen's top-left corner (Bounds.Location)
                        // into our image's top-left corner (0, 0)
                        g.CopyFromScreen(screen.Bounds.Location, Point.Empty, screen.Bounds.Size);
                    }

                    // 7. CREATE A UNIQUE FILENAME
                    // Format: d1_20251114_173005.png
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string filename = $"d{i + 1}_{timestamp}.png"; // We add +1 because the array starts at 0

                    // 8. SAVE THE FILE
                    // Combine the path and filename
                    string fullPath = Path.Combine(localSavePath, filename);
                    screenshot.Save(fullPath, ImageFormat.Png);

                    // Clean up the image from memory
                    screenshot.Dispose();
                }

                // 9. SHOW A SUCCESS MESSAGE
                MessageBox.Show($"Successfully saved {Screen.AllScreens.Length} screenshots to:\n{localSavePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // If anything goes wrong, show a detailed error
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            try
            {
                // --- 1. GET ALL SETTINGS FROM THE GUI ---
                string localPath = txtLocalPath.Text;
                string networkPath = textBox1.Text;
                string username = Environment.UserName;

                // Get format from ComboBox (default to PNG)
                ImageFormat saveFormat = ImageFormat.Png;
                if (comboBox1.Text == "JPG")
                {
                    saveFormat = ImageFormat.Jpeg;
                }
                string fileExtension = saveFormat == ImageFormat.Jpeg ? ".jpg" : ".png";

                // Get resolution scale from TrackBar (default to 100%)
                // We divide by 100.0 to get a double (e.g., 0.5 for 50%)
                double scale = trackBar1.Value / 100.0;

                // --- 2. LOOP THROUGH ALL MONITORS ---
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    Screen screen = Screen.AllScreens[i];

                    // --- 3. CAPTURE THE SCREEN ---
                    // Create a full-sized bitmap
                    Bitmap originalBitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(originalBitmap))
                    {
                        g.CopyFromScreen(screen.Bounds.Location, Point.Empty, screen.Bounds.Size);
                    }

                    // --- 4. RESIZE THE IMAGE (if needed) ---
                    // We'll use this variable to hold our final image
                    Bitmap finalBitmap;

                    if (scale < 1.0)
                    {
                        // User wants a smaller image
                        int newWidth = (int)(originalBitmap.Width * scale);
                        int newHeight = (int)(originalBitmap.Height * scale);

                        finalBitmap = new Bitmap(newWidth, newHeight);
                        using (Graphics g = Graphics.FromImage(finalBitmap))
                        {
                            // Draw the big image onto the small bitmap (this scales it)
                            g.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
                        }
                        originalBitmap.Dispose(); // Clean up the big one
                    }
                    else
                    {
                        // User wants 100%, just use the original
                        finalBitmap = originalBitmap;
                    }

                    // --- 5. CREATE FILENAME AND SAVE ---
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string filename = $"d{i + 1}_{timestamp}{fileExtension}";

                    // Save to Local Path (if one is provided)
                    if (!string.IsNullOrWhiteSpace(localPath))
                    {
                        Directory.CreateDirectory(localPath); // Ensure path exists
                        finalBitmap.Save(Path.Combine(localPath, filename), saveFormat);
                    }

                    // Save to Network Path (if one is provided)
                    if (!string.IsNullOrWhiteSpace(networkPath))
                    {
                        // Build the full path including the username
                        string fullNetworkPath = Path.Combine(networkPath, username);
                        Directory.CreateDirectory(fullNetworkPath); // Ensure path exists
                        finalBitmap.Save(Path.Combine(fullNetworkPath, filename), saveFormat);
                    }

                    finalBitmap.Dispose(); // Clean up the final image
                }
            }
            catch (Exception ex)
            {
                // Something went wrong (e.g., network path is unreachable)
                // We'll stop the timer and show an error
                screenshotTimer.Stop();
                isRunning = false;

                // Update UI
                lblStatus.Text = "Status: ERROR!";
                btnStartStop.Text = "Start";
                btnStartStop.BackColor = Color.LightGreen;
                groupBox1.Enabled = true; // Re-enable settings

                MessageBox.Show($"An error occurred: {ex.Message}\n\nThe screenshotter has been stopped.", "Runtime Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
