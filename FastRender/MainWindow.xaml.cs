using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TimelineEntryControl;
using static System.Net.WebRequestMethods;

namespace FastRender
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<Video> VideoList = new List<Video>();
		private bool isDragging;
		private bool isDraggingVideoPanel;
		private System.Windows.Point startpos;
		private const double dragThreshold = 1.0;
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
			Boolean didTouch = false;
			var currentPos = e.GetPosition(videoGrid);
			var delta = currentPos - startpos;
			Border border = sender as Border;
			if (border == null || e.LeftButton != MouseButtonState.Pressed)
			{
				return;
			}
			var position = e.GetPosition(border);
			System.Windows.Point draggedBorder = border.TransformToAncestor(videoGrid).Transform(new System.Windows.Point(0, 0));
			if (delta.X <= -1)
			{
				Canvas.SetLeft(border, 0);
				return;
			}
			//Debug.WriteLine($"RelativePOint:{relativePoint.X}");
			foreach (var item in videoPositionList)
			{
				var test = item.Key;
				var borderRelativePoint = test.TransformToAncestor(videoGrid).Transform(new System.Windows.Point(0, 0));
				var threshold = currentPos - borderRelativePoint;
				Debug.WriteLine(threshold.X);
				//Debug.WriteLine($"Border Point: {test.TransformToAncestor(videoGrid).Transform(new System.Windows.Point(0, 0))}");
				//Debug.WriteLine($"Border width:{test.Width}");
				if (test != border)
				{
					if ((borderRelativePoint.X - (draggedBorder.X + border.Width)) < 5 &&
						(borderRelativePoint.X - (draggedBorder.X + border.Width)) > -5)
					{
						if (threshold.X > -border.Width)
						{
							Canvas.SetLeft(border, borderRelativePoint.X - border.Width);
							didTouch = true;
						}
						else {
							Canvas.SetLeft(border, currentPos.X - startpos.X);
						}
					}
					else
					{
						Canvas.SetLeft(border, currentPos.X - startpos.X);
					}
					Debug.WriteLine($"Dragged Border X:{draggedBorder.X}");
					Debug.WriteLine($"Border X:{borderRelativePoint.X}");
					/*if (draggedBorder.X + border.Width > borderRelativePoint.X &&
						draggedBorder.X + border.Width < borderRelativePoint.X + test.Width)
					{
						if (threshold.X < -10)
						{
							//Canvas.SetLeft(border, currentPos.X - startpos.X);
						}
						else
						{
							//Canvas.SetLeft(border, borderRelativePoint.X - border.Width - 1);

						}

						border.CaptureMouse();
						return;
					}*/
				}
			}
			if (!didTouch)
			{
				Canvas.SetLeft(border, currentPos.X - startpos.X);
				border.CaptureMouse();
			}
			//Debug.WriteLine($"border Position:{position.X} Startpos:{startpos.X}");
			//Debug.WriteLine($"CurrentPOs:{currentPos.X}");
		}
		private void videoPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			border = sender as Border;
			isDraggingVideoPanel = false;
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

		}
		private Boolean isBordersTouching(System.Windows.Point relativePoint, System.Windows.Point currentPos)
		{
			foreach (var item in videoPositionList)
			{
				var test = item.Key;
				var borderRelativePoint = test.TransformToAncestor(videoGrid).Transform(new System.Windows.Point(0, 0));
				var threshold = currentPos - borderRelativePoint;
				if (relativePoint.X < borderRelativePoint.X + test.Width &&
					relativePoint.X > borderRelativePoint.X
					)
				{
					if (threshold.X > 10 + test.Width)
					{
						Canvas.SetLeft(border, currentPos.X - startpos.X);
					}
					else
					{
						Canvas.SetLeft(border, borderRelativePoint.X - 1);
					}
					border.CaptureMouse();
					return true;
				}

				if (relativePoint.X + border.Width > borderRelativePoint.X &&
					relativePoint.X + border.Width < borderRelativePoint.X + test.Width)
				{
					if (threshold.X < -10)
					{
						Canvas.SetLeft(border, currentPos.X - startpos.X);
					}
					else
					{
						Canvas.SetLeft(border, borderRelativePoint.X + test.Width + 1);
					}

					border.CaptureMouse();
					return true;
				}
				return true;
			}
			return false;
		}
		private void videoPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			isDraggingVideoPanel = false;
			if (border != null)
			{
				border.BorderBrush = new BrushConverter().ConvertFrom("#0099ff") as System.Windows.Media.Brush;
			}
			if (border != null)
			{
				border.ReleaseMouseCapture();
				border = null;
			}
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