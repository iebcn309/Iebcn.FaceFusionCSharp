using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using OpenCvSharp;

namespace Iebcn.FaceFusion;
public class FaceRecognizerArcface:Processor
{
    // 存储关键点的列表
    private List<Point2f> predefinedLandmarks = new List<Point2f>();
    // 构造函数，初始化ONNX推理所需的参数和关键点
    public FaceRecognizerArcface()
    {
        _model = new ModelInfo { Path = Path.Combine("model", "arcface_w600k_r50.onnx"), Url = "https://github.com/facefusion/facefusion-assets/releases/download/models/arcface_w600k_r50.onnx" };
        imageWidth = 112;
        imageHeight = 112;
        // 添加预定义的关键点
        predefinedLandmarks = new List<Point2f>
            {
                new Point2f(38.2946f, 51.6963f),
                new Point2f(73.5318f, 51.5014f),
                new Point2f(56.0252f, 71.7366f),
                new Point2f(41.5493f, 92.3655f),
                new Point2f(70.7299f, 92.2041f)
            };
        //predefinedLandmarks = new List<Point2f>
        //    {
        //        new Point2f(0.34191607f, 0.46157411f),
        //        new Point2f(0.65653393f, 0.45983393f),
        //        new Point2f(0.50022500f, 0.64050536f),
        //        new Point2f(0.37097589f, 0.82469196f),
        //        new Point2f(0.63151696f, 0.82325089f)
        //    };
        SetProcessor();

    }
    // 使用预定义的关键点对输入图像进行仿射变换
    private void WarpImageByLandmarks(Mat inputMat, List<Point2f> detectedLandmarks)
    {
        // 使用FaceFusionSharp库中的warp_face_by_face_landmark_5方法进行图像仿射变换
        Mat warpedMat = new Mat();
        Common.warp_face_by_face_landmark_5(inputMat, warpedMat, detectedLandmarks, predefinedLandmarks, new Size(imageWidth, imageHeight));
        // 分离图像的RGB通道，并将每个通道的数据类型转换为float，进行归一化
        Mat[] channels = Cv2.Split(warpedMat);
        for (int i = 0; i < channels.Length; i++)
        {
            channels[i].ConvertTo(channels[i], MatType.CV_32FC1, 2.0f / 255.0f, -1.0f);
        }
        // 合并通道
        Cv2.Merge(channels, warpedMat);
        // 释放分离出来的通道的内存
        foreach (Mat mat2 in channels)
        {
            mat2.Dispose();
        }
        // 提取图像数据
        processedImageData = Common.ExtractMat(warpedMat);
        // 释放warpedMat的内存
        warpedMat.Dispose();
    }
    // 将处理后的图像数据通过ONNX模型进行推理，返回推理结果
    internal List<float> CalcEmbedding(Mat inputMat, List<Point2f> detectedLandmarks)
    {
        // 对输入图像进行仿射变换
        WarpImageByLandmarks(inputMat, detectedLandmarks);
        // 将提取的图像数据转换为ONNX模型所需的Tensor格式
        Tensor<float> inputTensor = new DenseTensor<float>(processedImageData, new int[4] { 1, 3, imageWidth, imageHeight });
        // 准备ONNX模型的输入
        List<NamedOnnxValue> modelInputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", inputTensor) };
        // 运行ONNX模型并获取输出
        DisposableNamedOnnxValue[] modelOutputs = onnxInferenceSession.Run(modelInputs).ToArray();
        float[] outputData = modelOutputs[0].AsTensor<float>().ToArray();
        return outputData.ToList();
    }
}
