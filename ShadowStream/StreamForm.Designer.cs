namespace ShadowStream
{
	partial class StreamForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamForm));
			pic = new PictureBox();
			((System.ComponentModel.ISupportInitialize)pic).BeginInit();
			SuspendLayout();
			// 
			// pic
			// 
			pic.BackColor = SystemColors.ActiveCaptionText;
			pic.Dock = DockStyle.Fill;
			pic.Location = new Point(0, 0);
			pic.Name = "pic";
			pic.Size = new Size(438, 206);
			pic.SizeMode = PictureBoxSizeMode.Zoom;
			pic.TabIndex = 0;
			pic.TabStop = false;
			pic.MouseMove += pic_MouseMove;
			// 
			// StreamForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			BackColor = SystemColors.ActiveCaptionText;
			ClientSize = new Size(438, 206);
			Controls.Add(pic);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "StreamForm";
			Text = "Shadow Stream";
			FormClosing += Form1_FormClosing;
			Load += Form1_Load;
			KeyUp += Form1_KeyUp;
			((System.ComponentModel.ISupportInitialize)pic).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private PictureBox pic;
	}
}