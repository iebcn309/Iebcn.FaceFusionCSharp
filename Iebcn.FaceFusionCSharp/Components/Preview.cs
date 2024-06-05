using Gradio.Net;
using Iebcn.FaceFusion.Utility;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class Preview
    {
        public static async Task Render()
        {
            var PreviewImage = gr.Image(label: Config.UICn["preview_image"]);
            var PreviewImageSlider = gr.Slider(label: Config.UICn["preview_frame_slider"]);

        }
    }
}
