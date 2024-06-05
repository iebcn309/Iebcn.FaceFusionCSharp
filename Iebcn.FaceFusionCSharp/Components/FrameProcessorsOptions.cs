using Gradio.Net;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class FrameProcessorsOptions
    {
        public static async Task Render()
        {
            using (gr.Group())
            {
                var FaceDebuggerItemsCheckboxGroup = gr.CheckboxGroup(label: Config.UICn["face_debugger_items_checkbox_group"], choices: Choices.FaceDebuggerItems, value: VideoProcessingOption.Instance.FaceDebuggerItems, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FaceDebugger"));

                var FaceEnhancerModelDropdown = gr.Dropdown(label: Config.UICn["face_enhancer_model_dropdown"], choices: Choices.FaceEnhancerModels, value: VideoProcessingOption.Instance.FaceEnhancerModel, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FaceEnhancer"));
                var FaceEnhancerBlendSlider = gr.Slider(label: Config.UICn["face_enhancer_blend_slider"], value: VideoProcessingOption.Instance.FaceEnhancerBlend, step: 1, minimum: 0, maximum: 100, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FaceEnhancer"));

                var FaceSwapperModelDropdown = gr.Dropdown(label: Config.UICn["face_swapper_model_dropdown"], choices: Choices.FaceSwapperModels, value: VideoProcessingOption.Instance.FaceSwapperModel, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FaceSwapper"));

                var FrameColorizerModeDropdown = gr.Dropdown(label: Config.UICn["frame_colorizer_model_dropdown"], choices: Choices.FrameColorizerModels, value: VideoProcessingOption.Instance.FrameColorizerModel, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FrameColorizer"));
                var FrameColorizerBlendSlider = gr.Slider(label: Config.UICn["frame_colorizer_blend_slider"], value: VideoProcessingOption.Instance.FrameColorizerBlend, step: 1, minimum: 0, maximum: 100, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FrameColorizer"));
                var FrameColorizerSizeDropdown = gr.Dropdown(label: Config.UICn["frame_colorizer_size_dropdown"], choices: Choices.FrameColorizerSizes, value: VideoProcessingOption.Instance.FrameColorizerSize, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FrameColorizer"));

                var FrameEnhancerModelDropdown = gr.Dropdown(label: Config.UICn["frame_enhancer_model_dropdown"], choices: Choices.FrameEnhancerModels, value: VideoProcessingOption.Instance.FrameEnhancerModel, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FrameEnhancer"));
                var FrameEnhancerBlendSlider = gr.Slider(label: Config.UICn["frame_enhancer_blend_slider"], value: VideoProcessingOption.Instance.FrameEnhancerBlend, step: 1, minimum: 0, maximum: 100, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("FrameEnhancer"));

                var LipSyncerModelDropdown = gr.Dropdown(label: Config.UICn["lip_syncer_model_dropdown"], choices: Choices.LipSyncerModels, value: VideoProcessingOption.Instance.LipSyncerModel, visible: VideoProcessingOption.Instance.FrameProcessors.Contains("lip_syncer"));
                Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false, visible: false);
                Button btn = gr.Button("Run", visible: false);
                await btn.Click(fn: async (input) => gr.Output($@"
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[0]))}
"), inputs: [FaceDebuggerItemsCheckboxGroup, FaceEnhancerModelDropdown, FaceEnhancerBlendSlider, FaceSwapperModelDropdown, FrameColorizerModeDropdown, FrameColorizerBlendSlider, FrameColorizerSizeDropdown, FrameEnhancerModelDropdown, FrameEnhancerBlendSlider, LipSyncerModelDropdown], outputs: [text_Result]);
            }
        }

    }
}
