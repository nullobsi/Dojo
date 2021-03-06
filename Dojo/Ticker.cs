using System;
using System.ComponentModel;
using System.Timers;

namespace Dojo
{
	public class Ticker : INotifyPropertyChanged
	{
		public Ticker()
		{
			var timer = new Timer();
			timer.Interval = 1000.0;
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}

		public DateTime Now => DateTime.Now;

		public event PropertyChangedEventHandler PropertyChanged;

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
		}
	}
}