using DirectShowLib;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Diagnostics;

namespace ShadowStream
{
	public partial class Settings : Form
	{
		private ComboBox qualityComboBox;
		private CheckBox shadowSenseCheckBox;
		private ComboBox videoDeviceComboBox;
		private ComboBox audioDeviceComboBox;
		private ComboBox outputDeviceComboBox;

		private Label audioDeviceLabel;
		private Label videoDeviceLabel;

		public Settings()
		{
			InitializeComponent();
			Cursor.Current = Cursors.Default;

			InitializeUI();
		}

		private void InitializeUI()
		{
			// Set up the resolution ComboBox
			qualityComboBox = new ComboBox();
			qualityComboBox.Items.AddRange(new object[] { "720p 30fps", "720p 60fps", "1080p 30fps" });
			qualityComboBox.Location = new System.Drawing.Point(120, 20);
			qualityComboBox.Size = new System.Drawing.Size(121, 21);
			qualityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			qualityComboBox.SelectedIndex = Properties.Settings.Default.streamQuality;
			this.Controls.Add(qualityComboBox);

			// Set up the shadowSense CheckBox
			shadowSenseCheckBox = new CheckBox();
			shadowSenseCheckBox.Text = "Enable ShadowSense 🛈";
			shadowSenseCheckBox.Location = new System.Drawing.Point(20, 60);
			shadowSenseCheckBox.Size = new System.Drawing.Size(175, 21);
			shadowSenseCheckBox.CheckedChanged += new EventHandler(ShadowSenseCheckBox_CheckedChanged);
			this.Controls.Add(shadowSenseCheckBox);

			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(shadowSenseCheckBox, "When enabled it automatically locates your ShadowCast devices");

			// Set up the videoDevice ComboBox
			videoDeviceComboBox = new ComboBox();
			videoDeviceComboBox.Location = new System.Drawing.Point(120, 100);
			videoDeviceComboBox.Size = new System.Drawing.Size(200, 21);
			videoDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.Controls.Add(videoDeviceComboBox);

			// Set up the audioDevice ComboBox
			audioDeviceComboBox = new ComboBox();
			audioDeviceComboBox.Location = new System.Drawing.Point(120, 140);
			audioDeviceComboBox.Size = new System.Drawing.Size(200, 21);
			audioDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.Controls.Add(audioDeviceComboBox);

			// Set up the outputDevice ComboBox
			outputDeviceComboBox = new ComboBox();
			outputDeviceComboBox.Location = new System.Drawing.Point(120, 180);
			outputDeviceComboBox.Size = new System.Drawing.Size(200, 21);
			outputDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

			this.Controls.Add(outputDeviceComboBox);

			// Enumerate video capture devices
			DsDevice[] videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
			foreach (DsDevice device in videoDevices)
			{
				videoDeviceComboBox.Items.Add(device.Name);
			}

			if(videoDeviceComboBox.Items.Count == 0)
			{
				videoDeviceComboBox.Items.Add("No Devices Found");

			}

			videoDeviceComboBox.SelectedIndex = 0;

			// Enumerate audio capture devices
			DsDevice[] audioDevices = DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);
			foreach (DsDevice device in audioDevices)
			{
				audioDeviceComboBox.Items.Add(device.Name);
			}

			if (audioDeviceComboBox.Items.Count == 0)
			{
				audioDeviceComboBox.Items.Add("No Devices Found");

			}

			audioDeviceComboBox.SelectedIndex = 0;

			// Enumerate audio output devices
			var devices = DirectSoundOut.Devices;
			foreach (var device in devices)
			{
				outputDeviceComboBox.Items.Add(device.Description);
			}
			outputDeviceComboBox.SelectedIndex = 0;



			// Set up the labels
			Label resolutionLabel = new Label();
			resolutionLabel.Text = "Stream Quality:";
			resolutionLabel.Location = new System.Drawing.Point(20, 20);
			this.Controls.Add(resolutionLabel);

			videoDeviceLabel = new Label();
			videoDeviceLabel.Text = "Video Device:";
			videoDeviceLabel.Location = new System.Drawing.Point(20, 100);
			videoDeviceLabel.ForeColor = Color.Gray;
			this.Controls.Add(videoDeviceLabel);

			audioDeviceLabel = new Label();
			audioDeviceLabel.Text = "Audio Device:";
			audioDeviceLabel.Location = new System.Drawing.Point(20, 140);
			audioDeviceLabel.ForeColor = Color.Gray;
			this.Controls.Add(audioDeviceLabel);

			Label outputDeviceLabel = new Label();
			outputDeviceLabel.Text = "Output Device:";
			outputDeviceLabel.Location = new System.Drawing.Point(20, 180);
			this.Controls.Add(outputDeviceLabel);

