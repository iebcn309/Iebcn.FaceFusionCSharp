using Microsoft.ML.OnnxRuntime;
using NLog;

namespace Iebcn.FaceFusion;

// 定义视频内存策略枚举
public enum VideoMemoryStrategy
{
    Strict,
    Moderate,
    Tolerant
}
public enum ProcessMode
{
    Output,
    Preview,
    Stream
}

// 定义面部选择器模式枚举
public enum FaceSelectorMode
{
    Many,
    One,
    Reference
}
/// <summary>
/// 面部分析器的分析顺序枚举
/// </summary>
public enum FaceAnalyserOrder
{
    /// <summary>
    /// 从左到右分析
    /// </summary>
    LeftToRight,

    /// <summary>
    /// 从右到左分析
    /// </summary>
    RightToLeft,

    /// <summary>
    /// 从上到下分析
    /// </summary>
    TopToBottom,

    /// <summary>
    /// 从下到上分析
    /// </summary>
    BottomToTop,

    /// <summary>
    /// 从小到大分析
    /// </summary>
    SmallToLarge,

    /// <summary>
    /// 从大到小分析
    /// </summary>
    LargeToSmall,

    /// <summary>
    /// 优先分析质量最好的面部
    /// </summary>
    BestToWorst,

    /// <summary>
    /// 优先分析质量最差的面部
    /// </summary>
    WorstToBest
}
public enum FaceAnalyserAge
{
    child,
    teen,
    adult,
    senior
}
public enum FaceAnalyserGender
{
    female,
    male,
}
public enum FaceSwapperModel
{
    blendswap_256,
    inswapper_128,
    inswapper_128_fp16,
    simswap_256,
    simswap_512_unofficial,
    uniface_256
}
// 定义面部检测器模型类
public enum FaceDetectorModel
{
    many,
    retinaface,
    scrfd,
    yoloface,
    yunet
}
public enum FaceDetectorTweak
{
    low,
    high
}
// 定义面部识别器模型类
public enum FaceRecognizerModel
{
    arcface_blendswap,
    arcface_inswapper,
    arcface_simswap,
    arcface_uniface
}
// 定义面部遮罩类型枚举
public enum FaceMaskType
{
    box,
    occlusion,
    region
}

/// <summary>
/// 面部遮罩区域枚举，用于指定面部特定区域的遮罩类型
/// </summary>
public enum FaceMaskRegion
{
    /// <summary>
    /// 皮肤区域
    /// </summary>
    Skin,

    /// <summary>
    /// 左眉毛区域
    /// </summary>
    LeftEyebrow,

    /// <summary>
    /// 右眉毛区域
    /// </summary>
    RightEyebrow,

    /// <summary>
    /// 左眼区域
    /// </summary>
    LeftEye,

    /// <summary>
    /// 右眼区域
    /// </summary>
    RightEye,

    /// <summary>
    /// 眼镜区域
    /// </summary>
    Glasses,

    /// <summary>
    /// 鼻子区域
    /// </summary>
    Nose,

    /// <summary>
    /// 嘴巴区域
    /// </summary>
    Mouth,

    /// <summary>
    /// 上唇区域
    /// </summary>
    UpperLip,

    /// <summary>
    /// 下唇区域
    /// </summary>
    LowerLip
}
public enum TempFrameFormat
{
    jpg,
    png,
    bmp
}
// 定义输出视频编码器枚举
public enum OutputVideoEncoder
{
    libx264,
    libx265,
    libvpx_vp9,
    h264_nvenc,
    hevc_nvenc,
    h264_amf,
    hevc_amf
}

