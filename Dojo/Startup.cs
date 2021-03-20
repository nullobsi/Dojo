using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Dojo
{
	class Startup
	{
		[STAThread]
		public static int Main(string[] args)
		{
			if (args.Length != 0)
			{
				ConsoleAllocator.ShowConsoleWindow();
			}

			// run windowed
			App app = new App();
			app.InitializeComponent();
			app.Run();

			return 0;
		}

		internal static class ConsoleAllocator
		{
			[DllImport(@"kernel32.dll", SetLastError = true)]
			static extern bool AllocConsole();

			[DllImport(@"kernel32.dll")]
			static extern IntPtr GetConsoleWindow();

			[DllImport(@"user32.dll")]
			static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

			const int SwHide = 0;
			const int SwShow = 5;


			public static void ShowConsoleWindow()
			{
				var handle = GetConsoleWindow();

				if (handle == IntPtr.Zero)
				{
					AllocConsole();
				}
				else
				{
					ShowWindow(handle, SwShow);
				}
			}

			public static void HideConsoleWindow()
			{
				var handle = GetConsoleWindow();

				ShowWindow(handle, SwHide);
			}
		}
	}
}