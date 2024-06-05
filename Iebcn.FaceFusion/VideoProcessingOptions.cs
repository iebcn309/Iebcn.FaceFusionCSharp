using Microsoft.ML.OnnxRuntime;
using NLog;

namespace Iebcn.FaceFusion;

// 定义填充类
public class Padding
{
    public int Top { get; set; }
    public int Bottom { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
}
// 定义视频处理配置类
public class VideoProcessingOption
{
    public static VideoProcessingOption Instance = new VideoProcessingOption();
    #region general
    // 源文件路径列表
    public List<string> SourcePaths { get; set; } = new List<string>();
    // 目标文件路径
    public string TargetPath { get; set; } = ".";

    // 输出文件路径
    public string OutputPath { get; set; }
    #endregion
    #region misc
    // 是否强制下载
    public bool? ForceDownload { get; set; }

    // 是否跳过下载
    public bool SkipDownload { get; set; }
    // 是否无头模式运行
    public bool? Headless { get; set; }

    // 日志级别
    public LogLevel LogLevel { get; set; } = LogLevel.Info;
    #endregion
    public bool FrameEnhancerEnabled { get; set; }
    public string FrameEnhancerModel { get; set; }
    public bool FaceEnhancerEnabled { get; set; }

    public string FaceEnhancerModel { get; set; }

    private string _faceSwapperModel = "inswapper_128";

    public string FaceSwapperModel
    {
        get { return _faceSwapperModel; }
        set
        {
            _faceSwapperModel = value;
            //NotifyPropertyChanged("FaceSwapperModel");
        }
    } 

    #region execution
    public string ExecutionDeviceId { get; set; }
    // 执行提供者列表
    public List<string> ExecutionProviders { get; set; } = OrtEnv.Instance().GetAvailableProviders().ToList();
    // 执行线程数量
    public int ExecutionThreadCount { get; set; } = 4;
    // 执行队列数量
    public int ExecutionQueueCount { get; set; } = 1;
    #endregion

    #region memory
    // 视频内存策略
    public string VideoMemoryStrategy { get; set; }= "strict";

    // 系统内存限制
    public int SystemMemoryLimit { get; set; }
    #endregion

    #region face analyser
    // 面部分析器顺序
    public string FaceAnalyserOrder { get; set; }
    // 面部分析器年龄
    public string FaceAnalyserAge { get; set; }
    // 面部分析器性别
    public string FaceAnalyserGender { get; set; }
    // 面部检测器模型
    public string FaceDetectorModel { get; set; } = "yoloface";

    // 面部检测器尺寸
    public string FaceDetectorSize { get; set; } = "640x640";
    // 面部检测器得分
    public decimal FaceDetectorScore { get; set; } = 0.5m;
    // 面部标记器得分
    public decimal FaceLandmarkerScore { get; set; } = 0.5m;
    // 面部识别器模型
    public string FaceRecognizerModel { get; set; }
    #endregion

    #region face selector
    // 面部选择器模式
    public string FaceSelectorMode { get; set; } = "reference";
    // 参考面部位置
    public int ReferenceFacePosition { get; set; }
    // 参考面部相似度
    public decimal ReferenceFaceDistance { get; set; } = 0.6m;
    // 参考帧编号
    public int ReferenceFrameNumber { get; set; }
    #endregion

    #region face mask
    // 面部遮罩类型列表
    public List<string> FaceMaskTypes { get; set; } = new List<string>() { "box" };

    // 面部遮罩模糊度
    public decimal FaceMaskBlur { get; set; } = 0.3m;

    // 面部遮罩填充
    public Padding FaceMaskPadding { get; set; }=new Padding();

    // 面部遮罩区域列表
    public List<string> FaceMaskRegions { get; set; } = new List<string>();
    #endregion

    #region frame extraction
    // 剪辑开始帧
    public int TrimFrameStart { get; set; }
    // 剪辑结束帧
    public int TrimFrameEnd { get; set; }
    // 临时帧格式
    public string TempFrameFormat { get; set; } = "png";
    // 是否保留临时文件
    public bool KeepTemp { get; set; }
    #endregion

    #region output creation
    // 输出图像质量
    public int OutputImageQuality { get; set; } = 80;

    // 输出图像分辨率
    public string OutputImageResolution { get; set; }

    // 输出视频编码器
    public string OutputVideoEncoder { get; set; } = "libx264";

    // 输出视频预设
    public string OutputVideoPreset { get; set; }= "veryfast";

    // 输出视频质量
    public int OutputVideoQuality { get; set; } = 80;

    // 输出视频分辨率
    public string OutputVideoResolution { get; set; }

    // 输出视频帧率
    public decimal OutputVideoFps { get; set; }

    // 是否跳过音频
    public bool SkipAudio { get; set; }
    #endregion

    #region frame processors
    // 帧处理器列表
    public List<string> FrameProcessors { get; set; } = new List<string>() { "FaceSwapper" };
    #endregion

    #region frame processors
    // UI布局列表
    public List<string> UiLayouts { get; set; } = new List<string>();
    public int FaceEnhancerBlend { get; set; } = 80;
    public int FrameEnhancerBlend { get; set; } = 80;
    public List<string> FaceDebuggerItems { get; set; }
    public string FrameColorizerModel { get; set; }
    public int FrameColorizerBlend { get; set; }
    public string FrameColorizerSize { get; set; }
    public string LipSyncerModel { get; set; }
    #endregion

}
