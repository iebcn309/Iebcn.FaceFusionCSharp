using Gradio.Net;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class CommonOptions
    {
        public static async Task Render()
        {
            var value =new List<string>();
            if (VideoProcessingOption.Instance.KeepTemp)
            {
                value.Add("keep-temp");
            }
            if (VideoProcessingOption.Instance.SkipAudio)
            {
                value.Add("skip-audio");
            }
            if (VideoProcessingOption.Instance.SkipDownload)
            {
                value.Add("skip-download");
            }
            var CommonOptionsCheckboxgroup = gr.CheckboxGroup(label: Config.UICn["common_options_checkbox_group"], choices: Choices.CommonOptions, value: value);
        
        }
    }
}
