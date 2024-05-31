using Gradio.Net;
using Iebcn.FaceFusion;
using Iebcn.FaceFusion.Utility;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;
VideoProcessingOption videoProcessingOption = new VideoProcessingOption();
videoProcessingOption.FrameProcessors = new List<string>() { "换脸", "脸部增强", "帧增强" };
App.Launch(await CreateBlocks());

async Task<Blocks> CreateBlocks()
{
    var executingAssemblyName = Assembly.GetExecutingAssembly().GetName();
    string title = $"{executingAssemblyName.Name} {executingAssemblyName.Version.ToString()}";
    string url = "";
    using (var blocks = gr.Blocks(title: title))
    {
        Button ABOUT_BUTTON, DONATE_BUTTON;
        using (gr.Row())
        {
            using (gr.Column(scale:2))
            {
                ABOUT_BUTTON = gr.Button(title,variant: Gradio.Net.Enums.ButtonVariant.Primary,link: url);
                DONATE_BUTTON = gr.Button(Config.UICn["donate_button"], size: Gradio.Net.Enums.ButtonSize.Sm, link: url);
                gr.Markdown(Config.UICn["frame_processors_checkbox_group"]);
                using (gr.Group())
                {
                    gr.Textbox(value: string.Join(",", videoProcessingOption.FrameProcessors));
                }

                using (gr.Group())
                {
                    gr.Textbox(label: Config.UICn["face_debugger_items_checkbox_group"], visible: videoProcessingOption.FrameProcessors.Contains("face_debugger"));

                    gr.Textbox(value:string.Join("", Enum.GetNames(typeof( FaceEnhancerModel))), label: Config.UICn["face_enhancer_model_dropdown"], visible: videoProcessingOption.FrameProcessors.Contains("脸部增强"));
                    gr.Textbox(value:videoProcessingOption.FaceEnhancerBlend.ToString(), label: Config.UICn["face_enhancer_blend_slider"], visible: videoProcessingOption.FrameProcessors.Contains("脸部增强"));

                    gr.Textbox(value: string.Join("", Enum.GetNames(typeof(FaceSwapperModel))), label: Config.UICn["face_swapper_model_dropdown"], visible: videoProcessingOption.FrameProcessors.Contains("换脸"));

                    gr.Textbox(label: Config.UICn["frame_colorizer_model_dropdown"], visible: videoProcessingOption.FrameProcessors.Contains("frame_colorizer"));
                    gr.Textbox(label: Config.UICn["frame_colorizer_blend_slider"], visible: videoProcessingOption.FrameProcessors.Contains("frame_colorizer"));
                    gr.Textbox(label: Config.UICn["frame_colorizer_size_dropdown"], visible: videoProcessingOption.FrameProcessors.Contains("frame_colorizer"));

                    gr.Textbox(value: string.Join("", Enum.GetNames(typeof(FrameEnhancerModel))), label: Config.UICn["frame_enhancer_model_dropdown"], visible: videoProcessingOption.FrameProcessors.Contains("帧增强"));
                    gr.Textbox(value: videoProcessingOption.FrameEnhancerBlend.ToString(), label: Config.UICn["frame_enhancer_blend_slider"], visible: videoProcessingOption.FrameProcessors.Contains("帧增强"));

                    gr.Textbox(label: Config.UICn["lip_syncer_model_dropdown"], visible: videoProcessingOption.FrameProcessors.Contains("lip_syncer"));
                }

                using (gr.Group())
                {
                    gr.Textbox(value: string.Join(",", videoProcessingOption.ExecutionProviders),label: Config.UICn["execution_providers_checkbox_group"]);
                }
                using (gr.Group())
                {
                    gr.Textbox(value: videoProcessingOption.ExecutionThreadCount.ToString(), label: Config.UICn["execution_thread_count_slider"]);
                }
                using (gr.Group())
                {
                    gr.Textbox(value: videoProcessingOption.ExecutionQueueCount.ToString(), label: Config.UICn["execution_queue_count_slider"]);
                }
                using (gr.Group())
                {
                    gr.Textbox(value: videoProcessingOption.OutputPath, label: Config.UICn["output_path_textbox"]);
                }
            }
            using (gr.Column(scale: 2))
            {
                using (gr.Group())
                {
                    bool has_source_image = TempFileManager.HasImage(videoProcessingOption.SourcePaths);
                    gr.Textbox(value: videoProcessingOption.SourcePaths?.Count > 0 ? videoProcessingOption.SourcePaths[0] : "", label: Config.UICn["source_file"]);
                    gr.Image(value: videoProcessingOption.SourcePaths?.Count>0? videoProcessingOption.SourcePaths[0]:"",showLabel:false,visible: has_source_image);
                }
                using (gr.Group())
                {
                    gr.Textbox(value: videoProcessingOption.TargetPath, label: Config.UICn["target_file"]);

                    bool has_source_image = TempFileManager.HasImage(videoProcessingOption.SourcePaths);
                    gr.Image(value: videoProcessingOption.SourcePaths?.Count > 0 ? videoProcessingOption.SourcePaths[0] : "", showLabel: false, visible: has_source_image);

                }
                using (gr.Group())
                {
                    gr.Image(label: Config.UICn["output_image_or_video"], visible: false);
                    gr.Image(label: Config.UICn["output_image_or_video"]);
                    gr.Button(Config.UICn["start_button"], variant: Gradio.Net.Enums.ButtonVariant.Primary, size: Gradio.Net.Enums.ButtonSize.Sm); 
                    gr.Button(Config.UICn["stop_button"], variant: Gradio.Net.Enums.ButtonVariant.Primary, size: Gradio.Net.Enums.ButtonSize.Sm);
                    gr.Button(Config.UICn["clear_button"], variant: Gradio.Net.Enums.ButtonVariant.Primary, size: Gradio.Net.Enums.ButtonSize.Sm);
                }
            }
            using (gr.Column(scale: 3))
            {
                using (gr.Group())
                {
                    //FaceSelectorMode
                    gr.Textbox(value: string.Join("", Enum.GetNames(typeof(FaceSelectorMode))), label: Config.UICn["face_selector_mode_dropdown"]);
                }
            }
        }

 
        return blocks;
    }
}