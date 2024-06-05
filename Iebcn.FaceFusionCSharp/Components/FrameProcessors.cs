using Gradio.Net;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Utility;

namespace Iebcn.FaceFusionCSharp.Components
{
    public static class FrameProcessors
    {
        public static async Task Render()
        {
            CheckboxGroup checkboxGroup1 = gr.CheckboxGroup(label: Config.UICn["frame_processors_checkbox_group"], choices: SortFrameProcessors(), value: new[] { "FaceSwapper" });
            Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false,visible:false);
            Button btn = gr.Button("Run", visible: false);
            await btn.Click(fn: async (input) => gr.Output($@"
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[0]))}
"), inputs: [checkboxGroup1], outputs: [text_Result]);
        }
        // 用于排序frame_processors的列表
        public static IEnumerable<string> SortFrameProcessors()
        {
            // 获取facefusion/processors/frame/modules目录下的所有文件名
            List<string> availableFrameProcessors = typeof(IProcessor).Assembly.GetTypes()
           .Where(t => typeof(IProcessor).IsAssignableFrom(t) && !t.IsInterface && t.FullName.StartsWith("Iebcn.FaceFusion.Frame.Modules")).Select(t => t.Name).ToList();

            // 使用自定义排序逻辑对availableFrameProcessors进行排序
            availableFrameProcessors.Sort((frameProcessor1, frameProcessor2) =>
            {
                int index1 = VideoProcessingOption.Instance.FrameProcessors
.IndexOf(frameProcessor1);
                int index2 = VideoProcessingOption.Instance.FrameProcessors
.IndexOf(frameProcessor2);
                // 如果两个处理器都在frameProcessors中，则根据在frameProcessors中的索引排序
                // 如果只有一个在frameProcessors中，则该处理器排在前面
                // 如果都不在frameProcessors中，则根据availableFrameProcessors的长度排序，确保它们排在最后
                if (index1 >= 0 && index2 >= 0)
                    return index1.CompareTo(index2);
                else if (index1 >= 0)
                    return -1; // frameProcessor1在frameProcessors中，排在前面
                else if (index2 >= 0)
                    return 1; // frameProcessor2在frameProcessors中，排在前面
                else
                    return VideoProcessingOption.Instance.FrameProcessors
.Count.CompareTo(availableFrameProcessors.Count); // 都不在，排在最后
            });
            return availableFrameProcessors;
        }
    }
}
