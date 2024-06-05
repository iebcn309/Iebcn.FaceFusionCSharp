using Gradio.Net;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;
using Microsoft.ML.OnnxRuntime;
using Iebcn.FaceFusion.Frame;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class Memory
    {
        public static async Task Render()
        {

            var VideoMemoryStrategyDropdown = gr.Dropdown(label: Config.UICn["video_memory_strategy_dropdown"], choices: Choices.VideoMemoryStrategys, value: VideoProcessingOption.Instance.VideoMemoryStrategy);
            var SystemMemoryLimitSlider = gr.Slider(label: Config.UICn["system_memory_limit_slider"], value: VideoProcessingOption.Instance.SystemMemoryLimit, step: 1, minimum: 0, maximum: 128);

            Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false, visible: false);
            Button btn = gr.Button("Run", visible: false);
            await btn.Click(fn: async (input) => gr.Output($@"
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[0]))}
"), inputs: [VideoMemoryStrategyDropdown, SystemMemoryLimitSlider,], outputs: [text_Result]);
        }
    }
}
