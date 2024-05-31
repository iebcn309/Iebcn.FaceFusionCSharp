using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Iebcn.FaceFusion;

public class ContentAnalyser: Processor
{
    // 定义常量 _rateLimit
    //概率
    const float _probabilityLimit = 0.80f;
    const int _rateLimit = 10;
    int _streamCounter = 0;

    public ContentAnalyser() 
    {
        _model = new ModelInfo { Path = Path.Combine("model","open_nsfw.onnx"), Url = "https://github.com/facefusion/facefusion-assets/releases/download/models/open_nsfw.onnx" };
        SetProcessor();
    }

    // 分析视频流
    public bool AnalyseStream(Mat visionFrame, float videoFps)
    {
        _streamCounter++;
        if (_streamCounter % (int)videoFps == 0)
            return AnalyseFrame(visionFrame);
        return false;
    }

    // 分析单帧图像
    public bool AnalyseFrame(Mat visionFrame)
    {
        Mat preparedFrame = PrepareFrame(visionFrame);
        var float_0 = Common.ExtractMat(preparedFrame);
        Tensor<float> inputTensor = new DenseTensor<float>(float_0, new int[4] { 1, 224, 224, 3 });
        List<NamedOnnxValue> modelInput = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input:0", inputTensor) };
        DisposableNamedOnnxValue[] modelOutput = onnxInferenceSession.Run(modelInput).ToArray();
        float probability = modelOutput[0].AsTensor<float>().ToArray()[0];
        Console.WriteLine(probability);
        return probability > _probabilityLimit;
    }
    public Mat PrepareFrame(Mat inputFrame)
    {
        // 调整图像尺寸为 224x224
        var resizedFrame = new Mat(new Size(224, 224), MatType.CV_32FC3, Scalar.All(0));
        Cv2.Resize(inputFrame, resizedFrame, new Size(224, 224));

        // 将图像减去均值向量 [104, 117, 123]
        Scalar mean = new Scalar(104, 117, 123);
        Cv2.Subtract(resizedFrame, mean, resizedFrame);

        // 将图像转为一维数组并插入新维度（轴0），以符合模型输入要求
        //var reshapedFrame = new Mat(1, (int)resizedFrame.Total() * resizedFrame.Channels(), MatType.CV_32F);
        //resizedFrame.Reshape(0, 1).CopyTo(reshapedFrame);
        Mat[] channels = Cv2.Split(resizedFrame);
        // 将每个通道的数据类型转换为float，并归一化
        for (int i = 0; i < 3; i++)
        {
            channels[i].ConvertTo(channels[i], MatType.CV_32FC1, 1.0 / 255.0);
        }
        // 合并通道
        Cv2.Merge(channels, resizedFrame);
        foreach (Mat mat3 in channels)
        {
            mat3.Dispose();
        }
        return resizedFrame;
    }

    // 准备输入帧
    //public Mat PrepareFrame(Mat visionFrame)
    //{
    //    Mat resizedFrame = visionFrame.Resize(new Size(224, 224));
    //    // 将图像减去均值向量 [104, 117, 123]
    //    Scalar mean = new Scalar(104, 117, 123);
    //    Cv2.Subtract(resizedFrame, mean, resizedFrame);
    //    Mat[] channels = Cv2.Split(resizedFrame);
    //    // 将每个通道的数据类型转换为float，并归一化
    //    for (int i = 0; i < 3; i++)
    //    {
    //        channels[i].ConvertTo(channels[i], MatType.CV_32FC1, 1.0 / 255.0);
    //    }
    //    // 合并通道
    //    Cv2.Merge(channels, resizedFrame);
    //    foreach (Mat mat3 in channels)
    //    {
    //        mat3.Dispose();
    //    }
    //    return resizedFrame;
    //}

    // 缓存分析图像结果
    public bool AnalyseImage(string imagePath)
    {
        Mat frame = Vision.ReadImage(imagePath);
        return AnalyseFrame(frame);
    }

    // 缓存分析视频结果
    public bool AnalyseVideo(string videoPath, int startFrame, int endFrame)
    {
        int videoFrameTotal = Vision.CountVideoFrameTotal(videoPath);
        float videoFps = Vision.DetectVideoFps(videoPath);
        var frameRange = Enumerable.Range(startFrame, endFrame==0 ? videoFrameTotal:endFrame);
        double rate = 0.0;
        int counter = 0;
        foreach (int frameNumber in frameRange)
        {
            if (frameNumber % (int)videoFps == 0)
            {
                Mat frame = Vision.GetVideoFrame(videoPath, frameNumber);
                if (AnalyseFrame(frame))
                    counter++;
            }
            rate = counter * (int)videoFps / frameRange.Count() * 100;

        }

        return rate > _rateLimit;
    }
}

