using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DojoCommon.UI
{
	public partial class NinjaCard
	{
		private static readonly Brush      RedBrush   = new SolidColorBrush(Color.FromRgb(189, 5, 5));
		private static readonly Brush      BlueBrush  = new SolidColorBrush(Color.FromRgb(13, 16, 57));
		private static readonly Brush      GreenBrush = new SolidColorBrush(Color.FromRgb(0, 128, 0));
		private readonly        Storyboard _shakeAnim;

		public NinjaCard(ScanInData inData)
		{
			// save ref to scan in
			ScanInData = inData;

			// create shake animation
			_shakeAnim = createStoryboard();

			// set data context
			InitializeComponent();
			MainGrid.DataContext = this;

			ScanInData.PropertyChanged += ScanInTick;
			UpdateState();
		}

		public ScanInData ScanInData { get; private set; }

		private void StartBreak(object sender, RoutedEventArgs e)
		{
			if (!IntInput.Value.HasValue)
				return;
			ScanInData.StartBreak(IntInput.Value.Value);
		}

		private void ScanInTick(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "State") UpdateState();
		}

		private void UpdateState()
		{
			switch (ScanInData.State)
			{
				case ScanInState.Alert:
					BottomRect.Fill = RedBrush;
					EndBtn.IsEnabled = true;
					StartButton.IsEnabled = false;
					_shakeAnim.Begin(this, true);
					break;
				case ScanInState.Finishing:
					BottomRect.Fill = RedBrush;
					MinutesLeftLabel.Foreground = RedBrush;
					EndBtn.IsEnabled = false;
					StartButton.IsEnabled = true;
					CloseBtn.Visibility = Visibility.Visible;
					_shakeAnim.Stop(this);
					break;
				case ScanInState.Normal:
					BottomRect.Fill = BlueBrush;
					EndBtn.IsEnabled = false;
					StartButton.IsEnabled = true;
					_shakeAnim.Stop(this);
					break;
				case ScanInState.OnBreak:
					BottomRect.Fill = GreenBrush;
					EndBtn.IsEnabled = true;
					StartButton.IsEnabled = false;
					_shakeAnim.Stop(this);
					break;
			}
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			// remove from parent
			(Parent as Panel).Children.Remove(this);
			ScanInData.PropertyChanged -= ScanInTick;
			HideCard(ScanInData);
			ScanInData = null;
		}

		private void EndBreak(object sender, RoutedEventArgs e)
		{
			ScanInData.StopBreak();
		}

		public static event Action<ScanInData> hideCard;

		private static void HideCard(ScanInData c)
		{
			hideCard?.Invoke(c);
		}

		private static Storyboard createStoryboard()
		{
			var s = new Storyboard();
			var doubleAnimation = new DoubleAnimation
			{
				From = -10.0,
				To = 10.0,
				Duration = new Duration(TimeSpan.FromSeconds(1)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever
			};
			doubleAnimation.SetValue(Storyboard.TargetNameProperty, nameof(MainGridTransform));
			doubleAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(TranslateTransform.XProperty));
			var bounceEase = new BounceEase
			{
				Bounces = 1,
				Bounciness = 2.0,
				EasingMode = EasingMode.EaseOut
			};
			doubleAnimation.EasingFunction = bounceEase;
			s.Children.Add(doubleAnimation);
			return s;
		}
	}
}