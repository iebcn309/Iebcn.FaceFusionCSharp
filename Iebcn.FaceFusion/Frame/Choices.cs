using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iebcn.FaceFusion.Frame
{
    public class Choices
    {
        public static string[] FaceDebuggerItems = ["bounding-box", "face-landmark-5", "face-landmark-5/68", "face-landmark-68", "face-landmark-68/5", "face-mask", "face-detector-score", "face-landmarker-score", "age", "gender"];
        public static string[] FaceEnhancerModels = ["codeformer", "gfpgan_1.2", "gfpgan_1.3", "gfpgan_1.4", "gpen_bfr_256", "gpen_bfr_512", "gpen_bfr_1024", "gpen_bfr_2048", "restoreformer_plus_plus"];
        public static string[] FaceSwapperModels = ["blendswap_256", "inswapper_128", "inswapper_128_fp16", "simswap_256", "simswap_512_unofficial", "uniface_256"];
        public static string[] FrameColorizerModels = ["ddcolor", "ddcolor_artistic", "deoldify", "deoldify_artistic", "deoldify_stable"];
        public static string[] FrameColorizerSizes = ["192x192", "256x256", "384x384", "512x512"];
        public static string[] FrameEnhancerModels = ["clear_reality_x4", "lsdir_x4", "nomos8k_sc_x4", "real_esrgan_x2", "real_esrgan_x2_fp16", "real_esrgan_x4", "real_esrgan_x4_fp16", "real_hatgan_x4", "span_kendata_x4", "ultra_sharp_x4"];

        public static string[] LipSyncerModels = ["wav2lip_gan"];
        public static string[] VideoMemoryStrategys = ["strict", "moderate", "tolerant"];
        public static string[] FaceAnalyserOrders = ["left-right", "right-left", "top-bottom", "bottom-top", "small-large", "large-small", "best-worst", "worst-best"];
        public static string[] FaceAnalyserAges = ["child", "teen", "adult", "senior"];
        public static string[] FaceAnalyserGenders = ["female", "male"];
        public static Dictionary<string, List<string>> FaceDetectorSet = new Dictionary<string, List<string>>() { { "many", ["640x640"] }, { "retinaface", ["160x160", "320x320", "480x480", "512x512", "640x640"] }, { "scrfd", ["160x160", "320x320", "480x480", "512x512", "640x640"] }, { "yoloface", ["640x640"] }, { "yunet", ["160x160", "320x320", "480x480", "512x512", "640x640", "768x768", "960x960", "1024x1024"] } };
        public static string[] FaceSelectorModes = ["many", "one", "reference"];

        public static string[] FaceMaskTypes = ["box", "occlusion", "region"];

        public static string[] FaceMaskRegions = ["skin", "left-eyebrow", "right-eyebrow", "left-eye", "right-eye", "glasses", "nose", "mouth", "upper-lip", "lower-lip"];

        public static string[] TempFrameFormats = ["bmp", "jpg", "png"];
        public static string[] OutputVideoEncoders = ["libx264", "libx265", "libvpx-vp9", "h264_nvenc", "hevc_nvenc", "h264_amf", "hevc_amf"];

        public static string[] OutputVideoPresets = ["ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow"];

        public static double[] image_template_sizes = [0.25, 0.5, 0.75, 1, 1.5, 2, 2.5, 3, 3.5, 4];

        public static int[] video_template_sizes = [240, 360, 480, 540, 720, 1080, 1440, 2160, 4320];
        public static string[] CommonOptions = ["keep-temp", "skip-audio", "skip-download"];

    }
}
