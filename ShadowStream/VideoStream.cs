using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShadowStream
{
	public class VideoStream
	{
		private PictureBox pic;
		private VideoCapture videoFeed;

		private int frameWidth = 1920;
		private int frameHeight = 1080;
		private int[] formoffset = { 16, 39 };

		private int fps = 30;
		private int refreshRate;

		private int videoDeviceIndex = 0;

		private bool isRunning = false;
		private bool isFullscreen = false;



		private Form form;
		private System.Drawing.Size prevSize;
		private FormWindowState prevWindowState;



		public VideoStream(Form f)
		{
			pic = f.Controls["pic"] as PictureBox;

			// Handle the SizeChanged event of the form to update the size of the PictureBox
			form = f;

			prevWindowState = form.WindowState;
			prevSize = form.Size;

			form.SizeChanged += Form_SizeChanged;

			// Handle the DoubleClick event of the PictureBox to toggle full screen
			pic.DoubleClick += Pic_DoubleClick;


			Properties.Settings.Default.PropertyChanged += OnSettingsChanged;

			init();
		}

		private void Form_SizeChanged(object sender, EventArgs e)
		{
			if (form.WindowState != FormWindowState.Minimized && form.Width > 1 && form.Height > 1)
			{
				// Calculate the new width and height based on the aspect ratio
				int newWidth = pic.Width;
				int newHeight = (int)Math.Round(newWidth / 16.0 * 9.0);
				if (newHeight > pic.Height)
				{
					newHeight = pic.Height;
					newWidth = (int)Math.Round(newHeight / 9.0 * 16.0);
				}

				// Update the frame width and height
				frameWidth = newWidth;
				frameHeight = newHeight;


				// Resize the PictureBox to maintain the aspect ratio
				pic.Width = newWidth;
				pic.Height = newHeight;
			}
		}

		private void Pic_DoubleClick(object sender, EventArgs e)
		{
			if (isFullscreen)
			{
				// Restore the window to its previous size and state
				form.WindowState = prevWindowState;
				form.ClientSize = prevSize;
				form.FormBorderStyle = FormBorderStyle.Sizable;
				isFullscreen = false;

			}
			else
			{
				// Save the current size and state of the window
				prevSize = form.ClientSize;
				prevWindowState = form.WindowState;

				// Enter full screen mode
				form.FormBorderStyle = FormBorderStyle.None;
				form.WindowState = FormWindowState.Maximized;
				isFullscreen = true;

			}


			GC.Collect();

		}

		public void CloseFullScreen()
		{
			if (isFullscreen)
			{
				// Restore the window to its previous size and state
				form.WindowState = prevWindowState;
				form.ClientSize = prevSize;
				form.FormBorderStyle = FormBorderStyle.Sizable;
				isFullscreen = false;

			}

		}

		private void OnSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "resolution" || e.PropertyName == "fps" || e.PropertyName == "videoDevice")
			{
				init();
			}
		}

		private void stream()
		{
			// Stream loop. While stream and app are running, keep stream updating
			while (isRunning && videoFeed.IsOpened())
			{
				try
				{
					// Grab the next frame from the video feed
					//videoFeed.Grab();

					// Decode and return the grabbed frame as a Mat object
					Mat rawFrame = new Mat();
					videoFeed.Retrieve(rawFrame);

					// Resize the frame on a separate thread
					// Resizing helps reduce the lag and keep the 1080p resolution at different window sizes 
					Task.Run(() =>
					{
						// Resize frame and dispose of old frame
						Mat resizedFrame = new Mat();
						Cv2.Resize(rawFrame, resizedFrame, new OpenCvSharp.Size(frameWidth, frameHeight), interpolation: InterpolationFlags.Cubic);
						rawFrame.Release();
						rawFrame.Dispose();

						// Display the resized frame on the UI thread
						pic?.BeginInvoke(new Action(() =>
						{
							try
							{
								if (pic.Image != null)
								{
									pic.Image.Dispose();
									pic.Image = null;
								}
								pic.Image = resizedFrame.ToBitmap();
								pic.Refresh();


							}
							catch (Exception e)
							{
								Debug.WriteLine(e.Message);
							}
							finally
							{
								resizedFrame.Release();
								resizedFrame.Dispose();
							}

							// Dispose of the resized frame after it has been
							

						}));

					});
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}
			}
		}

		private void init()
		{
			isRunning = false;

			fps = Properties.Settings.Default.fps;


			frameHeight = Properties.Settings.Default.resolution;
			frameWidth = frameHeight * 16 / 9;


			this.form.Size = new System.Drawing.Size(frameWidth + formoffset[0], frameHeight + formoffset[1]);


			//refresh video feed
			DsDevice[] videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

			for (int i = 0; i < videoDevices.Length; i++)
			{
				if (videoDevices[i].Name.Contains(Properties.Settings.Default.videoDevice))
				{
					videoDeviceIndex = i;
					break;
				}
			}

			int resolution = Properties.Settings.Default.resolution;

			// Initialize video feed from settings
			if (videoFeed != null)
			{
				videoFeed.Dispose();
			}

			videoFeed = new VideoCapture(videoDeviceIndex, VideoCaptureAPIs.DSHOW);
			videoFeed.Set(VideoCaptureProperties.Fps, fps);
			videoFeed.Set(VideoCaptureProperties.FrameWidth, frameWidth);
			videoFeed.Set(VideoCaptureProperties.FrameHeight, frameHeight);
			videoFeed.Set(VideoCaptureProperties.BufferSize, 1);
			videoFeed.Set(VideoCaptureProperties.BitRate, 5000000);
			videoFeed.Set(VideoCaptureProperties.FourCC, FourCC.MJPG);

			isRunning = true;

			Thread videoStreamThread = new Thread(stream);
			videoStreamThread.Start();

		}

		public void Stop()
		{
			isRunning = false;
			videoFeed.Dispose();
		}
	}

}
