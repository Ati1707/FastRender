using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FastRender
{
	public partial class TimeLineCursor : UserControl
	{
		protected bool isDragging;
		private System.Windows.Point clickPosition;
		public TimeLineCursor()
		{
			InitializeComponent();
			this.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
			this.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
			this.MouseMove += new MouseEventHandler(Control_MouseMove);
		}
		private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			isDragging = true;
			var draggableControl = sender as UserControl;
			clickPosition = e.GetPosition(this);
			draggableControl.CaptureMouse();
		}

		private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			isDragging = false;
			var draggable = sender as UserControl;
			draggable.ReleaseMouseCapture();
		}

		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.IsMouseCaptured)
			{
				var container = VisualTreeHelper.GetParent(this) as UIElement;
				if (container == null)
					return;
				var mousePosition = e.GetPosition(container).X - 90;
				this.RenderTransform = new TranslateTransform(mousePosition - clickPosition.X, 0);
			}
		}
	}
}
