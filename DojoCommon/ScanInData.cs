using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DojoCommon
{
	public class ScanInData : INotifyPropertyChanged, IDisposable
	{
		public static readonly Dictionary<string, BitmapImage> Belts;

		// update left minutes
		private readonly Timer    _timer;
		private          TimeSpan _breakTime = TimeSpan.Zero;

		// break time
		private DispatcherTimer _breakTimer;

		// internal state
		public  string   beltName;
		public  DateTime dateCreated;
		private bool     disposedValue;
		public  string   firstName;
		public  bool     hasBirthdayToday;
		public  int      jrTotalMins;

		// json properties
		public long   key;
		public string lastName;
		public string programSlug;
		public int    scanInSessionLength;
		public int    totalHours;
		public string userGuid;

		static ScanInData()
		{
			Belts = new Dictionary<string, BitmapImage>();
			string[] names = {"white", "yellow", "orange", "green", "blue", "purple", "brown", "red", "black"};
			foreach (var beltName in names)
			{
				var img = new BitmapImage();
				img.BeginInit();
				img.UriSource = new Uri($"pack://application:,,,/DojoCommon;component/Belts/{beltName}.png",
				                        UriKind.Absolute);
				img.EndInit();
				Belts[beltName] = img;
			}
		}

		public ScanInData()
		{
			_timer = new Timer(1000.0) {AutoReset = true};
			_timer.Elapsed += Update;
			_timer.Start();
		}

		public ScanInData(NinjaData i)
		{
			firstName = i.FirstName;
			lastName = i.LastName;
			dateCreated = i.CreatedAt;
			beltName = i.BeltName;
			scanInSessionLength = i.SessionLength;
			userGuid = i.UserGuid;
		}

		// Break Status
		public TimeSpan BreakStatus => _breakTime;
		public string BreakLeftStr => _breakTime.ToString(@"mm\:ss");

		// Information fields
		public DateTime DateCreated => dateCreated;
		public string FullName => firstName + " " + lastName.FirstOrDefault() + ".";
		public BitmapImage Image => Belts[beltName.ToLower()];

		// Time Remaining
		public int MinutesLeft =>
			(int) Math.Floor((dateCreated.AddHours(scanInSessionLength) - DateTime.Now).TotalMinutes);

		public ScanInState State { get; private set; } = ScanInState.Normal;


		public void Dispose()
		{
			Dispose(true);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void StartBreak(int minutes)
		{
			// break time left
			_breakTime = TimeSpan.FromMinutes(minutes);
			// update state
			State = ScanInState.OnBreak;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
			// update property
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakStatus)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakLeftStr)));
			// stop existing timer if it exists
			_breakTimer?.Stop();
			_breakTimer = new DispatcherTimer(
				// every second
				new TimeSpan(0, 0, 1),
				DispatcherPriority.Normal,
				(_param1, _param2) =>
				{
					// decrease left time by 1 second
					_breakTime = _breakTime.Add(TimeSpan.FromSeconds(-1.0));
					// stop timer if no time left
					if (_breakTime == TimeSpan.Zero)
					{
						_breakTimer.Stop();
						// update state
						State = ScanInState.Alert;
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
					}

					// send property changed signal
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakStatus)));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakLeftStr)));
				},
				Application.Current.Dispatcher);
		}

		private void Update(object sender, ElapsedEventArgs e)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinutesLeft)));
			if (State == ScanInState.Normal && MinutesLeft <= 5)
			{
				State = ScanInState.Finishing;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
			}
		}

		public void StopBreak()
		{
			_breakTimer?.Stop();
			_breakTime = TimeSpan.Zero;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakStatus)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakLeftStr)));
			State = MinutesLeft <= 5 ? ScanInState.Finishing : ScanInState.Normal;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
				return;
			if (disposing)
			{
				_timer?.Stop();
				_timer?.Dispose();
				_breakTimer?.Stop();
			}

			disposedValue = true;
		}

		public override string ToString()
		{
			return FullName;
		}
	}

	public enum ScanInState
	{
		Normal,
		OnBreak,
		Alert,
		Finishing
	}
}