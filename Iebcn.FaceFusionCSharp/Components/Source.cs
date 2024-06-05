using Gradio.Net;
using Iebcn.FaceFusion.Frame;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusion;

namespace Iebcn.FaceFusionCSharp.Components
{
    public class Source
    {
        public static async Task Render()
        {
            var has_source_audio = TempFileManager.HasAudio(VideoProcessingOption.Instance.SourcePaths);
            var has_source_image = TempFileManager.HasImage(VideoProcessingOption.Instance.SourcePaths);
            gr.Image(label: Config.UICn["source_file"], value: has_source_image ? VideoProcessingOption.Instance.SourcePaths[0] : null);

        }
    }
}
