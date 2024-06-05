using OpenCvSharp;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using Iebcn.FaceFusion.Utility;

namespace Iebcn.FaceFusion;
public class YoloFace : Processor, IFaceAnalyser
{
    public YoloFace()
    {
        _model = new ModelInfo() { Path = Path.Combine("model", "yoloface_8n.onnx"), Url = "https://github.com/facefusion/facefusion-assets/releases/download/models/yoloface_8n.onnx" };
        SetProcessor();
    }
    public FaceDetectResult Detect(Mat visionFrame, string faceDetectorSize = "640x640")
    {
        // 解析分辨率
        var detectorResolution = Vision.UnpackResolution(faceDetectorSize);
        var tempVisionFrame = Vision.ResizeFrameResolution(visionFrame, detectorResolution);
        // 计算缩放比例
        var ratioWidth = (double)visionFrame.Width / (double)tempVisionFrame.Cols;
        var ratioHeight = (double)visionFrame.Height / (double)tempVisionFrame.Rows;
        Mat input_img = new Mat();
        Cv2.CopyMakeBorder(tempVisionFrame, input_img, 0, detectorResolution.Height - tempVisionFrame.Rows, 0, detectorResolution.Width - tempVisionFrame.Cols, BorderTypes.Constant, 0.0);
        Mat[] bgrChannels = Cv2.Split(input_img);
        for (int i = 0; i < 3; i++)
        {
            bgrChannels[i].ConvertTo(bgrChannels[i], MatType.CV_32FC1, 1.0 / 128.0, -127.5 / 128.0);
        }
        Cv2.Merge(bgrChannels, input_img);
        foreach (Mat channel in bgrChannels)
        {
            channel.Dispose();
        }
        var input_image = Common.ExtractMat(input_img);
        Tensor<float> input_tensor = new DenseTensor<float>(input_image, new int[4] { 1, 3, detectorResolution.Height, detectorResolution.Width });
        List<NamedOnnxValue> input_container = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("images", input_tensor) };
        DisposableNamedOnnxValue[] ort_outputs = onnxInferenceSession.Run(input_container).ToArray();