			// Set up the buttons
			Button okButton = new Button();
			okButton.Text = "OK";
			okButton.Location = new System.Drawing.Point(80, 260);
			okButton.Click += new EventHandler(OkButton_Click);
			this.Controls.Add(okButton);

			Button closeButton = new Button();
			closeButton.Text = "Close";
			closeButton.Location = new System.Drawing.Point(20, 260);
			closeButton.Click += new EventHandler(CloseButton_Click);
			this.Controls.Add(closeButton);


			if (Properties.Settings.Default.shadowSense != null)
			{
				bool isChecked = Properties.Settings.Default.shadowSense;
				shadowSenseCheckBox.Checked = Properties.Settings.Default.shadowSense;
				videoDeviceComboBox.Enabled = !isChecked;
				audioDeviceComboBox.Enabled = !isChecked;
				audioDeviceLabel.ForeColor = isChecked ? Color.Gray : SystemColors.ControlText;
				videoDeviceLabel.ForeColor = isChecked ? Color.Gray : SystemColors.ControlText;

			}

			if (Properties.Settings.Default.videoDevice != null)
			{
				if (videoDeviceComboBox.Items.Contains(Properties.Settings.Default.videoDevice))
				{
					videoDeviceComboBox.SelectedItem = Properties.Settings.Default.videoDevice;
				}
			}
			if (Properties.Settings.Default.audioDevice != null)
			{
				if (audioDeviceComboBox.Items.Contains(Properties.Settings.Default.audioDevice))
				{
					audioDeviceComboBox.SelectedItem = Properties.Settings.Default.audioDevice;
				}
			}
			if (Properties.Settings.Default.outputDevice != null)
			{
				outputDeviceComboBox.SelectedItem = Properties.Settings.Default.outputDevice;
			}
			this.AutoSize = true;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.Padding = new Padding(20);

			//center buttons
			okButton.Location = new Point((this.Size.Width / 2) + 20, 260);
			closeButton.Location = new Point((this.Size.Width / 2) - closeButton.Width - 20, 260);


		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			// Save user settings here
			if (Properties.Settings.Default.streamQuality != qualityComboBox.SelectedIndex)
			{
				var parts = qualityComboBox.SelectedItem.ToString().Split(" ");
				int resolution = int.Parse(parts[0].Remove(parts[0].Length - 1));
				int fps = int.Parse(parts[1].Remove(parts[1].Length - 3));

				Debug.WriteLine($"resolution: {parts[0].Remove(parts[0].Length - 1)}\nfps: {fps}");


				Properties.Settings.Default.resolution = resolution;
				Properties.Settings.Default.fps = fps;
				Properties.Settings.Default.streamQuality = qualityComboBox.SelectedIndex;


			}

			Properties.Settings.Default.shadowSense = shadowSenseCheckBox.Checked;

			if (shadowSenseCheckBox.Checked)
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
					Debug.WriteLine(device.Name.Contains("ShadowCast"));
					if (device.Name.Contains("ShadowCast"))
					{
						Properties.Settings.Default.audioDevice = device.Name;
						break;
					}
				}
			}
			else
			{
				if (videoDeviceComboBox.Enabled && videoDeviceComboBox.SelectedItem != null && Properties.Settings.Default.videoDevice != videoDeviceComboBox.SelectedItem.ToString())
				{
					Properties.Settings.Default.videoDevice = videoDeviceComboBox.SelectedItem.ToString();
				}

				if (audioDeviceComboBox.Enabled && audioDeviceComboBox.SelectedItem != null && Properties.Settings.Default.audioDevice != audioDeviceComboBox.SelectedItem.ToString())
				{
					Properties.Settings.Default.audioDevice = audioDeviceComboBox.SelectedItem.ToString();
				}

			}

			if (Properties.Settings.Default.outputDevice != outputDeviceComboBox.SelectedItem.ToString())
			{
				Properties.Settings.Default.outputDevice = outputDeviceComboBox.SelectedItem.ToString();
			}

			Properties.Settings.Default.Save();

			// Close the form
			this.Close();
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			// Close the form
			this.Close();
		}

		private void ShadowSenseCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			bool isChecked = !shadowSenseCheckBox.Checked;
			videoDeviceComboBox.Enabled = isChecked;
			audioDeviceComboBox.Enabled = isChecked;

			// Set the video and audio labels' ForeColor to gray if shadowSenseCheckBox is checked, otherwise set it to the default color
			Color labelColor = isChecked ? SystemColors.ControlText : Color.Gray;
			videoDeviceLabel.ForeColor = labelColor;
			audioDeviceLabel.ForeColor = labelColor;
		}

		private void Settings_Load(object sender, EventArgs e)
		{
			StreamForm.cursorLock = true;
			Cursor = Cursors.Default;
		}

		private void Settings_FormClosed(object sender, FormClosedEventArgs e)
		{
			StreamForm.cursorLock = false;

		}

	}
}
