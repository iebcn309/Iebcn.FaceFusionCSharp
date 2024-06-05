using Gradio.Net;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Utility;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class TrimFrame
    {
        public static async Task Render()
        {
            var step = 1;
            var minimum = 0;
            var maximum = 100;
            var visible = false;
            var start = 0;
            var end = 0;

            if (TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath))
            {
                var video_frame_total = Vision.CountVideoFrameTotal(VideoProcessingOption.Instance.TargetPath);
                start = VideoProcessingOption.Instance.TrimFrameStart;
                end = VideoProcessingOption.Instance.TrimFrameEnd <= 0 ? video_frame_total : VideoProcessingOption.Instance.TrimFrameEnd;
                maximum = video_frame_total;
                visible = true;
            }
            using (gr.Row())
            {
                var TrimFrameStartSlider = gr.Slider(label: Config.UICn["trim_frame_start_slider"], step: step, minimum: minimum, maximum: maximum, visible: visible);
                var TrimFrameEndSlider = gr.Slider(label: Config.UICn["trim_frame_end_slider"], step: step, minimum: minimum, maximum: maximum, visible: visible);
            }

        }
    }
}
