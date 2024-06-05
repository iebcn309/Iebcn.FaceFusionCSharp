using Gradio.Net;
using Iebcn.FaceFusion.Utility;

namespace Iebcn.FaceFusionCSharp.Components
{
    public static class About
    {
        public static async Task Render()
        {
            await Task.Factory.StartNew(() =>
            {
                string title = $"{Metadata.Instance.Name} {Metadata.Instance.Version.ToString()}";
                Button ABOUT_BUTTON = gr.Button(title, variant: Gradio.Net.Enums.ButtonVariant.Primary, link: Metadata.Instance.Url);
                Button DONATE_BUTTON = gr.Button(Config.UICn["donate_button"], size: Gradio.Net.Enums.ButtonSize.Sm, link: Metadata.Instance.Url);
            });
        }

    }
}
