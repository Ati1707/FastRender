using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TimelineEntryControl;

namespace FastRender
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<Video> VideoList = new List<Video>();
		public MainWindow()
		{
			InitializeComponent();
			System.IO.Directory.CreateDirectory("Images");
			System.IO.Directory.CreateDirectory("Videos");
			videoListBox.ItemsSource = VideoList;

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
					mediaElement.Source = new System.Uri(((sender as ListBox).SelectedItem as Video).VideoPath);
					break;
				}
				obj = VisualTreeHelper.GetParent(obj);
			}
		}

		private void AddVideoFromFile(String filepath)
		{
			// Add logic to extract video information and add it to your VideoList
			// For demonstration purposes, let's assume you have a Video class with VideoThumbnail and VideoTitle properties.
			var videoInfo = VideoInformationExtractorHelper.VidExtractor(filepath);
			VideoList.Add(new Video { VideoThumbnail = videoInfo.Item1, VideoTitle = videoInfo.Item2, VideoDuration = videoInfo.Item3 , VideoPath = filepath});

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

	}
	public class Video
	{

		public required string VideoThumbnail { get; set; }
		public required string VideoTitle { get; set; }
		public required string VideoDuration { get; set; }
		public required string VideoPath {  get; set; }
		public override string ToString() => this.VideoTitle;
	}


}