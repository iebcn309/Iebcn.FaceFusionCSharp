using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using OpenCvSharp;

namespace Iebcn.FaceFusion;
public class FaceLandmarker:Processor
{
    private Mat mat_0 = new Mat();
    public FaceLandmarker()
    {
        _model = new ModelInfo { Path = Path.Combine("model", "2dfan4.onnx"), Url = "https://github.com/facefusion/facefusion-assets/releases/download/models/2dfan4.onnx" };
        imageWidth = 256;
        imageHeight = 256;
        SetProcessor();
    }
    // 对输入图像进行预处理，以适配模型输入尺寸
    private void PreprocessImage(Mat inputMat, BoundingBox bbox)
    {
        // 计算缩放比例
        float scale = Math.Max(bbox.XMax - bbox.XMin, bbox.YMax - bbox.YMin);
        float scaleRatio = 195f / scale;
        // 计算平移量
        float[] translation = new float[2]
        {
        (imageWidth - (bbox.XMax + bbox.XMin) * scaleRatio) * 0.5f,
        (imageHeight - (bbox.YMax + bbox.YMin) * scaleRatio) * 0.5f
        };
        // 创建仿射变换矩阵
        Mat affineMatrix = new Mat(2, 3, MatType.CV_32FC1, new float[6]
        {
        scaleRatio,
        0f,
        translation[0],
        0f,
        scaleRatio,
        translation[1]
        });
        // 应用仿射变换
        Mat processedMat = new Mat();
        Cv2.WarpAffine(inputMat, processedMat, affineMatrix, new Size(imageWidth, imageHeight));
        // 计算仿射变换的逆矩阵，用于后续坐标转换
        Cv2.InvertAffineTransform(affineMatrix, mat_0);
        // 将处理后的图像数据转换为模型输入所需的格式
        // 分离图像的RGB通道
        Mat[] channels = Cv2.Split(processedMat);
        // 将每个通道的数据类型转换为float，并归一化
        for (int i = 0; i < 3; i++)
        {
            channels[i].ConvertTo(channels[i], MatType.CV_32FC1, 1.0 / 255.0);
        }
        // 合并通道
        Cv2.Merge(channels, processedMat);
        foreach (Mat mat3 in channels)
        {
            mat3.Dispose();
        }
        // 提取图像数据为一维数组
        processedImageData = Common.ExtractMat(processedMat);
        processedMat.Dispose();
        // 创建Tensor对象，模拟模型输入
        //return new DenseTensor<float>(imageData, new int[4] { 1, 3, imageWidth, imageHeight });
    }
    // 运行ONNX模型并获取输出
    internal List<Point2f> RunModelAndGetLandmarks(Mat inputMat, BoundingBox bbox)
    {
        // 预处理图像
        PreprocessImage(inputMat, bbox);

        // 将预处理后的图像转换为模型输入所需的Tensor格式
        Tensor<float> inputTensor = new DenseTensor<float>(processedImageData, new int[4] { 1, 3, imageWidth, imageHeight });
        // 准备模型输入
        List<NamedOnnxValue> modelInput = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };
        // 运行模型并获取输出
        DisposableNamedOnnxValue[] modelOutput = onnxInferenceSession.Run(modelInput).ToArray();
        float[] rawOutput = modelOutput[0].AsTensor<float>().ToArray();
        // 解析模型输出，转换为关键点坐标
        List<Point2f> landmarks = new List<Point2f>();
        for (int i = 0; i < 68; i++)// 假设模型输出68个关键点
        {
            landmarks.Add(new Point2f((float)((double)rawOutput[i * 3] / 64.0 * imageWidth), (float)((double)rawOutput[i * 3 + 1] / 64.0 * imageHeight)));
        }
        // 应用逆变换，将关键点坐标转换回原始图像空间
        Mat landmarksMat = new Mat(landmarks.Count, 1, MatType.CV_32FC2, landmarks.ToArray());
        Mat mat2 = new Mat();
        Cv2.Transform(landmarksMat, mat2, mat_0);
        mat2.GetArray<Point2f>(out var processedLandmarks);
        // 根据需要选择关键点
        Point2f[] selectedLandmarks = new Point2f[5];
        float num2 = 0f;
        float num3 = 0f;
        for (int j = 36; j < 42; j++)
        {
            num2 += processedLandmarks[j].X;
            num3 += processedLandmarks[j].Y;
        }
        num2 /= 6f;
        num3 /= 6f;
        selectedLandmarks[0] = new Point2f(num2, num3);
        num2 = 0f;
        num3 = 0f;
        for (int k = 42; k < 48; k++)
        {
            num2 += processedLandmarks[k].X;
            num3 += processedLandmarks[k].Y;
        }
        num2 /= 6f;
        num3 /= 6f;
        selectedLandmarks[1] = new Point2f(num2, num3);
        selectedLandmarks[2] = processedLandmarks[30];
        selectedLandmarks[3] = processedLandmarks[48];
        selectedLandmarks[4] = processedLandmarks[54];
        return selectedLandmarks.ToList();
    }
}