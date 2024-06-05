using Gradio.Net;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class TempFrame
    {
        public static async Task Render()
        {

            var TempFrameFormatDropdown = gr.Dropdown(label: Config.UICn["temp_frame_format_dropdown"], choices: Choices.TempFrameFormats, value: VideoProcessingOption.Instance.TempFrameFormat,visible:TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath));
 
            Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false, visible: false);
            Button btn = gr.Button("Run", visible: false);
            await btn.Click(fn: async (input) => gr.Output($@"
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[0]))}
"), inputs: [TempFrameFormatDropdown], outputs: [text_Result]);
        }
    }
}
