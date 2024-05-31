namespace Iebcn.FaceFusion;
public static class TempFileManager
{
    private static readonly string TEMP_DIRECTORY_PATH = Path.Combine(Path.GetTempPath(), "facefusion");
    private static readonly string TEMP_OUTPUT_VIDEO_NAME = "temp.mp4";

    public static VideoProcessingOption Globals { get; set; } = new VideoProcessingOption();

    public static List<string> GetTempFramePaths(string targetPath)
    {
        string tempFramesPattern = GetTempFramesPattern(targetPath, "*");
        return Directory.GetFiles(TEMP_DIRECTORY_PATH, tempFramesPattern).ToList();
    }

    public static string GetTempFramesPattern(string targetPath, string tempFramePrefix)
    {
        string tempDirectoryPath = GetTempDirectoryPath(targetPath);
        return Path.Combine(tempDirectoryPath, $"{tempFramePrefix}.{Globals.TempFrameFormat}");
    }

    public static string GetTempDirectoryPath(string targetPath)
    {
        string targetName = Path.GetFileNameWithoutExtension(targetPath);
        return Path.Combine(TEMP_DIRECTORY_PATH, targetName);
    }

    public static string GetTempOutputVideoPath(string targetPath)
    {
        string tempDirectoryPath = GetTempDirectoryPath(targetPath);
        return Path.Combine(tempDirectoryPath, TEMP_OUTPUT_VIDEO_NAME);
    }

    public static void CreateTemp(string targetPath)
    {
        string tempDirectoryPath = GetTempDirectoryPath(targetPath);
        Directory.CreateDirectory(tempDirectoryPath);
    }

    public static void MoveTemp(string targetPath, string outputPath)
    {
        string tempOutputVideoPath = GetTempOutputVideoPath(targetPath);
        if (File.Exists(tempOutputVideoPath))
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            File.Move(tempOutputVideoPath, outputPath);
        }
    }

    public static void ClearTemp(string targetPath)
    {
        string tempDirectoryPath = GetTempDirectoryPath(targetPath);
        string parentDirectoryPath = Path.GetDirectoryName(tempDirectoryPath);
        if (!Globals.KeepTemp && Directory.Exists(tempDirectoryPath))
        {
            Directory.Delete(tempDirectoryPath, true);
        }
        if (Directory.Exists(parentDirectoryPath) && !Directory.EnumerateFileSystemEntries(parentDirectoryPath).Any())
        {
            Directory.Delete(parentDirectoryPath);
        }
    }

    public static bool IsFile(string filePath)
    {
        return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
    }

    public static bool IsDirectory(string directoryPath)
    {
        return !string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath);
    }

    public static bool IsAudio(this string audioPath)
    {
        string extension = Path.GetExtension(audioPath).ToLowerInvariant();

        return IsFile(audioPath) && extension.IsInIgnoreCase(".mp3", ".wav", ".aac", ".ogg");
    }
    public static bool IsInIgnoreCase(this string source, params string[] list)
    {
        if (null == source) return false;

        IEnumerable<string> en = list.Where(i => string.Compare(i, source, StringComparison.OrdinalIgnoreCase) == 0);
        return en.Count() == 0 ? false : true;
    }
    public static bool HasAudio(List<string> audioPaths)
    {
        return audioPaths.Any(IsAudio);
    }

    public static bool IsImage(this string imagePath)
    {
        string extension = Path.GetExtension(imagePath).ToLowerInvariant();

        return IsFile(imagePath) && extension.IsInIgnoreCase(".jpg", ".png", ".gif", ".bmp", ".tiff", ".tif");
    }

    public static bool HasImage(List<string> imagePaths)
    {
        return imagePaths?.Any(IsImage) == true;
    }
    public static bool IsVideo(this string videoPath)
    {
        string extension = Path.GetExtension(videoPath).ToLowerInvariant();
        return IsFile(videoPath) && extension.IsInIgnoreCase(".mp4");
    }
    public static List<string> FilterAudioPaths(List<string> paths)
    {
        return paths.Where(IsAudio).ToList();
    }

    public static List<string> FilterImagePaths(List<string> paths)
    {
        return paths.Where(IsImage).ToList();
    }

    public static string ResolveRelativePath(string path)
    {
        return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
    }

    public static List<string> ListDirectory(string directoryPath)
    {
        if (IsDirectory(directoryPath))
        {
            return Directory.GetFiles(directoryPath)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(file => !file.StartsWith(".") && !file.StartsWith("__"))
                .OrderBy(file => file)
                .ToList();
        }
        return null;
    }
}
