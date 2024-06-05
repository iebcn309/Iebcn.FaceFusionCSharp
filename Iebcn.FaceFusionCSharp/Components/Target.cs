using Gradio.Net;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class Target
    {
        public static async Task Render()
        {
            var is_target_image = TempFileManager.IsImage(VideoProcessingOption.Instance.TargetPath);
            var is_target_video = TempFileManager.IsVideo(VideoProcessingOption.Instance.TargetPath);
            gr.Image(label: Config.UICn["target_file"], value:VideoProcessingOption.Instance.TargetPath);

        }
    }
}
