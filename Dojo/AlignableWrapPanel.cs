using System;
using System.Windows;
using System.Windows.Controls;

namespace Dojo
{
  public class AlignableWrapPanel : Panel
  {
    public static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register(nameof (HorizontalContentAlignment), typeof (HorizontalAlignment), typeof (AlignableWrapPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

    public HorizontalAlignment HorizontalContentAlignment
    {
      get => (HorizontalAlignment) this.GetValue(AlignableWrapPanel.HorizontalContentAlignmentProperty);
      set => this.SetValue(AlignableWrapPanel.HorizontalContentAlignmentProperty, (object) value);
    }

    protected override Size MeasureOverride(Size constraint)
    {
      Size size1 = new Size();
      Size size2 = new Size();
      UIElementCollection internalChildren = this.InternalChildren;
      for (int index = 0; index < internalChildren.Count; ++index)
      {
        UIElement uiElement = internalChildren[index];
        uiElement.Measure(constraint);
        Size desiredSize = uiElement.DesiredSize;
        if (size1.Width + desiredSize.Width > constraint.Width)
        {
          size2.Width = Math.Max(size1.Width, size2.Width);
          size2.Height += size1.Height;
          size1 = desiredSize;
          if (desiredSize.Width > constraint.Width)
          {
            size2.Width = Math.Max(desiredSize.Width, size2.Width);
            size2.Height += desiredSize.Height;
            size1 = new Size();
          }
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
      int start = 0;
      Size lineSize = new Size();
      double y = 0.0;
      UIElementCollection internalChildren = this.InternalChildren;
      for (int index = 0; index < internalChildren.Count; ++index)
      {
        Size desiredSize = internalChildren[index].DesiredSize;
        if (lineSize.Width + desiredSize.Width > arrangeBounds.Width)
        {
          this.ArrangeLine(y, lineSize, arrangeBounds.Width, start, index);
          y += lineSize.Height;
          lineSize = desiredSize;
          if (desiredSize.Width > arrangeBounds.Width)
          {
            this.ArrangeLine(y, desiredSize, arrangeBounds.Width, index, ++index);
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
        this.ArrangeLine(y, lineSize, arrangeBounds.Width, start, internalChildren.Count);
      return arrangeBounds;
    }

    private void ArrangeLine(double y, Size lineSize, double boundsWidth, int start, int end)
    {
      double x = 0.0;
      if (this.HorizontalContentAlignment == HorizontalAlignment.Center)
        x = (boundsWidth - lineSize.Width) / 2.0;
      else if (this.HorizontalContentAlignment == HorizontalAlignment.Right)
        x = boundsWidth - lineSize.Width;
      UIElementCollection internalChildren = this.InternalChildren;
      for (int index = start; index < end; ++index)
      {
        UIElement uiElement = internalChildren[index];
        uiElement.Arrange(new Rect(x, y, uiElement.DesiredSize.Width, lineSize.Height));
        x += uiElement.DesiredSize.Width;
      }
    }
  }
}
