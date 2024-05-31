namespace Iebcn.FaceFusion;
public class FaceFusionConstants
{
    public static List<string> VideoMemoryStrategies = new List<string> { "strict", "moderate", "tolerant" };
    public static List<string> FaceAnalyserOrders = new List<string> { "left-right", "right-left", "top-bottom", "bottom-top", "small-large", "large-small", "best-worst", "worst-best" };
    public static List<string> FaceAnalyserAges = new List<string> { "child", "teen", "adult", "senior" };
    public static List<string> FaceAnalyserGenders = new List<string> { "female", "male" };

    public static Dictionary<string, List<string>> FaceDetectorSet = new Dictionary<string, List<string>>()
    {
        { "many", new List<string> { "640x640" } },
        { "retinaface", new List<string> { "160x160", "320x320", "480x480", "512x512", "640x640" } },
        { "scrfd", new List<string> { "160x160", "320x320", "480x480", "512x512", "640x640" } },
        { "yoloface", new List<string> { "640x640" } },
        { "yunet", new List<string> { "160x160", "320x320", "480x480", "512x512", "640x640", "768x768", "960x960", "1024x1024" } }
    };

    public static List<string> FaceSelectorModes = new List<string> { "many", "one", "reference" };
    public static List<string> FaceMaskTypes = new List<string> { "box", "occlusion", "region" };
    public static List<string> FaceMaskRegions = new List<string> { "skin", "left-eyebrow", "right-eyebrow", "left-eye", "right-eye", "glasses", "nose", "mouth", "upper-lip", "lower-lip" };
    public static List<string> TempFrameFormats = new List<string> { "bmp", "jpg", "png" };
    public static List<string> OutputVideoEncoders = new List<string> { "libx264", "libx265", "libvpx-vp9", "h264_nvenc", "hevc_nvenc", "h264_amf", "hevc_amf" };
    public static List<string> OutputVideoPresets = new List<string> { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow" };

    public static List<float> ImageTemplateSizes = new List<float> { 0.25f, 0.5f, 0.75f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
    public static List<int> VideoTemplateSizes = new List<int> { 240, 360, 480, 540, 720, 1080, 1440, 2160, 4320 };

    public static List<int> ExecutionThreadCountRange = CreateIntRange(1, 128, 1);
    public static List<int> ExecutionQueueCountRange = CreateIntRange(1, 32, 1);
    public static List<int> SystemMemoryLimitRange = CreateIntRange(0, 128, 1);
    public static List<float> FaceDetectorScoreRange = CreateFloatRange(0.0f, 1.0f, 0.05f);
    public static List<float> FaceLandmarkerScoreRange = CreateFloatRange(0.0f, 1.0f, 0.05f);
    public static List<float> FaceMaskBlurRange = CreateFloatRange(0.0f, 1.0f, 0.05f);
    public static List<int> FaceMaskPaddingRange = CreateIntRange(0, 100, 1);
    public static List<float> ReferenceFaceDistanceRange = CreateFloatRange(0.0f, 1.5f, 0.05f);
    public static List<int> OutputImageQualityRange = CreateIntRange(0, 100, 1);
    public static List<int> OutputVideoQualityRange = CreateIntRange(0, 100, 1);

    private static List<int> CreateIntRange(int start, int end, int step)
    {
        List<int> range = new List<int>();
        for (int i = start; i <= end; i += step)
        {
            range.Add(i);
        }
        return range;
    }

    private static List<float> CreateFloatRange(float start, float end, float step)
    {
        List<float> range = new List<float>();
        for (float i = start; i <= end; i += step)
        {
            range.Add(i);
        }
        return range;
    }
}
