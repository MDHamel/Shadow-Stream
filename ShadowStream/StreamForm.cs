using DirectShowLib;
using DiscordRPC.Message;
using Microsoft.VisualBasic;
using ShadowStream.Properties;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms;


namespace ShadowStream
{
	public partial class StreamForm : Form
	{

		private static VideoStream video;
		private static AudioStream audio;

		private System.Threading.Timer alertTimer;
		private System.Windows.Forms.Timer cursorTimer;
		int strokeSize = 3;
		private string alertText = "";
		private Point previousCursorPosition = Cursor.Position;
		Cursor transparentCursor = new Cursor(new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb).GetHicon());
		internal static bool cursorLock = false;

		private Settings settings;

		public StreamForm()
		{
			InitializeComponent();
			this.ClientSize = new System.Drawing.Size(1920, 1080);

			alertTimer = new System.Threading.Timer(alertTimerEvent, null, 2000, Timeout.Infinite);

			cursorTimer = new System.Windows.Forms.Timer();
			cursorTimer.Interval = 3000;
			cursorTimer.Tick += CursorTimer_Tick;

			this.MouseWheel += Form1_MouseWheel;
			pic.Paint += pic_Paint;
		}

		private void CursorTimer_Tick(object? sender, EventArgs e)
		{
			Cursor = transparentCursor;
		}

		private void pic_Paint(object sender, PaintEventArgs e)
		{
			if (alertText != "")
			{
				Font font = new Font("Arial", Width / 45);
				Brush fillBrush = new SolidBrush(Color.White);
				Brush outlineBrush = new SolidBrush(Color.Black);

				int margin = 30; // Margin from top and right edges, adjust as needed
				int x = pic.Width - (int)e.Graphics.MeasureString(alertText, font).Width - margin - 10;
				int y = margin;

				// Draw the black outline
				for (int i = -strokeSize; i <= strokeSize; i++)
				{
					for (int j = -strokeSize; j <= strokeSize; j++)
					{
						if (i != 0 || j != 0) // Skip the center to avoid double drawing
						{
							e.Graphics.DrawString(alertText, font, outlineBrush, new PointF(x + i, y + j));
						}
					}
				}

				// Draw the filled text on top of the outline
				e.Graphics.DrawString(alertText, font, fillBrush, new PointF(x, y));
			}
			if (audio.IsMuted)
			{
				// Load the mute icon image from the resource folder (replace "YourNamespace" with the actual namespace of your project)
				Image muteIcon = Resources.muteIcon;

				int x = pic.Width - muteIcon.Width - 40;
				int y = pic.Height - muteIcon.Height - 40;

				// Draw the mute icon image at the desired location
				e.Graphics.DrawImage(muteIcon, x, y); // Adjust the location and size as needed

				// Dispose of the image object
				muteIcon.Dispose();
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (Properties.Settings.Default.shadowSense)
			{
				DsDevice[] videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
				DsDevice[] audioDevices = DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);

				foreach (var device in videoDevices)
				{
					if (device.Name.Contains("ShadowCast"))
					{
						Properties.Settings.Default.videoDevice = device.Name;
						break;
					}
				}

				foreach (var device in audioDevices)
				{

					if (device.Name.Contains("ShadowCast"))
					{
						Properties.Settings.Default.audioDevice = device.Name;
						break;
					}
				}
			}

			video = new VideoStream(this);
			audio = new AudioStream();
		}

		private void alertTimerEvent(object state)
		{
			alertText = "";
		}

		private void alert(string text, int seconds = 2)
		{
			alertText = text;
			alertTimer.Change(seconds * 1000, Timeout.Infinite);
		}

		private void Form1_MouseWheel(object sender, MouseEventArgs e)
		{
			int volume = Properties.Settings.Default.volume;

			//dont adjust volume while muted
			if (!audio.IsMuted)
			{
				// Check if the mouse wheel was scrolled up
				if (e.Delta > 0)
				{
					if (volume <= 195)
						Properties.Settings.Default.volume += 5;
				}
				// Check if the mouse wheel was scrolled down
				else if (e.Delta < 0)
				{
					if (volume >= 5)
						Properties.Settings.Default.volume -= 5;
				}

				alert($"Volume: {Properties.Settings.Default.volume}%");

			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			video.Stop();
			audio.Stop();
			alertTimer.Dispose(); // Dispose the timer to release resources
		}


		// Keyboard Controls:
		// F1 - Opens the settings window
		// F5 or R - Refreshes the audio and video streams
		// Esc - Closes fullscreen
		// M - Toggles muting the audio
		// S - Takes a screenshot of your stream and saves it to the clipboard
		// Ctrl + S - Saves the screenshot to your computer

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F1)
			{
				// Show settings form
				settings = new Settings();
				settings.StartPosition = FormStartPosition.CenterScreen;
				settings.Show();
			}
			else if (e.KeyCode == Keys.F5 || e.KeyCode == Keys.R)
			{
				// Refresh stream
				video.Stop();
				video = new VideoStream(this);
				audio.Stop();
				audio = new AudioStream();
			}
			else if (e.KeyCode == Keys.M)
			{
				//mute audio
				audio.Mute();
				if (audio.IsMuted)
					alert("Muted\t");
				else
					alertTimer.Change(0, 0);

			}
			else if (e.KeyCode == Keys.Escape)
			{
				video.CloseFullScreen();
			}
			else if (e.Control && e.KeyCode == Keys.S)
			{
				// Create a SaveFileDialog instance
				SaveFileDialog saveFileDialog = new SaveFileDialog();

				// Set the properties of the SaveFileDialog, should be able to save to most file types automatically
				saveFileDialog.Title = "Save Screenshot";
				saveFileDialog.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|All Files (*.*)|*.*";
				saveFileDialog.FilterIndex = 1;
				saveFileDialog.RestoreDirectory = true;

				//set the image to when you press the buttons, 
				//clone the frame because the frame wont exist 
				Image screenCap = (Image)pic.Image.Clone();

				// Show the SaveFileDialog and get the result
				DialogResult result = saveFileDialog.ShowDialog();

				// Check if the user clicked the "Save" button
				if (result == DialogResult.OK)
				{
					//save image at location
					string fileName = saveFileDialog.FileName.ToString();
					screenCap.Save(fileName);
					alert($"Frame Saved to {fileName}");
				}
			}
			else if (e.KeyCode == Keys.PrintScreen || e.KeyCode == Keys.S)
			{
				//save image to clipboard
				Clipboard.SetImage(pic.Image);
				alert("Frame Saved to Clipboard");
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.C)
			{
				Cursor.Current = transparentCursor;
			}

		}

		private void pic_MouseMove(object sender, MouseEventArgs e)
		{
			if (Cursor == transparentCursor)
			{
				Cursor = Cursors.Default;
			}
			if (!cursorLock)
				cursorTimer.Start();
			else
				cursorTimer.Stop();
		}
	}
}
