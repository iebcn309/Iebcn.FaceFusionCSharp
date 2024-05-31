using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Iebcn.FaceFusion.Utility
{
	public class Config
	{
		public static readonly object Logger;
		public static string rootdir = Directory.GetCurrentDirectory().Replace('\\', '/');
		public static string TEMP_DIR = Path.Combine(rootdir, "tmp").Replace('\\', '/');
		public static string homedir = Directory.GetCurrentDirectory().Replace('\\', '/');
		public static string LOGS_DIR = Path.Combine(rootdir, "logs");
		public static string LOG_FILENAME = $"video-{DateTime.Now.ToString("yyyyMMdd")}.log";
		public static string LOG_FILEPATH = Path.Combine(LOGS_DIR, LOG_FILENAME);
		public static string SETTING_FILE = Path.Combine(rootdir, "videotrans/set.ini");
		public static string LANG_DIR = Path.Combine(rootdir, "videotrans/language");
		public static string DEFAULT_LANG = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
		public static Dictionary<string, List<string>> langCodeList = new Dictionary<string, List<string>>()
				{
					{ "中文简", new List<string> { "zh-cn", "chi", "zh", "ZH", "zh" } },
					{ "中文繁", new List<string> { "zh-tw", "chi", "cht", "ZH", "zh-TW" } },
					{ "英语", new List<string> { "en", "eng", "en", "EN-US", "en" } },
					{ "法语", new List<string> { "fr", "fre", "fra", "FR", "fr" } },
					{ "德语", new List<string> { "de", "ger", "de", "DE", "de" } },
					{ "日语", new List<string> { "ja", "jpn", "jp", "JA", "ja" } },
					{ "韩语", new List<string> { "ko", "kor", "kor", "KO", "ko" } },
					{ "俄语", new List<string> { "ru", "rus", "ru", "RU", "ru" } },
					{ "西班牙语", new List<string> { "es", "spa", "spa", "ES", "es" } },
					{ "泰国语", new List<string> { "th", "tha", "th", "No", "th" } },
					{ "意大利语", new List<string> { "it", "ita", "it", "IT", "it" } },
					{ "葡萄牙语", new List<string> { "pt", "por", "pt", "PT", "pt" } },
					{ "越南语", new List<string> { "vi", "vie", "vie", "No", "vi" } },
					{ "阿拉伯语", new List<string> { "ar", "are", "ara", "No", "ar" } },
					{ "土耳其语", new List<string> { "tr", "tur", "tr", "tr", "tr" } },
					{ "印度语", new List<string> { "hi", "hin", "hi", "No", "hi" } }
				};
		public static string CurrentStatus = "stop";
		public static Dictionary<string, object> Transobj;
		public static string[] openaiTTS_rolelist = { "alloy", "echo", "fable", "onyx", "nova", "shimmer" };
		//public static string[] chatgpt_model_list = Settings["chatgpt_model"]?.ToString().Split(',').Select(s => s.Trim()).ToArray();
		public static Dictionary<string, object> Settings = new Dictionary<string, object>()
				{
					{"source_mp4", ""},
					{"target_dir", ""},
					{"source_language", "en"},
					{"detect_language", "en"},
					{"target_language", "zh-cn"},
					{"subtitle_language", "chi"},
					{"cuda", false},
					{"is_separate", false},
					{"voice_role", "No"},
					{"voice_rate", "+0%"},
					{"listen_text_zh-cn", "你好啊，我亲爱的朋友，希望你的每一天都是美好愉快的！"},
					{"listen_text_zh-tw", "你好啊，我親愛的朋友，希望你的每一天都是美好愉快的！"},
					{"listen_text_en", "Hello, my dear friend. I hope your every day is beautiful and enjoyable!"},
					{"listen_text_fr", "Bonjour mon cher ami. J'espère que votre quotidien est beau et agréable !"},
					{"listen_text_de", "Hallo mein lieber Freund. Ich hoffe, dass Ihr Tag schön und angenehm ist!"},
					{"listen_text_ja", "こんにちは私の親愛なる友人。 あなたの毎日が美しく楽しいものでありますように！"},
					{"listen_text_ko", "안녕, 내 사랑하는 친구. 당신의 매일이 아름답고 즐겁기를 바랍니다!"},
					{"listen_text_ru", "Привет, мой дорогой друг. Желаю, чтобы каждый твой день был прекрасен и приятен!"},
					{"listen_text_es", "Hola mi querido amigo. ¡Espero que cada día sea hermoso y agradable!"},
					{"listen_text_th", "สวัสดีเพื่อนรัก. ฉันหวังว่าทุกวันของคุณจะสวยงามและสนุกสนาน!"},
					{"listen_text_it", "Ciao caro amico mio. Spero che ogni tuo giorno sia bello e divertente!"},
					{"listen_text_pt", "Olá meu querido amigo. Espero que todos os seus dias sejam lindos e agradáveis!"},
					{"listen_text_vi", "Xin chào người bạn thân yêu của tôi. Tôi hy vọng mỗi ngày của bạn đều đẹp và thú vị!"},
					{"listen_text_ar", "مرحبا صديقي العزيز. أتمنى أن يكون كل يوم جميلاً وممتعًا!"},
					{"listen_text_tr", "Merhaba sevgili arkadaşım. Umarım her gününüz güzel ve keyifli geçer!"},
					{"listen_text_hi", "नमस्ते मेरे प्यारे दोस्त। मुझे आशा है कि आपका हर दिन सुंदर और आनंददायक हो!!"},
					{"listen_text_hu", "Helló kedves barátom. Remélem minden napod szép és kellemes!"},
					{"tts_type", "edgeTTS"},
					{"tts_type_list", new List<string>{"edgeTTS", "GPT-SoVITS", "clone-voice", "openaiTTS", "elevenlabsTTS", "TTS-API"}},
					{"whisper_type", "all"},
					{"whisper_model", "base"},
					{"model_type", "faster"},
					{"only_video", false},
					{"translate_type", "google"},
					{"subtitle_type", 0},
					{"voice_autorate", false},
					{"auto_ajust", true},
					{"deepl_authkey", ""},
					{"deepl_api", ""},
					{"deeplx_address", ""},
					{"ott_address", ""},
					{"tencent_SecretId", ""},
					{"tencent_SecretKey", ""},
					{"baidu_appid", ""},
					{"baidu_miyue", ""},
					{"coquitts_role", ""},
					{"coquitts_key", ""},
					{"elevenlabstts_role", new List<string>()},
					{"elevenlabstts_key", ""},
					{"clone_api", ""},
					{"chatgpt_api", ""},
					{"chatgpt_key", ""},
					{"chatgpt_model", ""},
					{"chatgpt_template", ""},
					{"azure_api", ""},
					{"azure_key", ""},
					{"azure_model", "gpt-3.5-turbo"},
					{"azure_template", ""},
					{"openaitts_role", string.Join(",", openaiTTS_rolelist)},
					{"gemini_key", ""}
				};
        public static Dictionary<string, string> Wording = new Dictionary<string, string>
		{
			{"conda_not_activated", "Conda is not activated"},
			{"python_not_supported", "Python version is not supported, upgrade to {version} or higher"},
			{"ffmpeg_not_installed", "FFMpeg is not installed"},
			{"creating_temp", "Creating temporary resources"},
			{"extracting_frames", "Extracting frames with a resolution of {resolution} and {fps} frames per second"},
			{"extracting_frames_succeed", "Extracting frames succeed"},
			{"extracting_frames_failed", "Extracting frames failed"},
			{"analysing", "Analysing"},
			{"processing", "Processing"},
			{"downloading", "Downloading"},
			{"temp_frames_not_found", "Temporary frames not found"},
			{"copying_image", "Copying image with a resolution of {resolution}"},
			{"copying_image_succeed", "Copying image succeed"},
			{"copying_image_failed", "Copying image failed"},
			{"finalizing_image", "Finalizing image with a resolution of {resolution}"},
			{"finalizing_image_succeed", "Finalizing image succeed"},
			{"finalizing_image_skipped", "Finalizing image skipped"},
			{"merging_video", "Merging video with a resolution of {resolution} and {fps} frames per second"},
			{"merging_video_succeed", "Merging video succeed"},
			{"merging_video_failed", "Merging video failed"},
			{"skipping_audio", "Skipping audio"},
			{"restoring_audio_succeed", "Restoring audio succeed"},
			{"restoring_audio_skipped", "Restoring audio skipped"},
			{"clearing_temp", "Clearing temporary resources"},
			{"processing_stopped", "Processing stopped"},
			{"processing_image_succeed", "Processing to image succeed in {seconds} seconds"},
			{"processing_image_failed", "Processing to image failed"},
			{"processing_video_succeed", "Processing to video succeed in {seconds} seconds"},
			{"processing_video_failed", "Processing to video failed"},
			{"model_download_not_done", "Download of the model is not done"},
			{"model_file_not_present", "File of the model is not present"},
			{"select_image_source", "Select a image for source path"},
			{"select_audio_source", "Select a audio for source path"},
			{"select_video_target", "Select a video for target path"},
			{"select_image_or_video_target", "Select a image or video for target path"},
			{"select_file_or_directory_output", "Select a file or directory for output path"},
			{"no_source_face_detected", "No source face detected"},
			{"frame_processor_not_loaded", "Frame processor {frame_processor} could not be loaded"},
			{"frame_processor_not_implemented", "Frame processor {frame_processor} not implemented correctly"},
			{"ui_layout_not_loaded", "UI layout {ui_layout} could not be loaded"},
			{"ui_layout_not_implemented", "UI layout {ui_layout} not implemented correctly"},
			{"stream_not_loaded", "Stream {stream_mode} could not be loaded"},
			{"point", "."},
			{"comma", ","},
			{"colon", ":"},
			{"question_mark", "?"},
			{"exclamation_mark", "!"},
			{"help", "Help"},
			    // 安装程序
			{ "install_dependency", "选择要安装的{dependency}变体" },
			{ "skip_conda", "跳过conda环境检查" },

			// 通用
			{ "source", "选择单个或多个源图像或音频" },
			{ "target", "选择单个目标图像或视频" },
			{ "output", "指定输出文件或目录" },

			// 杂项
			{ "force_download", "强制自动下载并退出" },
			{ "skip_download", "省略自动下载和远程查找" },
			{ "headless", "在无用户界面的情况下运行程序" },
			{ "log_level", "调整终端中显示的消息严重性" },

			// 执行
			{ "execution_providers", "使用不同的提供程序加速模型推理（选项：{choices}，...）" },
			{ "execution_thread_count", "指定处理过程中的并行线程数" },
			{ "execution_queue_count", "指定每个线程处理的帧数" },

			// 内存
			{ "video_memory_strategy", "平衡快速帧处理和低VRAM使用" },
			{ "system_memory_limit", "限制处理过程中可用的RAM量" },

			// 面部分析器
			{ "face_analyser_order", "指定面部分析器检测人脸的顺序" },
			{ "face_analyser_age", "根据年龄过滤检测到的人脸" },
			{ "face_analyser_gender", "根据性别过滤检测到的人脸" },
			{ "face_detector_model", "选择负责检测人脸的模型" },
			{ "face_detector_size", "指定提供给人脸检测器的帧大小" },
			{ "face_detector_score", "根据置信度分数过滤检测到的人脸" },
			{ "face_landmarker_score", "根据置信度分数过滤检测到的地标" },

			// 面部选择器
			{ "face_selector_mode", "使用基于参考的跟踪或简单匹配" },
			{ "reference_face_position", "指定用于创建参考人脸的位置" },
			{ "reference_face_distance", "指定参考人脸与目标人脸之间的期望相似度" },
			{ "reference_frame_number", "指定用于创建参考人脸的帧" },

			// 面部面具
			{ "face_mask_types", "混合和匹配不同的面部面具类型（选项：{choices}）" },
			{ "face_mask_blur", "指定应用于盒子掩模的模糊程度" },
			{ "face_mask_padding", "对盒子掩模应用顶部、右侧、底部和左侧填充" },
			{ "face_mask_regions", "选择用于区域掩模的面部特征（选项：{choices}）" },

			// 帧提取
			{ "trim_frame_start", "指定目标视频的起始帧" },
			{ "trim_frame_end", "指定目标视频的结束帧" },
			{ "temp_frame_format", "指定临时资源格式" },
			{ "keep_temp", "处理后保留临时资源" },

			// 输出创建
			{ "output_image_quality", "指定图像质量，即压缩因子" },
			{ "output_image_resolution", "基于目标图像指定图像输出分辨率" },
			{ "output_video_encoder", "指定用于视频压缩的编码器" },
			{ "output_video_preset", "平衡快速视频处理和视频文件大小" },
			{ "output_video_quality", "指定视频质量，即压缩因子" },
			{ "output_video_resolution", "基于目标视频指定视频输出分辨率" },
			{ "output_video_fps", "基于目标视频指定视频输出帧速率" },
			{ "skip_audio", "省略目标视频中的音频" },

			// 帧处理器
			{ "frame_processors", "加载单个或多个帧处理器（选项：{choices}，...）" },
			{ "face_debugger_items", "加载单个或多个帧处理器（选项：{choices}）" },
			{ "face_enhancer_model", "选择负责增强人脸的模型" },
			{ "face_enhancer_blend", "将增强的人脸与之前的脸部混合" },
			{ "face_swapper_model", "选择负责交换人脸的模型" },
			{ "frame_colorizer_model", "选择负责着色帧的模型" },
			{ "frame_colorizer_blend", "将着色帧与之前的帧混合" },
			{ "frame_enhancer_model", "选择负责增强帧的模型" },
			{ "frame_enhancer_blend", "将增强的帧与之前的帧混合" },
			{ "lip_syncer_model", "选择负责同步嘴唇的模型" },

			// UI布局
			{ "ui_layouts", "启动单个或多个UI布局（选项：{choices}，...）" }
        };

        public static Dictionary<string, string> UIEn = new Dictionary<string, string>
			{
				["start_button"] = "START",
				["stop_button"] = "STOP",
				["clear_button"] = "CLEAR",
				["donate_button"] = "DONATE",
				["benchmark_results_dataframe"] = "BENCHMARK RESULTS",
				["benchmark_runs_checkbox_group"] = "BENCHMARK RUNS",
				["benchmark_cycles_slider"] = "BENCHMARK CYCLES",
				["common_options_checkbox_group"] = "OPTIONS",
				["execution_providers_checkbox_group"] = "EXECUTION PROVIDERS",
				["execution_queue_count_slider"] = "EXECUTION QUEUE COUNT",
				["execution_thread_count_slider"] = "EXECUTION THREAD COUNT",
				["face_analyser_order_dropdown"] = "FACE ANALYSER ORDER",
				["face_analyser_age_dropdown"] = "FACE ANALYSER AGE",
				["face_analyser_gender_dropdown"] = "FACE ANALYSER GENDER",
				["face_detector_model_dropdown"] = "FACE DETECTOR MODEL",
				["face_detector_size_dropdown"] = "FACE DETECTOR SIZE",
				["face_detector_score_slider"] = "FACE DETECTOR SCORE",
				["face_landmarker_score_slider"] = "FACE LANDMARKER SCORE",
				["face_mask_types_checkbox_group"] = "FACE MASK TYPES",
				["face_mask_blur_slider"] = "FACE MASK BLUR",
				["face_mask_padding_top_slider"] = "FACE MASK PADDING TOP",
				["face_mask_padding_right_slider"] = "FACE MASK PADDING RIGHT",
				["face_mask_padding_bottom_slider"] = "FACE MASK PADDING BOTTOM",
				["face_mask_padding_left_slider"] = "FACE MASK PADDING LEFT",
				["face_mask_region_checkbox_group"] = "FACE MASK REGIONS",
				["face_selector_mode_dropdown"] = "FACE SELECTOR MODE",
				["reference_face_gallery"] = "REFERENCE FACE",
				["reference_face_distance_slider"] = "REFERENCE FACE DISTANCE",
				["frame_processors_checkbox_group"] = "FRAME PROCESSORS",
				["face_debugger_items_checkbox_group"] = "FACE DEBUGGER ITEMS",
				["face_enhancer_model_dropdown"] = "FACE ENHANCER MODEL",
				["face_enhancer_blend_slider"] = "FACE ENHANCER BLEND",
				["face_swapper_model_dropdown"] = "FACE SWAPPER MODEL",
				["frame_colorizer_model_dropdown"] = "FRAME COLORIZER MODEL",
				["frame_colorizer_blend_slider"] = "FRAME COLORIZER BLEND",
				["frame_colorizer_size_dropdown"] = "FRAME COLORIZER SIZE",
				["frame_enhancer_model_dropdown"] = "FRAME ENHANCER MODEL",
				["frame_enhancer_blend_slider"] = "FRAME ENHANCER BLEND",
				["lip_syncer_model_dropdown"] = "LIP SYNCER MODEL",
				["video_memory_strategy_dropdown"] = "VIDEO MEMORY STRATEGY",
				["system_memory_limit_slider"] = "SYSTEM MEMORY LIMIT",
				["output_image_or_video"] = "OUTPUT",
				["output_path_textbox"] = "OUTPUT PATH",
				["output_image_quality_slider"] = "OUTPUT IMAGE QUALITY",
				["output_image_resolution_dropdown"] = "OUTPUT IMAGE RESOLUTION",
				["output_video_encoder_dropdown"] = "OUTPUT VIDEO ENCODER",
				["output_video_preset_dropdown"] = "OUTPUT VIDEO PRESET",
				["output_video_quality_slider"] = "OUTPUT VIDEO QUALITY",
				["output_video_resolution_dropdown"] = "OUTPUT VIDEO RESOLUTION",
				["output_video_fps_slider"] = "OUTPUT VIDEO FPS",
				["preview_image"] = "PREVIEW",
				["preview_frame_slider"] = "PREVIEW FRAME",
				["source_file"] = "SOURCE",
				["target_file"] = "TARGET",
				["temp_frame_format_dropdown"] = "TEMP FRAME FORMAT",
				["trim_frame_start_slider"] = "TRIM FRAME START",
				["trim_frame_end_slider"] = "TRIM FRAME END",
				["webcam_image"] = "WEBCAM",
				["webcam_mode_radio"] = "WEBCAM MODE",
				["webcam_resolution_dropdown"] = "WEBCAM RESOLUTION",
				["webcam_fps_slider"] = "WEBCAM FPS"
			};
        public static Dictionary<string, string> UICn = new Dictionary<string, string>
        {
            // general  
            ["start_button"] = "开始",
            ["stop_button"] = "停止",
            ["clear_button"] = "清除",
            // about  
            ["donate_button"] = "捐赠",
            // benchmark  
            ["benchmark_results_dataframe"] = "基准测试结果",
            // benchmark options  
            ["benchmark_runs_checkbox_group"] = "基准测试运行",
            ["benchmark_cycles_slider"] = "基准测试周期",
            // common options  
            ["common_options_checkbox_group"] = "选项",
            // execution  
            ["execution_providers_checkbox_group"] = "执行提供者",
            // execution queue count  
            ["execution_queue_count_slider"] = "执行队列计数",
            // execution thread count  
            ["execution_thread_count_slider"] = "执行线程数",
            // face analyser  
            ["face_analyser_order_dropdown"] = "面部分析器顺序",
            ["face_analyser_age_dropdown"] = "面部分析器年龄",
            ["face_analyser_gender_dropdown"] = "面部分析器性别",
            ["face_detector_model_dropdown"] = "面部检测器模型",
            ["face_detector_size_dropdown"] = "面部检测器大小",
            ["face_detector_score_slider"] = "面部检测器得分",
            ["face_landmarker_score_slider"] = "面部标记器得分",
            // face masker  
            ["face_mask_types_checkbox_group"] = "面部遮罩类型",
            ["face_mask_blur_slider"] = "面部遮罩模糊度",
            ["face_mask_padding_top_slider"] = "面部遮罩上填充",
            ["face_mask_padding_right_slider"] = "面部遮罩右填充",
            ["face_mask_padding_bottom_slider"] = "面部遮罩下填充",
            ["face_mask_padding_left_slider"] = "面部遮罩左填充",
            ["face_mask_region_checkbox_group"] = "面部遮罩区域",
            // face selector  
            ["face_selector_mode_dropdown"] = "面部选择器模式",
            ["reference_face_gallery"] = "参考面部",
            ["reference_face_distance_slider"] = "参考面部距离",
            // frame processors  
            ["frame_processors_checkbox_group"] = "帧处理器",
            // frame processors options  
            ["face_debugger_items_checkbox_group"] = "面部调试器项目",
            ["face_enhancer_model_dropdown"] = "面部增强器模型",
            ["face_enhancer_blend_slider"] = "面部增强器混合度",
            ["face_swapper_model_dropdown"] = "面部交换器模型",
            ["frame_colorizer_model_dropdown"] = "帧彩色器模型",
            ["frame_colorizer_blend_slider"] = "帧彩色器混合度",
            ["frame_colorizer_size_dropdown"] = "帧彩色器大小",
            ["frame_enhancer_model_dropdown"] = "帧增强器模型",
            ["frame_enhancer_blend_slider"] = "帧增强器混合度",
            ["lip_syncer_model_dropdown"] = "嘴唇同步器模型",
            // memory  
            ["video_memory_strategy_dropdown"] = "视频内存策略",
            ["system_memory_limit_slider"] = "系统内存限制",
            // output  
            ["output_image_or_video"] = "输出",
            // output options  
            ["output_path_textbox"] = "输出路径",
            ["output_image_quality_slider"] = "输出图像质量",
            ["output_image_resolution_dropdown"] = "输出图像分辨率",
            ["output_video_encoder_dropdown"] = "输出视频编码器",
            ["output_video_preset_dropdown"] = "输出视频预设",
            ["output_video_quality_slider"] = "输出视频质量",
            ["output_video_resolution_dropdown"] = "输出视频分辨率",
            ["output_video_fps_slider"] = "输出视频帧率",
            // preview  
            ["preview_image"] = "预览",
            ["preview_frame_slider"] = "预览帧",
            // source  
            ["source_file"] = "源文件",
            // target  
            ["target_file"] = "目标文件",
            // temp frame  
            ["temp_frame_format_dropdown"] = "临时帧格式",
            // trim frame  
            ["trim_frame_start_slider"] = "裁剪帧开始",
            ["trim_frame_end_slider"] = "裁剪帧结束",
            // webcam  
            ["webcam_image"] = "网络摄像头",
            // webcam options  
            ["webcam_mode_radio"] = "网络摄像头模式",
            ["webcam_resolution_dropdown"] = "网络摄像头分辨率",
            ["webcam_fps_slider"] = "网络摄像头帧率"
        };

        //工具箱tts状态
        public static string BoxTTS = "stop";

		public static string GetWord(string key)
		{
			if (Wording.TryGetValue(key, out var word))
			{
				return word;
			}
			return key;
		}

  //      public static void Main(string[] args)
		//{
		//	if (!Directory.Exists(TEMP_DIR))
		//		Directory.CreateDirectory(TEMP_DIR);

		//	if (!Directory.Exists(LOGS_DIR))
		//		Directory.CreateDirectory(LOGS_DIR);

		//	// Set up logging
		//	using (StreamWriter sw = new StreamWriter(LOG_FILEPATH, true))
		//	{
		//		sw.WriteLine($"INFO: Starting execution at {DateTime.Now}");
		//	}

		//	try
		//	{
		//		Settings = ParseInit();
		//		if (!string.IsNullOrEmpty(Settings["lang"]))
		//			DEFAULT_LANG = Settings["lang"].ToLower();

		//		if (!Directory.Exists(Path.Combine(LANG_DIR, $"{DEFAULT_LANG}.json")))
		//			DEFAULT_LANG = "en";

		//		string langFilePath = Path.Combine(LANG_DIR, $"{DEFAULT_LANG}.json");
		//		var obj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(langFilePath));

		//		Transobj = (Dictionary<string, object>)obj["translate_language"];
		//		var uilanglist = (Dictionary<string, object>)obj["ui_lang"];
		//		var langlist = ((Dictionary<string, object>)obj["language_code_list"]).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
		//		var rev_langlist = langlist.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
		//		var langnamelist = langlist.Values.ToList();
		//		var box_lang = (Dictionary<string, object>)obj["toolbox_lang"];


		//		var queue_logs = new Queue<string>(1000);
		//		var queuebox_logs = new Queue<string>(1000);

		//		var box_status = "stop";
		//		int geshi_num = 0;

		//		var clone_voicelist = new List<string> { "clone" };
		//		string edgeTTS_rolelist = null;
		//		string proxy = null;
 
		//		// Configure ffmpeg path
		//		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		//			Environment.SetEnvironmentVariable("PATH", $"{rootdir};{rootdir}/ffmpeg;{Environment.GetEnvironmentVariable("PATH")}");
		//		else
		//			Environment.SetEnvironmentVariable("PATH", $"{rootdir}:{rootdir}/ffmpeg:{Environment.GetEnvironmentVariable("PATH")}");

		//		Environment.SetEnvironmentVariable("QT_API", "pyside6");

		//		// Start your application logic here...

		//	}
		//	catch (Exception ex)
		//	{
		//		using (StreamWriter sw = new StreamWriter(LOG_FILEPATH, true))
		//		{
		//			sw.WriteLine($"ERROR: {ex.Message}");
		//		}
		//	}
		//}

		public static Dictionary<string, string> ParseInit()
		{
			var settings = new Dictionary<string, string>
			{
				{"lang", DEFAULT_LANG},
				{"dubbing_thread", "5"},
				{"trans_thread", "15"},
				{"countdown_sec", "30"},
				{"cuda_com_type", "int8"},
				{"whisper_threads", "4"},
				{"whisper_worker", "1"},
				{"beam_size", "1"},
				{"best_of", "1"},
				{"vad", "True"},
				{"temperature", "0"},
				{"condition_on_previous_text", "False"},
				{"crf", "13"},
				{"retries", "5"},
				{"chatgpt_model", "gpt-3.5-turbo,gpt-4"},
				{"separate_sec", "1800"},
				{"audio_rate", "2.5"},
				{"initial_prompt_zh", ""},
				{"fontsize", "14"},
				{"voice_silence", "200"},
				{"interval_split", "10"},
				{"cjk_len", "24"},
				{"other_len", "36"},
				{"backaudio_volume", "0.5"},
				{"overall_silence", "100"},
				{"overall_maxsecs", "4"},
				{"remove_srt_silence", "False"},
				{"remove_silence", "True"},
				{"remove_white_ms", "0"}
			};

			if (File.Exists(SETTING_FILE))
			{
				string[] lines = File.ReadAllLines(SETTING_FILE);
				foreach (string line in lines)
				{
					if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line.StartsWith('['))
						continue;

					string[] parts = line.Split(new char[] { '=' }, 2);
					string key = parts[0].Trim();
					string value = parts[1].Trim();

					if (int.TryParse(value, out int intValue))
						settings[key] = intValue.ToString();
					else if (double.TryParse(value, out double doubleValue))
						settings[key] = Math.Round(doubleValue, 1).ToString();
					else if (bool.TryParse(value, out bool boolValue))
						settings[key] = boolValue.ToString();
					else
						settings[key] = value.ToLower();
				}
			}

			if (settings["fontsize"].EndsWith("px"))
				settings["fontsize"] = settings["fontsize"].Replace("px", "");

			return settings;
		}
	}
}
