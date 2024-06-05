using Gradio.Net;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class FaceAnalyser
    {
        public static async Task Render()
        {
            var label = Config.UICn["face_detector_size_dropdown"];
            var value = VideoProcessingOption.Instance.FaceDetectorSize;
            List<string> choices = [];
            if (Choices.FaceDetectorSet.TryGetValue(VideoProcessingOption.Instance.FaceDetectorModel,out List<string> faceDetectorSizes)&& faceDetectorSizes.Contains(value))
            {
                choices = faceDetectorSizes;
            }
            using (gr.Row())
            {
                gr.Dropdown(label: Config.UICn["face_analyser_order_dropdown"], choices: Choices.FaceAnalyserOrders, value: VideoProcessingOption.Instance.FaceAnalyserOrder);
                gr.Dropdown(label: Config.UICn["face_analyser_age_dropdown"], choices: Choices.FaceAnalyserAges, value: VideoProcessingOption.Instance.FaceAnalyserAge);
                gr.Dropdown(label: Config.UICn["face_analyser_gender_dropdown"], choices: Choices.FaceAnalyserGenders, value: VideoProcessingOption.Instance.FaceAnalyserGender);
            }
            gr.Dropdown(label: Config.UICn["face_detector_model_dropdown"], choices: Choices.FaceDetectorSet.Keys, value: VideoProcessingOption.Instance.FaceDetectorModel);
            gr.Dropdown(label: label, choices: choices, value: value);
            using (gr.Row())
            {
                gr.Slider(label: Config.UICn["face_detector_score_slider"],value: VideoProcessingOption.Instance.FaceDetectorScore,step:0.05m,minimum:0,maximum:1.0m);
                gr.Slider(label: Config.UICn["face_landmarker_score_slider"], value: VideoProcessingOption.Instance.FaceLandmarkerScore, step: 0.05m, minimum: 0, maximum: 1.0m);

            }
        }
    }
}
