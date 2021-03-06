using System;
using System.Windows;
using System.Windows.Controls;

namespace Dojo
{
	public class AlignableWrapPanel : Panel
	{
		public static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register(
			nameof(HorizontalContentAlignment), typeof(HorizontalAlignment), typeof(AlignableWrapPanel),
			new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

		public HorizontalAlignment HorizontalContentAlignment
		{
			get => (HorizontalAlignment) GetValue(HorizontalContentAlignmentProperty);
			set => SetValue(HorizontalContentAlignmentProperty, value);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var size1 = new Size();
			var size2 = new Size();
			var internalChildren = InternalChildren;
			for (var index = 0; index < internalChildren.Count; ++index)
			{
				var uiElement = internalChildren[index];
				uiElement.Measure(constraint);
				var desiredSize = uiElement.DesiredSize;
				if (size1.Width + desiredSize.Width > constraint.Width)
				{
					size2.Width = Math.Max(size1.Width, size2.Width);
					size2.Height += size1.Height;
					size1 = desiredSize;
					if (!(desiredSize.Width > constraint.Width)) continue;
					size2.Width = Math.Max(desiredSize.Width, size2.Width);
					size2.Height += desiredSize.Height;
					size1 = new Size();
				}
				else
				{
					size1.Width += desiredSize.Width;
					size1.Height = Math.Max(desiredSize.Height, size1.Height);
				}
			}

			size2.Width = Math.Max(size1.Width, size2.Width);
			size2.Height += size1.Height;
			return size2;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var start = 0;
			var lineSize = new Size();
			var y = 0.0;
			var internalChildren = InternalChildren;
			for (var index = 0; index < internalChildren.Count; ++index)
			{
				var desiredSize = internalChildren[index].DesiredSize;
				if (lineSize.Width + desiredSize.Width > arrangeBounds.Width)
				{
					ArrangeLine(y, lineSize, arrangeBounds.Width, start, index);
					y += lineSize.Height;
					lineSize = desiredSize;
					if (desiredSize.Width > arrangeBounds.Width)
					{
						ArrangeLine(y, desiredSize, arrangeBounds.Width, index, ++index);
						y += desiredSize.Height;
						lineSize = new Size();
					}

					start = index;
				}
				else
				{
					lineSize.Width += desiredSize.Width;
					lineSize.Height = Math.Max(desiredSize.Height, lineSize.Height);
				}
			}

			if (start < internalChildren.Count)
				ArrangeLine(y, lineSize, arrangeBounds.Width, start, internalChildren.Count);
			return arrangeBounds;
		}

		private void ArrangeLine(double y, Size lineSize, double boundsWidth, int start, int end)
		{
			var x = 0.0;
			if (HorizontalContentAlignment == HorizontalAlignment.Center)
				x = (boundsWidth - lineSize.Width) / 2.0;
			else if (HorizontalContentAlignment == HorizontalAlignment.Right)
				x = boundsWidth - lineSize.Width;
			var internalChildren = InternalChildren;
			for (var index = start; index < end; ++index)
			{
				var uiElement = internalChildren[index];
				uiElement.Arrange(new Rect(x, y, uiElement.DesiredSize.Width, lineSize.Height));
				x += uiElement.DesiredSize.Width;
			}
		}
	}
}