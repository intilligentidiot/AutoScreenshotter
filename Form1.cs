using System.Drawing.Imaging;
using System.IO;

namespace AutoScreenshotter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

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
    }
}
