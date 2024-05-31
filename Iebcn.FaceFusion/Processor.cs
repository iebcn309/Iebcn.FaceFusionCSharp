using Iebcn.FaceFusion.Utility;
using Microsoft.ML.OnnxRuntime;
using OpenCvSharp;

namespace Iebcn.FaceFusion;
public class Processor: IProcessor
{
    // 存储图像处理后的数据
    protected float[] processedImageData;
    // 定义图像的尺寸
    protected int imageWidth;
    protected int imageHeight;

    protected SessionOptions onnxSessionOptions;
    protected InferenceSession onnxInferenceSession;
    protected SemaphoreSlim _threadSemaphore = new SemaphoreSlim(1, 1);
    protected static object _threadLock = new object();
    protected ModelInfo _model;
    public void SetProcessor(string modelPath = null)
    {
        lock (_threadLock)
        {
            while (ProcessManager.IsChecking())
            {
                Thread.Sleep(500);
            }
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                modelPath = _model.Path;
            }
            var onnxSessionOptions = new SessionOptions();
            onnxSessionOptions.LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR;
            //onnxSessionOptions.AppendExecutionProvider_CPU(0);
            onnxSessionOptions.AppendExecutionProvider_CUDA(0);
            onnxInferenceSession = new InferenceSession(modelPath, onnxSessionOptions);
        }
    }
    public void ClearProcessor()
    {
        onnxInferenceSession = null;
    }
    public bool PreCheck()
    {
        string modelUrl = _model.Url;
        string modelPath = _model.Path;

        if (!TempFileManager.Globals.SkipDownload)
        {
            ProcessManager.Check();
            ConditionalDownload(modelPath, new[] { modelUrl });
            ProcessManager.End();
        }
        return File.Exists(modelPath);
    }

    private void ConditionalDownload(string downloadDirectoryPath, string[] strings)
    {
        throw new NotImplementedException();
    }

    public bool PostCheck()
    {
        string modelUrl = _model.Url;
        string modelPath = _model.Path;

        if (!TempFileManager.Globals.SkipDownload && !IsDownloadDone(modelUrl, modelPath))
        {
            NLogUtil.Error(Config.GetWord("model_download_not_done") + Config.GetWord("exclamation_mark"), nameof(Processor));
            return false;
        }
        if (!File.Exists(modelPath))
        {
            NLogUtil.Error(Config.GetWord("model_file_not_present") + Config.GetWord("exclamation_mark"), nameof(Processor));
            return false;
        }
        return true;
    }

    private bool IsDownloadDone(string modelUrl, string modelPath)
    {
        throw new NotImplementedException();
    }

    public void PostProcess()
    {
        throw new NotImplementedException();
    }

    public void Process(Mat image)
    {
        throw new NotImplementedException();
    }
}
public class ModelInfo
{
    public string Url { get; set; }
    public string Path { get; set; }
    public string Template { get; set; }
    public Size Size { get; set; }
    public string Type { get;  set; }
    public float[] Mean { get;  set; }
    public float[] StandardDeviation { get; set; }

}
