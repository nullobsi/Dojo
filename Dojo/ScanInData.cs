﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using System.Drawing;
using Dojo.Properties;

namespace Dojo {
	public class ScanInData : INotifyPropertyChanged, IDisposable {
		private bool disposedValue;

		// update left minutes
		private readonly Timer _timer;

		// break time
		private DispatcherTimer _breakTimer;
		private TimeSpan _breakTime = TimeSpan.Zero;

		// json properties
		public long key;
		public string userGuid;
		public string firstName;
		public string lastName;
		public bool hasBirthdayToday;
		public DateTime dateCreated;
		public string beltName;
		public int totalHours;
		public int scanInSessionLength;
		public string programSlug;
		public int jrTotalMins;


		public ScanInData() {
			_timer = new Timer(1000.0) {AutoReset = true};
			_timer.Elapsed += Timer_Elapsed;
			_timer.Start();
		}

		public void StartBreak(int minutes) {
			// break time left
			_breakTime = TimeSpan.FromMinutes(minutes);
			// update property
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BreakStatus"));
			// stop existing timer if it exists
			_breakTimer?.Stop();
			_breakTimer = new DispatcherTimer(
				// every second
				new TimeSpan(0, 0, 1),
				DispatcherPriority.Normal,
				(_param1, _param2) => {
					// decrease left time by 1 second
					_breakTime = _breakTime.Add(TimeSpan.FromSeconds(-1.0));
					// stop timer if no time left
					if (_breakTime == TimeSpan.Zero)
						_breakTimer.Stop();
					// send property changed signal
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BreakStatus"));
				},
				Application.Current.Dispatcher);
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinutesLeft"));
		}

		public TimeSpan BreakStatus => _breakTime;

		public void StopBreak() {
			_breakTimer?.Stop();
			_breakTime = TimeSpan.Zero;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BreakStatus"));
		}

		public DateTime DateCreated => dateCreated;

		public string FullName => firstName + " " + lastName.FirstOrDefault() + ".";

		public int MinutesLeft => (int) Math.Floor((dateCreated.AddHours(scanInSessionLength) - DateTime.Now).TotalMinutes);

		public Bitmap Image {
			get {
				switch (beltName){
					case "Black":
						return Resources.BlackBelt;
					case "Blue":
						return Resources.BlueBelt;
					case "Brown":
						return Resources.BrownBelt;
					case "Green":
						return Resources.GreenBelt;
					case "Orange":
						return Resources.OrangeBelt;
					case "Purple":
						return Resources.PurpleBelt;
					case "Red":
						return Resources.RedBelt;
					case "White":
						return Resources.WhiteBelt;
					case "Yellow":
						return Resources.YellowBelt;
					default:
						return Resources.WhiteBelt;
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void Dispose(bool disposing) {
			if (disposedValue)
				return;
			if (disposing){
				_timer?.Stop();
				_timer?.Dispose();
				_breakTimer?.Stop();
			}

			disposedValue = true;
		}

		public void Dispose() => Dispose(true);
	}
}