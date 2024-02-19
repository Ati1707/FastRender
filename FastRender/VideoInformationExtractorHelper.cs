using System;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
internal static class VideoInformationExtractorHelper
{
	private static string ffmpegPath = Path.Combine(Environment.CurrentDirectory, @"ffmpeg\", "ffmpeg.exe");
	private static string ffprobePath = Path.Combine(Environment.CurrentDirectory, @"ffmpeg\", "ffprobe.exe");
	public static (string, string, string) VidExtractor(string videoFilePath)
	{
		Process process = new Process();
		var videoDuration = GetVideoDuration(process, videoFilePath);
		var imagePath = GetImagePath(process, videoFilePath);

		return (imagePath, GetVideoTitleFromPath(videoFilePath), videoDuration);

	}

	private static String GetVideoDuration(Process p, string videoFilePath)
	{
		String videoDuration = null;
		String stringArguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 -sexagesimal \"{videoFilePath}\"";
		p = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = ffprobePath,
				Arguments = stringArguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				RedirectStandardError = true
			},
			EnableRaisingEvents = true,
		};
		p.Start();
		string processOutput = null;
		while ((processOutput = p.StandardOutput.ReadLine()) != null)
		{
			// do something with processOutput
			Debug.WriteLine(processOutput);
			videoDuration = processOutput.Trim();
		}
		return videoDuration;
	}
	private static String GetImagePath(Process p, string videoFilePath)
	{
		String videoTitle = GetVideoTitleFromPath(videoFilePath);
		String imagePath = "Images\\" + videoTitle + ".jpg";
		String stringArguments = $"-i \"{videoFilePath}\" -vframes 1 -y \"Images/{videoTitle}.jpg\"";
		p = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = ffmpegPath,
				Arguments = stringArguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				RedirectStandardError = true
			},
			EnableRaisingEvents = true,
		};

		p.Start();
		imagePath = Directory.GetCurrentDirectory() + "\\" + imagePath;
		while (!File.Exists(imagePath)) ;
		p.WaitForExit();

		return imagePath;
	}

	private static String GetVideoTitleFromPath(String path) {
		String videoTitle = path.Split("\\").Last().Split(".").First();
		return videoTitle;
	}
}