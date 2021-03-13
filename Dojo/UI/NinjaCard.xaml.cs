using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dojo.UI
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
			ScanInData.PropertyChanged += ScanInTick;

			// create shake animation
			_shakeAnim = new Storyboard();
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
			_shakeAnim.Children.Add(doubleAnimation);

			// set data context
			InitializeComponent();
			MainGrid.DataContext = this;
		}

		public ScanInData ScanInData { get; }

		private void StartBreak(object sender, RoutedEventArgs e)
		{
			if (!IntInput.Value.HasValue)
				return;
			BottomRect.Fill = GreenBrush;
			StartButton.IsEnabled = false;
			EndBtn.IsEnabled = true;
			ScanInData.StartBreak(IntInput.Value.Value);
		}

		private void ScanInTick(object sender, PropertyChangedEventArgs e)
		{
			// check for minutes left update
			if (e.PropertyName == "MinutesLeft" && ScanInData.MinutesLeft <= 5)
				Dispatcher.Invoke(() =>
				{
					// allow closing, fill red
					CloseBtn.Visibility = Visibility.Visible;
					BottomRect.Fill = RedBrush;
					MinutesLeftLabel.Foreground = RedBrush;
				});

			// change to red & shake if break is over
			if (e.PropertyName != "BreakStatus" || ScanInData.BreakStatus != TimeSpan.Zero || !EndBtn.IsEnabled)
				return;
			BottomRect.Fill = RedBrush;
			_shakeAnim.Begin(this, true);
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			// remove from parent
			// TODO: add restore option
			(Parent as AlignableWrapPanel).Children.Remove(this);
			ScanInData.Dispose();
		}

		private void EndBreak(object sender, RoutedEventArgs e)
		{
			BottomRect.Fill = BlueBrush;
			EndBtn.IsEnabled = false;
			StartButton.IsEnabled = true;
			_shakeAnim.Stop(this);
			ScanInData.StopBreak();
		}
	}
}