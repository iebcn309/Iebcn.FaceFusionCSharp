using Gradio.Net;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Utility;
using Microsoft.ML.OnnxRuntime;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class Execution
    {
        public static async Task Render()
        {

            CheckboxGroup ExecutionProvidersCheckboxGroup = gr.CheckboxGroup(label: Config.UICn["execution_providers_checkbox_group"], choices: OrtEnv.Instance().GetAvailableProviders().ToList(), value: VideoProcessingOption.Instance.ExecutionProviders);
            var ExecutionThreadCountSlider = gr.Slider(label: Config.UICn["execution_thread_count_slider"], value: VideoProcessingOption.Instance.ExecutionThreadCount, step: 1, minimum: 1, maximum: 128);
            var ExecutionQueueCounSlidert = gr.Slider(label: Config.UICn["execution_queue_count_slider"], value: VideoProcessingOption.Instance.ExecutionQueueCount, step: 1, minimum: 1, maximum: 32);

            Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false, visible: false);
            Button btn = gr.Button("Run", visible: false);
            await btn.Click(fn: async (input) => gr.Output($@"
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[0]))}
"), inputs: [ExecutionProvidersCheckboxGroup, ExecutionThreadCountSlider, ExecutionQueueCounSlidert], outputs: [text_Result]);
        }

    }
}
