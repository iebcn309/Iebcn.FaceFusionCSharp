using OpenCvSharp;

namespace Iebcn.FaceFusion;
public class Vision
{
    // 从文件路径中读取静态图像
    public static Mat ReadStaticImage(string imagePath)
    {
        return ReadImage(imagePath);
    }
    // 从多个文件路径中读取静态图像
    public static List<Mat> ReadStaticImages(List<string> imagePaths)
    {
        List<Mat> frames = new List<Mat>();
        if (imagePaths != null)
        {
            foreach (var imagePath in imagePaths)
            {
                frames.Add(ReadStaticImage(imagePath));
            }
        }
        return frames;
    }
    // 读取图像
    public static Mat ReadImage(string imagePath)
    {
        if (imagePath.IsImage())
        {
            return new Mat(imagePath);
        }
        return null;
    }
    // 写入图像
    public static bool WriteImage(string imagePath, Mat visionFrame)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            return visionFrame.ImWrite(imagePath);
        }
        return false;
    }

    // 检测图像分辨率
    public static Size? DetectImageResolution(string imagePath)
    {
        if (imagePath.IsImage())
        {
            Mat image = ReadImage(imagePath);
            return new Size(image.Cols, image.Rows);
        }
        return null;
    }
    // 限制图像分辨率
    public static Size RestrictImageResolution(string imagePath, Size resolution)
    {
        Size imageResolution = DetectImageResolution(imagePath) ?? new Size(0, 0);
        return new Size(Math.Min(imageResolution.Width, resolution.Width), Math.Min(imageResolution.Height, resolution.Height));
    }
    // 获取视频帧
    public static Mat GetVideoFrame(string videoPath, int frameNumber = 0)
    {
        if (videoPath.IsVideo())
        {
            using (VideoCapture videoCapture = new VideoCapture(videoPath))
            {
                if (videoCapture.IsOpened())
                {
                    int frameTotal = (int)videoCapture.Get(VideoCaptureProperties.FrameCount);
                    videoCapture.Set(VideoCaptureProperties.PosFrames, Math.Min(frameTotal, frameNumber - 1));
                    Mat visionFrame = new Mat();
                    if (videoCapture.Read(visionFrame))
                    {
                        return visionFrame;
                    }
                }
            }
        }
        return null;
    }
    // 创建图像分辨率列表
    public static List<string> CreateImageResolutions(Size resolution)
    {
        List<string> resolutions = new List<string>();
        List<Size> tempResolutions = new List<Size>();
        if (resolution != null)
        {
            int width = resolution.Width;
            int height = resolution.Height;
            tempResolutions.Add(NormalizeResolution(resolution));
            foreach (var templateSize in FaceFusionConstants.ImageTemplateSizes)
            {
                tempResolutions.Add(NormalizeResolution(new Size(width * templateSize, height * templateSize)));
            }
            tempResolutions = tempResolutions.Distinct().ToList();
            foreach (var tempResolution in tempResolutions)
            {
                resolutions.Add(PackResolution(tempResolution));
            }
        }
        return resolutions;
    }

    // 计算视频帧总数
    public static int CountVideoFrameTotal(string videoPath)
    {
        if (videoPath.IsVideo())
        {
            using (VideoCapture videoCapture = new VideoCapture(videoPath))
            {
                if (videoCapture.IsOpened())
                {
                    int videoFrameTotal = (int)videoCapture.Get(VideoCaptureProperties.FrameCount);
                    return videoFrameTotal;
                }
            }
        }
        return 0;
    }

    // 检测视频帧率
    public static float DetectVideoFps(string videoPath)
    {
        if (videoPath.IsVideo())
        {
            using (VideoCapture videoCapture = new VideoCapture(videoPath))
            {
                if (videoCapture.IsOpened())
                {
                    float videoFps = (float)videoCapture.Get(VideoCaptureProperties.Fps);
                    return videoFps;
                }
            }
        }
        return 0;
    }

    // 限制视频帧率
    public static float RestrictVideoFps(string videoPath, float fps)
    {
        if (videoPath.IsVideo())
        {
            float videoFps = DetectVideoFps(videoPath);
            if (videoFps < fps)
            {
                return videoFps;
            }
        }
        return fps;
    }

    // 检测视频分辨率
    public static Size? DetectVideoResolution(string videoPath)
    {
        if (videoPath.IsVideo())
        {
            using (VideoCapture videoCapture = new VideoCapture(videoPath))
            {
                if (videoCapture.IsOpened())
                {
                    double width = videoCapture.Get(VideoCaptureProperties.FrameWidth);
                    double height = videoCapture.Get(VideoCaptureProperties.FrameHeight);
                    return new Size((int)width, (int)height);
                }
            }
        }
        return null;
    }
    // 限制视频分辨率
    public static Size RestrictVideoResolution(string videoPath, Size resolution)
    {
        if (videoPath.IsVideo())
        {
            Size videoResolution = DetectVideoResolution(videoPath) ?? new Size(0, 0);
            return new Size(Math.Min(videoResolution.Width, resolution.Width), Math.Min(videoResolution.Height, resolution.Height));
        }
        return resolution;
    }

    // 创建视频分辨率列表
    public static List<string> CreateVideoResolutions(Size resolution)
    {
        List<string> resolutions = new List<string>();
        List<Size> tempResolutions = new List<Size>();
        if (resolution != null)
        {
            int width = resolution.Width;
            int height = resolution.Height;
            tempResolutions.Add(NormalizeResolution(resolution));
            foreach (var templateSize in FaceFusionConstants.VideoTemplateSizes)
            {
                if (width > height)
                {
                    tempResolutions.Add(NormalizeResolution(new Size((int)(templateSize * width / height), (int)templateSize)));
                }
                else
                {
                    tempResolutions.Add(NormalizeResolution(new Size((int)templateSize, (int)(templateSize * height / width))));
                }
            }
            tempResolutions = tempResolutions.Distinct().ToList();
            foreach (var tempResolution in tempResolutions)
            {
                resolutions.Add(PackResolution(tempResolution));
            }
        }
        return resolutions;
    }

    // 规范化分辨率
    public static Size NormalizeResolution(Size resolution)
    {
        int width = resolution.Width;
        int height = resolution.Height;
        if (width > 0 && height > 0)
        {
            int normalizeWidth = (int)Math.Round(width / 2d) * 2;
            int normalizeHeight = (int)Math.Round(height / 2d) * 2;
            return new Size(normalizeWidth, normalizeHeight);
        }
        return new Size(0, 0);
    }

    // 封装分辨率
    public static string PackResolution(Size resolution)
    {
        Size normalizedResolution = NormalizeResolution(resolution);
        return $"{normalizedResolution.Width}x{normalizedResolution.Height}";
    }

    // 解析分辨率
    public static Size UnpackResolution(string resolution)
    {
        string[] parts = resolution.Split('x');
        if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
        {
            return new Size(width, height);
        }
        return new Size(0, 0);
    }

    // 调整帧分辨率
    public static Mat ResizeFrameResolution(Mat visionFrame, Size maxResolution)
    {
        int height = visionFrame.Rows;
        int width = visionFrame.Cols;
        int maxWidth = maxResolution.Width;
        int maxHeight = maxResolution.Height;
        if (height > maxHeight || width > maxWidth)
        {
            double scale = Math.Min((double)maxHeight / height, (double)maxWidth / width);
            int newWidth = (int)(width * scale);
            int newHeight = (int)(height * scale);
            return visionFrame.Resize(new Size(newWidth, newHeight));
        }
        return visionFrame;
    }

    // 规范化帧颜色
    public static Mat NormalizeFrameColor(Mat visionFrame)
    {
        return visionFrame.CvtColor(ColorConversionCodes.BGR2RGB);
    }
    // 创建分块帧
    public static (List<Mat>, int, int) CreateTileFrames(Mat visionFrame, Size size)
    {
        Mat paddedVisionFrame = new Mat();
        Cv2.CopyMakeBorder(visionFrame, paddedVisionFrame, size.Height, size.Height, size.Width, size.Width, BorderTypes.Constant, new Scalar(0, 0, 0));

        int tileWidth = size.Width - 2 * size.Height;
        int padSizeBottom = size.Height + tileWidth - paddedVisionFrame.Rows % tileWidth;
        int padSizeRight = size.Height + tileWidth - paddedVisionFrame.Cols % tileWidth;
        Mat padVisionFrame = new Mat();
        Cv2.CopyMakeBorder(paddedVisionFrame, padVisionFrame, size.Height, padSizeBottom, size.Width, padSizeRight, BorderTypes.Constant, new Scalar(0, 0, 0));

        int padHeight = padVisionFrame.Rows;
        int padWidth = padVisionFrame.Cols;

        List<Mat> tileVisionFrames = new List<Mat>();
        for (int row = size.Height; row < padHeight - size.Height; row += tileWidth)
        {
            int top = row - size.Height;
            int bottom = row + size.Height + tileWidth;
            for (int col = size.Height; col < padWidth - size.Height; col += tileWidth)
            {
                int left = col - size.Height;
                int right = col + size.Height + tileWidth;
                Mat tile = padVisionFrame[new Rect(left, top, right - left, bottom - top)];
                tileVisionFrames.Add(tile.Clone());
            }
        }

        return (tileVisionFrames, padWidth, padHeight);
    }

    // 合并分块帧
    public static Mat MergeTileFrames(List<Mat> tileVisionFrames, int tempWidth, int tempHeight, int padWidth, int padHeight, Size size)
    {
        Mat mergeVisionFrame = new Mat(new Size(padWidth, padHeight), MatType.CV_8UC3, new Scalar(0, 0, 0));

        int tileWidth = tileVisionFrames[0].Cols - 2 * size.Height;
        int tilesPerRow = Math.Min(padWidth / tileWidth, tileVisionFrames.Count);

        for (int index = 0; index < tileVisionFrames.Count; index++)
        {
            Mat tileVisionFrame = tileVisionFrames[index];
            tileVisionFrame = tileVisionFrame[new Rect(size.Height, size.Height, tileVisionFrame.Cols - 2 * size.Height, tileVisionFrame.Rows - 2 * size.Height)];

            int rowIndex = index / tilesPerRow;
            int colIndex = index % tilesPerRow;

            int top = rowIndex * tileVisionFrame.Rows;
            int bottom = top + tileVisionFrame.Rows;
            int left = colIndex * tileVisionFrame.Cols;
            int right = left + tileVisionFrame.Cols;

            Mat roi = new Mat(mergeVisionFrame, new Rect(left, top, right - left, bottom - top));
            tileVisionFrame.CopyTo(roi);
        }

        return new Mat(mergeVisionFrame, new Rect(size.Width, size.Width, tempWidth, tempHeight));
    }

    internal static Dictionary<string, List<Face>> GetReferenceFaces()
    {
        throw new NotImplementedException();
    }


}
