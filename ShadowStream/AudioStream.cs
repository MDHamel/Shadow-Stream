using DirectShowLib;
using NAudio.Wave;
using System.Diagnostics;

namespace ShadowStream
{
	public class AudioStream
	{
		private WaveIn recorder;
		private BufferedWaveProvider bufferedWaveProvider;
		private DirectSoundOut ds;
		private int audioDeviceInIndex;
		private Thread audioThread;
		private int latency = 40;
		private bool isMuted = false;


		public bool IsMuted
		{
			get { return isMuted; }
		}


		public AudioStream()
		{
			init();

			Properties.Settings.Default.PropertyChanged += OnSettingsChanged;
		}

		private void init()
		{
			var audioOutDevices = DirectSoundOut.Devices;
			var audioOutDevice = audioOutDevices.FirstOrDefault(d => d.Description == Properties.Settings.Default.outputDevice);
			ds = new DirectSoundOut(audioOutDevice != null ? audioOutDevice.Guid : DirectSoundOut.DSDEVID_DefaultPlayback, latency);


			DsDevice[] audioInDevices = DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);
			for (int i = 0; i < audioInDevices.Length; i++)
			{
				if (audioInDevices[i].Name.Contains(Properties.Settings.Default.audioDevice))
				{
					audioDeviceInIndex = i;
					break;
				}
			}

			recorder = new WaveIn();

			audioThread = new Thread(audioStream);
			audioThread.Start();
		}

		private void OnSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "outputDevice" || e.PropertyName == "audioDevice")
			{
				ds.Stop();
				ds.Dispose();
				init();
			}
		}

		public void audioStream()
		{
			// Set audio thread priority to high
			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			recorder.DeviceNumber = audioDeviceInIndex;
			recorder.DataAvailable += recorder_DataAvailable;
			recorder.WaveFormat = new WaveFormat(96000, 1);
			bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat) { BufferDuration = TimeSpan.FromMilliseconds(200) };
			bufferedWaveProvider.DiscardOnBufferOverflow = true;

			recorder.StartRecording();

			ds.Init(bufferedWaveProvider);
			ds.Play();
		}

		void recorder_DataAvailable(object sender, WaveInEventArgs e)
		{
			if (bufferedWaveProvider.BufferedDuration.TotalMilliseconds <= bufferedWaveProvider.BufferDuration.TotalMilliseconds)
			{
				// Apply volume control by multiplying each sample by the volume multiplier, if muted the multiplier is set to 0.
				double volumeMultiplier = isMuted ? 0 : Properties.Settings.Default.volume / 100f;
				for (int i = 0; i < e.BytesRecorded; i += 2)
				{
					short sample = BitConverter.ToInt16(e.Buffer, i);
					sample = (short)(sample * volumeMultiplier);
					byte[] sampleBytes = BitConverter.GetBytes(sample);
					bufferedWaveProvider.AddSamples(sampleBytes, 0, 2);
				}
			}
		}

		public void Mute()
		{
			isMuted = !isMuted;
		}

		public void Stop()
		{
			if (recorder != null) { recorder.StopRecording(); recorder.Dispose(); }
			if (ds != null) { ds.Stop(); ds.Dispose(); }

		}
	}

}
