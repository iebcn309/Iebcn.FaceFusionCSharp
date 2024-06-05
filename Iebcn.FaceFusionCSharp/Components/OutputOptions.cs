using Gradio.Net;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class OutputOptions
    {
        public static async Task Render()
        {
            var output_image_resolutions = new List<string>();
            var output_video_resolutions = new List<string>();
            var OutputPathTextbox = gr.Textbox(label: Config.UICn["output_path_textbox"], value: VideoProcessingOption.Instance.OutputPath, maxLines: 1);

            if (TempFileManager.IsImage(VideoProcessingOption.Instance.TargetPath))
            {
               var output_image_resolution= Vision.DetectImageResolution(VideoProcessingOption.Instance.TargetPath);
                if (output_image_resolution != null)
                {
                    output_image_resolutions=Vision.CreateImageResolutions(output_image_resolution.Value);
                }
                var OutputImageQualitySlider = gr.Slider(label: Config.UICn["output_image_quality_slider"], value: VideoProcessingOption.Instance.OutputImageQuality, step: 1, minimum: 0, maximum: 100, visible: TempFileManager.IsImage(VideoProcessingOption.Instance.TargetPath));
                var OutputImageResolutionDropdown = gr.Dropdown(label: Config.UICn["output_image_resolution_dropdown"], choices: output_image_resolutions, value: VideoProcessingOption.Instance.OutputImageResolution, visible: TempFileManager.IsImage(VideoProcessingOption.Instance.TargetPath));
            }
            else if (TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath))
            {
                var output_video_resolution = Vision.DetectVideoResolution(VideoProcessingOption.Instance.TargetPath);
                if (output_video_resolution != null)
                {
                    output_video_resolutions = Vision.CreateImageResolutions(output_video_resolution.Value);
                }
                var OutputVideoEncoderDropdown = gr.Dropdown(label: Config.UICn["output_video_encoder_dropdown"], choices: Choices.OutputVideoEncoders, value: VideoProcessingOption.Instance.OutputVideoEncoder, visible: TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath));
                var OutputVideoPresetDropdown = gr.Dropdown(label: Config.UICn["output_video_preset_dropdown"], choices: Choices.OutputVideoPresets, value: VideoProcessingOption.Instance.OutputVideoPreset, visible: TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath));
                var OutputVideoQualitySlider = gr.Slider(label: Config.UICn["output_video_quality_slider"], value: VideoProcessingOption.Instance.OutputVideoQuality, step: 1, minimum: 0, maximum: 100, visible: TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath));
                var OutputVideoResolutionDropdown = gr.Dropdown(label: Config.UICn["output_video_resolution_dropdown"], choices: output_video_resolutions, value: VideoProcessingOption.Instance.OutputVideoResolution, visible: TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath));
                var OutputVideoFpsSlider = gr.Slider(label: Config.UICn["output_video_fps_slider"], value: VideoProcessingOption.Instance.OutputVideoFps, step: 0.01m, minimum: 1, maximum: 60, visible: TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath));
            }
            Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false, visible: false);
            Button btn = gr.Button("Run", visible: false);
            await btn.Click(fn: async (input) => gr.Output($@"
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[0]))}
"), inputs: [OutputPathTextbox], outputs: [text_Result]);
        }
    }
}
