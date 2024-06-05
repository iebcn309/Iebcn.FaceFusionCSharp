using Gradio.Net;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Frame;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class FaceSelector
    {
        public static async Task Render()
        {
           var FaceSelectorModeDropdown = gr.Dropdown(label: Config.UICn["face_selector_mode_dropdown"], choices: Choices.FaceSelectorModes, value: VideoProcessingOption.Instance.FaceSelectorMode);
            gr.Image(label: Config.UICn["reference_face_gallery"], visible: VideoProcessingOption.Instance.FaceSelectorMode.Contains("reference"));
            gr.Slider(label: Config.UICn["reference_face_distance_slider"],value: VideoProcessingOption.Instance.ReferenceFaceDistance,step:0.05m,minimum:0,maximum:1.5m, visible: VideoProcessingOption.Instance.FaceSelectorMode.Contains("reference"));
        }
    }
}
