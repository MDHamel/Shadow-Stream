namespace ShadowStream
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			try
			{
				Application.Run(new StreamForm());
			}
			catch (Exception ex)
			{
				// Log the exception information, e.g., ex.Message, ex.StackTrace, etc.
				MessageBox.Show($"An error occurred: {ex.Message}");
			}
		}
	}
}