        // 形状是(1, 20, 8400),不考虑第0维batchsize，每一列的长度20,前4个元素是检测框坐标(cx,cy,w,h)，第4个元素是置信度，剩下的15个元素是5个关键点坐标x,y和置信度
        // 初始化结果列表
        float[] pdata = ort_outputs[0].AsTensor<float>().ToArray();
        int num_box = 8400;
        List<BoundingBox> bounding_box_raw = new List<BoundingBox>();
        List<float> score_raw = new List<float>();
        for (int i = 0; i < num_box; i++)
        {
            float score = pdata[4 * num_box + i];
            if (score.ObjectToDecimal() > TempFileManager.Globals.FaceDetectorScore)
            {
                float xmin = (float)(((double)pdata[i] - 0.5 * (double)pdata[2 * num_box + i]) * (double)ratioWidth);//(cx,cy,w,h)转到(x,y,w,h)并还原到原图
                float ymin = (float)(((double)pdata[num_box + i] - 0.5 * (double)pdata[3 * num_box + i]) * (double)ratioHeight);
                float xmax = (float)(((double)pdata[i] + 0.5 * (double)pdata[2 * num_box + i]) * (double)ratioWidth);
                float ymax = (float)(((double)pdata[num_box + i] + 0.5 * (double)pdata[3 * num_box + i]) * (double)ratioHeight);
                //坐标的越界检查保护，可以添加一下
                bounding_box_raw.Add(new BoundingBox(xmin, ymin, xmax, ymax));
                score_raw.Add(score);
                //剩下的5个关键点坐标的计算,暂时不写,因为在下游的模块里没有用到5个关键点坐标信息
            }
        }
        float iouThreshold = TempFileManager.Globals.FaceDetectorModel == "many" ? 0.1f : 0.4f;
        List<int> keep_inds = Common.nms(bounding_box_raw, score_raw, iouThreshold);
        int keep_num = keep_inds.Count();
        var result = new FaceDetectResult();
        for (int j = 0; j < keep_num; j++)
        {
            int ind = keep_inds[j];
            result.BoundingBox.Add(bounding_box_raw[ind]);
            result.ScoreList.Add(score_raw[ind]);
        }
        return result;
    }
    ///// <summary>
    ///// 准备检测帧
    ///// </summary>
    ///// <param name="frame">图像帧</param>
    ///// <param name="detectorSize">检测器的分辨率</param>
    ///// <returns>准备后的图像帧</returns>
    //private Mat PrepareDetectFrame(Mat tempVisionFrame, string faceDetectorSize)
    //{
    //    var faceDetectorResolution = Vision.UnpackResolution(faceDetectorSize);
    //    // 初始化检测用图像帧，大小为面部检测器分辨率，颜色深度为3通道（BGR）
    //    Mat detectVisionFrame = new Mat(faceDetectorResolution, MatType.CV_8UC3, Scalar.All(0));
    //    // 将原始图像帧裁剪并复制到检测用图像帧
    //    Cv2.Resize(tempVisionFrame, detectVisionFrame, faceDetectorResolution);
    //    // 中心化像素值（减去127.5）并除以范围（除以128.0）
    //    Cv2.Subtract(detectVisionFrame, new Scalar(127.5), detectVisionFrame);
    //    Cv2.ConvertScaleAbs(detectVisionFrame, detectVisionFrame, 1.0 / 128.0);
    //    detectVisionFrame.ConvertTo(detectVisionFrame, MatType.CV_32F);
    //    // 转置图像通道顺序（从 BGR 到 RGB）
    //    detectVisionFrame = detectVisionFrame.T();

    //    // 将图像增加一个额外维度（批量大小为1）
    //    detectVisionFrame.Reshape(1, Convert.ToInt32(detectVisionFrame.Total()));
    //    // 确保图像数据类型为单精度浮点数
    //    detectVisionFrame.ConvertTo(detectVisionFrame, MatType.CV_32F);
    //    return detectVisionFrame;
    //}
    ///// <summary>
    ///// 准备用于面部检测的图像帧。
    ///// </summary>
    ///// <param name="tempVisionFrame">待处理的原始图像帧。</param>
    ///// <param name="faceDetectorSize">面部检测器所需的分辨率，格式为 "宽度x高度"。</param>
    ///// <returns>已准备好的用于面部检测的图像帧。</returns>
    //public ndarray PrepareDetectFrame(ndarray tempVisionFrame, string faceDetectorSize)
    //{
    //    // 解析面部检测器分辨率
    //    Size faceDetectorResolution = Vision.UnpackResolution(faceDetectorSize);

    //    // 初始化检测用图像帧，大小为面部检测器分辨率，颜色深度为3通道（BGR）
    //    var detectVisionFrame = np.zeros(new shape(faceDetectorResolution.Height, faceDetectorResolution.Width, 3));

    //    // 将原始图像帧裁剪并复制到检测用图像帧
    //    // 裁剪原始图像帧到检测用图像帧大小
    //    detectVisionFrame[":", ":", ":"] = tempVisionFrame[":", ":", ":", new Slice(0, Math.Min(tempVisionFrame.shape[0], detectVisionFrame.shape[0])),
    //                                                           new Slice(0, Math.Min(tempVisionFrame.shape[1], detectVisionFrame.shape[1])),
    //                                                           new Slice(0, detectVisionFrame.shape[2])];
    //    // 归一化像素值并转为浮点数
    //    // 中心化像素值（减去127.5）并除以范围（除以128.0）
    //    detectVisionFrame = detectVisionFrame - 127.5;
    //    detectVisionFrame /= 128.0;

    //    // 调整通道顺序并增加额外维度（批量大小为1）
    //    // 转置图像通道顺序（从 BGR 到 RGB）
    //    detectVisionFrame =np.expand_dims( detectVisionFrame.Transpose(new long[] { 2, 0, 1 }),0).astype(np.Float32);

    //    detectVisionFrame = detectVisionFrame.reshape(new shape(1, detectVisionFrame.size));

    //    return detectVisionFrame;
    //}
}
