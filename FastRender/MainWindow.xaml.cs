using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FastRender
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static readonly RoutedCommand SnappingCommand = new RoutedCommand();
		public List<Video> VideoList = new List<Video>();
		private bool isDragging;
		private bool isSnippingEnabled = true;
		private System.Windows.Point startpos;
		private bool isDragCompleted;
		private bool isMediaLoaded;
		DispatcherTimer timer;
		private double nextLeftPosition = 0;
		private Border border = new Border();
		private Dictionary<Border, System.Windows.Point> videoPositionList = new Dictionary<Border, System.Windows.Point>();
		private int videoIndex = 0;
		public MainWindow()
		{
			InitializeComponent();
			Directory.CreateDirectory("Images");
			Directory.CreateDirectory("Videos");
			videoListBox.ItemsSource = VideoList;
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(200);
			timer.Tick += new EventHandler(timer_Tick);
			this.CommandBindings.Add(new CommandBinding(SnappingCommand, SnappingCommand_Executed));

		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (isMediaLoaded)
			{
				var mediaDuration = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
				var mediaElapsedTime = mediaElement.Position.TotalMilliseconds;
				timelineSlider.Value = (100 * mediaElapsedTime) / mediaDuration;
				if (timelineSlider.Value == 100)
				{
					timer.Stop();
					isMediaLoaded = false;
				}
			}
		}
		private void ListBox_PreviewDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}

		private void ListBox_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				// Handle the dropped files (add them to your VideoList, for example)
				foreach (var file in files)
				{
					// Add logic to check if the file is a video file and get its information
					// For demonstration purposes, let's assume you have a method called AddVideoFromFile
					// that adds a video to the VideoList based on the file path.
					AddVideoFromFile(file);
				}
			}
		}

		void videoListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			DependencyObject obj = (DependencyObject)e.OriginalSource;

			while (obj != null && obj != videoListBox)
			{
				if (obj.GetType() == typeof(ListBoxItem))
				{
					mediaElement.Source = new Uri(((sender as ListBox).SelectedItem as Video).VideoPath);
					break;
				}
				obj = VisualTreeHelper.GetParent(obj);
			}
			mediaElement.Play();
		}

		void ListBoxItem_MouseLeftButtonDown(object sender, MouseEventArgs e)
		{
			if (e.OriginalSource is FrameworkElement element && element.DataContext is Video video)
			{
				isDragging = true;
			}
		}
		private void AddVideoFromFile(String filepath)
		{
			// Add logic to extract video information and add it to your VideoList
			// For demonstration purposes, let's assume you have a Video class with VideoThumbnail and VideoTitle properties.
			var videoInfo = VideoInformationExtractorHelper.VidExtractor(filepath);
			VideoList.Add(new Video { VideoThumbnail = videoInfo.Item1, VideoTitle = videoInfo.Item2, VideoDuration = videoInfo.Item3, VideoPath = filepath });

			videoListBox.Items.Refresh();

		}

		void Window_Closing(object sender, CancelEventArgs e)
		{
			System.IO.DirectoryInfo imageDirectory = new DirectoryInfo("Images");
			foreach (FileInfo file in imageDirectory.GetFiles())
			{
				file.Delete();
			}
		}


		private void videoListBox_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (isDragging && e.LeftButton == MouseButtonState.Pressed)
			{
				ListBox box = sender as ListBox;
				Video video = box.SelectedItem as Video;
				DragDrop.DoDragDrop(box, video, DragDropEffects.Copy);
			}
		}

		private string GetThumbnailPathForDraggedVideo(Video video)
		{
			return video.VideoThumbnail;
		}

		private void Rectangle_Drop(object sender, DragEventArgs e)
		{
			object data = e.Data.GetData(typeof(Video));
			if (data != null)
			{
				Video video = data as Video;
				TimeSpan test = TimeSpan.Parse(video.VideoDuration);
				System.Windows.Controls.Image thumbnail = new System.Windows.Controls.Image();
				thumbnail.Source = new BitmapImage(new Uri(video.VideoThumbnail));
				Border rect = new Border();
				rect.Height = 90;
				rect.Width = test.Milliseconds;
				rect.Background = new BrushConverter().ConvertFrom("#FF0E2137") as System.Windows.Media.Brush;
				rect.BorderBrush = new BrushConverter().ConvertFrom("#0099ff") as System.Windows.Media.Brush;
				rect.BorderThickness = new Thickness(1, 1, 1, 1);
				TextBlock videoTitle = new TextBlock();
				videoTitle.Text = video.VideoTitle;
				videoTitle.Foreground = new SolidColorBrush(Colors.Black);
				videoTitle.TextAlignment = TextAlignment.Left;
				DockPanel dockPanel = new DockPanel();
				DockPanel.SetDock(videoTitle, Dock.Top);
				dockPanel.Children.Add(videoTitle);
				dockPanel.Children.Add(thumbnail);
				rect.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(videoPanel_MouseLeftButtonDown);
				rect.PreviewMouseMove += new MouseEventHandler(videoPanel_PreviewMouseMove);
				rect.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(videoPanel_MouseLeftButtonUp);

				rect.Child = dockPanel;
				Canvas.SetLeft(rect, nextLeftPosition);
				videoGrid.Children.Add(rect);
				nextLeftPosition += rect.Width;
			}
		}
		private void videoPanel_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var draggedBorder = sender as Border;

			if (draggedBorder == null || e.LeftButton != MouseButtonState.Pressed)
				return;

			var currentMousePosition = e.GetPosition(videoGrid);
			var draggedBorderPosition = draggedBorder.TransformToAncestor(videoGrid).Transform(new Point(0, 0));
			var deltaX = currentMousePosition.X - startpos.X;

			if (deltaX <= -1)
			{
				Canvas.SetLeft(draggedBorder, 0);
				return;
			}

			Canvas.SetLeft(draggedBorder, currentMousePosition.X - startpos.X);

			if (isSnippingEnabled)
			{
				foreach (var item in videoPositionList)
				{
					Border adjacentBorder = item.Key;
					Point adjacentBorderPosition = adjacentBorder.TransformToAncestor(videoGrid).Transform(new Point(0, 0));
					if (IsWithinLeftThreshold((draggedBorderPosition.X + draggedBorder.Width) - adjacentBorderPosition.X))
					{
						Canvas.SetLeft(draggedBorder, adjacentBorderPosition.X - draggedBorder.Width);

						break;
					}
					if (IsWithinRightThreshold(adjacentBorderPosition.X + adjacentBorder.Width - draggedBorderPosition.X))
					{
						Canvas.SetLeft(draggedBorder, adjacentBorder.Width + adjacentBorderPosition.X);
						break;
					}
				}
				return;
			}
		}

		private bool IsWithinLeftThreshold(double value)
		{
			return value >= -15 && value <= 15;
		}
		private bool IsWithinRightThreshold(double value)
		{
			return value >= -15 && value <= 15;
		}
		private void videoPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if(border != null)
			{
				border.BorderBrush = new BrushConverter().ConvertFrom("#0099ff") as System.Windows.Media.Brush;
				border.ReleaseMouseCapture();
				border = null;

			}
			border = sender as Border;
			if (!videoPositionList.ContainsKey(border))
			{
				startpos = e.GetPosition(border);
				videoPositionList.Add(border, startpos);
			}
			else
			{
				startpos = videoPositionList[border];
			}
			border.BorderBrush = new BrushConverter().ConvertFrom("#ff3300") as System.Windows.Media.Brush;
			border.CaptureMouse();

		}
		private void videoPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
				border.ReleaseMouseCapture();
		}

		private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
		{
			mediaElement.Volume = (double)volumeSlider.Value;
		}

		private void SeekToMediaPosition()
		{
			int SliderValue = (int)timelineSlider.Value;
			if (mediaElement.NaturalDuration.HasTimeSpan)
			{
				var mediaDuration = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
				SliderValue = (int)(timelineSlider.Value * mediaDuration) / 100;
			}
			Debug.WriteLine(SliderValue);
			// Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
			// Create a TimeSpan with miliseconds equal to the slider value.

			TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
			mediaElement.Position = ts;
			isDragCompleted = false;
			if (SliderValue != 100)
			{
				timer.Start();
				isMediaLoaded = true;
			}
		}

		private void timelineSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			SeekToMediaPosition();
		}

		private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
		{
			timer.Start();
			videoTotalDuration.Text = mediaElement.NaturalDuration.ToString();
			isMediaLoaded = true;
		}

		private void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs e)
		{
			mediaElement.Play();
		}
		private void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs e)
		{
			mediaElement.Pause();
		}
		private void SnappingCommand_Executed(object sender, RoutedEventArgs e)
		{
			if(isSnippingEnabled)
			{
				isSnippingEnabled = false;
			}
			else
			{
				isSnippingEnabled = true;
			}

		}
	}

	public class Video
	{

		public required string VideoThumbnail { get; set; }
		public required string VideoTitle { get; set; }
		public required string VideoDuration { get; set; }
		public required string VideoPath { get; set; }
		public override string ToString() => this.VideoTitle;
	}
}