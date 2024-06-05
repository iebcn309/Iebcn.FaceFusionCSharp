using Gradio.Net;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Utility;
using Iebcn.FaceFusionCSharp.Components;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;

App.Launch(await CreateBlocks());

async Task<Blocks> CreateBlocks()
{
    string title = $"{Metadata.Instance.Name} {Metadata.Instance.Version.ToString()}";
    var css = await File.ReadAllTextAsync(Path.Combine("assets", "fixes.css")) + await File.ReadAllTextAsync(Path.Combine("assets", "overrides.css"));
    using (var blocks = gr.Blocks(title: title, css: css))
    {
        using (gr.Row())
        {
            using (gr.Column(scale:2))
            {
                await About.Render();
                using (gr.Group())
                {
                    await FrameProcessors.Render();
                }
                using (gr.Group())
                {
                    await FrameProcessorsOptions.Render();
                }

                using (gr.Group())
                {
                    await Execution.Render();
                }
                using (gr.Group())
                {
                    await Memory.Render();
                }
                using (gr.Group())
                {
                    await OutputOptions.Render();
                }

            }
            using (gr.Column(scale: 2))
            {
                using (gr.Group())
                {
                   await Source.Render();
                }
                using (gr.Group())
                {
                    await Target.Render();
                }
                using (gr.Group())
                {
                    await Iebcn.FaceFusionCSharp.Components.Output.Render();
                }
            }
            using (gr.Column(scale: 3))
            {
                using (gr.Group())
                {
                    await Preview.Render();
                }
                await TrimFrame.Render();
                await FaceSelector.Render();
                await FaceMasker.Render();
                await FaceAnalyser.Render();
                await CommonOptions.Render();
            }
        }

 
        return blocks;
    }
}