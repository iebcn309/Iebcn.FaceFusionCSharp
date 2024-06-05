using Gradio.Net;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class FaceMasker
    {
        public static async Task Render()
        {
            var has_box_mask = VideoProcessingOption.Instance.FaceMaskTypes.Contains("box");
            var has_region_mask = VideoProcessingOption.Instance.FaceMaskTypes.Contains("region");
            var FaceMaskTypesCheckboxGroup = gr.CheckboxGroup(label: Config.UICn["face_mask_types_checkbox_group"],choices:Choices.FaceMaskTypes,value: VideoProcessingOption.Instance.FaceMaskTypes);
            using (gr.Group(visible: has_box_mask))
            {
                gr.Slider(label: Config.UICn["face_mask_blur_slider"], value: VideoProcessingOption.Instance.FaceMaskBlur, step: 0.05m, minimum: 0, maximum: 1.0m);
                using (gr.Row())
                {
                    gr.Slider(label: Config.UICn["face_mask_padding_top_slider"], value: VideoProcessingOption.Instance.FaceMaskPadding.Top, step: 1, minimum: 0, maximum: 100);
                    gr.Slider(label: Config.UICn["face_mask_padding_right_slider"], value: VideoProcessingOption.Instance.FaceMaskPadding.Right, step: 1, minimum: 0, maximum: 100);
                }
                using (gr.Row())
                {
                    gr.Slider(label: Config.UICn["face_mask_padding_bottom_slider"], value: VideoProcessingOption.Instance.FaceMaskPadding.Bottom, step: 1, minimum: 0, maximum: 100);
                    gr.Slider(label: Config.UICn["face_mask_padding_left_slider"], value: VideoProcessingOption.Instance.FaceMaskPadding.Left, step: 1, minimum: 0, maximum: 100);

                }
            }
            using (gr.Row())
            {
                var FaceMaskRegionCheckboxGroup = gr.CheckboxGroup(label: Config.UICn["face_mask_region_checkbox_group"], choices: Choices.FaceMaskRegions, value: VideoProcessingOption.Instance.FaceMaskRegions,visible:has_region_mask);
            }
            var FaceSelectorModeDropdown = gr.Dropdown(label: Config.UICn["face_selector_mode_dropdown"], choices: Choices.FaceSelectorModes, value: VideoProcessingOption.Instance.FaceSelectorMode);
            gr.Image(label: Config.UICn["reference_face_gallery"], visible: VideoProcessingOption.Instance.FaceSelectorMode.Contains("reference"));
            gr.Slider(label: Config.UICn["reference_face_distance_slider"], value: VideoProcessingOption.Instance.ReferenceFaceDistance, step: 0.05m, minimum: 0, maximum: 1.5m, visible: VideoProcessingOption.Instance.FaceSelectorMode.Contains("reference"));
        }
    }
}
