using Gradio.Net;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;
using Newtonsoft.Json.Linq;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class Output
    {
        public static async Task Render()
        {
            var OutputImage = gr.Image(label: Config.UICn["output_image_or_video"], visible: false);
            var OutputVideo = gr.Image(label: Config.UICn["output_image_or_video"], visible: false);
            var StartButton = gr.Button(value: Config.UICn["start_button"], variant: Gradio.Net.Enums.ButtonVariant.Primary, size: Gradio.Net.Enums.ButtonSize.Sm);
            var StopButton = gr.Button(value: Config.UICn["stop_button"], variant: Gradio.Net.Enums.ButtonVariant.Primary, size: Gradio.Net.Enums.ButtonSize.Sm, visible: false);
            var ClearButton = gr.Button(value: Config.UICn["clear_button"], size: Gradio.Net.Enums.ButtonSize.Sm);
            await StartButton.Click(fn: async (input) => { return gr.Output(); });
        }
    }
}