// 定义输出视频预设枚举
public enum OutputVideoPreset
{
    ultrafast,
    superfast,
    veryfast,
    faster,
    fast,
    medium,
    slow,
    slower,
    veryslow
}
public enum FaceEnhancerModel
{
    codeformer,
    gfpgan_1_2,
    gfpgan_1_3,
    gfpgan_1_4,
    gpen_bfr_256,
    gpen_bfr_512,
    gpen_bfr_1024,
    gpen_bfr_2048,
    restoreformer_plus_plus
}
public enum FrameEnhancerModel
{
    clear_reality_x4,
    lsdir_x4,
    nomos8k_sc_x4,
    real_esrgan_x2,
    real_esrgan_x2_fp16,
    real_esrgan_x4,
    real_esrgan_x4_fp16,
    real_hatgan_x4,
    span_kendata_x4,
    ultra_sharp_x4
}
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
    #region general
    // 源文件路径列表
    public List<string> SourcePaths { get; set; } = new List<string>();
    // 目标文件路径
    public string TargetPath { get; set; }

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
    public FrameEnhancerModel FrameEnhancerModel { get; set; }
    public bool FaceEnhancerEnabled { get; set; }

    public FaceEnhancerModel FaceEnhancerModel { get; set; }

    private FaceSwapperModel _faceSwapperModel = FaceSwapperModel.inswapper_128;

    public FaceSwapperModel FaceSwapperModel
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
    public VideoMemoryStrategy VideoMemoryStrategy { get; set; }= VideoMemoryStrategy.Strict;

    // 系统内存限制
    public int SystemMemoryLimit { get; set; }
    #endregion

    #region face analyser
    // 面部分析器顺序
    public FaceAnalyserOrder FaceAnalyserOrder { get; set; }= FaceAnalyserOrder.LeftToRight;
    // 面部分析器年龄
    public FaceAnalyserAge FaceAnalyserAge { get; set; }
    // 面部分析器性别
    public FaceAnalyserGender FaceAnalyserGender { get; set; }
    // 面部检测器模型
    public FaceDetectorModel FaceDetectorModel { get; set; } = FaceDetectorModel.yoloface;

    // 面部检测器尺寸
    public string FaceDetectorSize { get; set; } = "640x640";
    // 面部检测器得分
    public float FaceDetectorScore { get; set; } = 0.5f;
    // 面部标记器得分
    public double FaceLandmarkerScore { get; set; } = 0.5f;
    // 面部识别器模型
    public FaceRecognizerModel FaceRecognizerModel { get; set; }
    #endregion

    #region face selector
    // 面部选择器模式
    public FaceSelectorMode FaceSelectorMode { get; set; } = FaceSelectorMode.Reference;
    // 参考面部位置
    public int ReferenceFacePosition { get; set; }
    // 参考面部相似度
    public double ReferenceFaceDistance { get; set; } = 0.6f;
    // 参考帧编号
    public int ReferenceFrameNumber { get; set; }
    #endregion

    #region face mask
    // 面部遮罩类型列表
    public List<FaceMaskType> FaceMaskTypes { get; set; } = new List<FaceMaskType>() {  FaceMaskType.box};

    // 面部遮罩模糊度
    public double FaceMaskBlur { get; set; } = 0.3;

    // 面部遮罩填充
    public Padding FaceMaskPadding { get; set; }

    // 面部遮罩区域列表
    public List<FaceMaskRegion> FaceMaskRegions { get; set; } = new List<FaceMaskRegion>();
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
    public OutputVideoEncoder OutputVideoEncoder { get; set; } = OutputVideoEncoder.libx264;

    // 输出视频预设
    public OutputVideoPreset OutputVideoPreset { get; set; }= OutputVideoPreset.veryfast;

    // 输出视频质量
    public int OutputVideoQuality { get; set; } = 80;

    // 输出视频分辨率
    public string OutputVideoResolution { get; set; }

    // 输出视频帧率
    public float OutputVideoFps { get; set; }

    // 是否跳过音频
    public bool SkipAudio { get; set; }
    #endregion

    #region frame processors
    // 帧处理器列表
    public List<string> FrameProcessors { get; set; } = new List<string>();
    #endregion

    #region frame processors
    // UI布局列表
    public List<string> UiLayouts { get; set; } = new List<string>();
    public int FaceEnhancerBlend { get; set; } = 80;
    public int FrameEnhancerBlend { get; set; } = 80;
    #endregion

}
