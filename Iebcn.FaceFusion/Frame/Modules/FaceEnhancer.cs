using Iebcn.FaceFusion.Utility;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Iebcn.FaceFusion.Frame.Modules;
public class FaceEnhancer : Processor
{
    // 预定义的关键点列表，用于图像仿射变换
    private List<Point2f> predefinedLandmarks;
    private float float_1 = 0.3f;
    private int[] int_2 = new int[4];

    public FaceEnhancer()
    {
        _model = new ModelInfo { Path = Path.Combine("model", "gfpgan_1.4.onnx"), Url = "https://github.com/facefusion/facefusion-assets/releases/download/models/gfpgan_1.4.onnx", Template = "", Size = new Size(512, 512) };
        imageWidth = 512;
        imageHeight = 512;
        predefinedLandmarks = new List<Point2f>();
        predefinedLandmarks.Add(new Point2f(192.98138f, 239.94708f));
        predefinedLandmarks.Add(new Point2f(318.90277f, 240.1936f));
        predefinedLandmarks.Add(new Point2f(256.63416f, 314.01935f));
        predefinedLandmarks.Add(new Point2f(201.26117f, 371.41043f));
        predefinedLandmarks.Add(new Point2f(313.08905f, 371.15118f));
        SetProcessor();
    }

    public static bool PreProcess(ProcessMode mode)
    {
        if (mode == ProcessMode.Output || mode == ProcessMode.Preview && !TempFileManager.Globals.TargetPath.IsImage() && !TempFileManager.Globals.TargetPath.IsVideo())
        {
            NLogUtil.Error(Config.GetWord("select_image_or_video_target") + Config.GetWord("exclamation_mark"), nameof(FaceEnhancer));
            return false;
        }
        if (mode == ProcessMode.Output && FileNormalization.NormalizeOutputPath(TempFileManager.Globals.TargetPath, TempFileManager.Globals.OutputPath) == null)
        {
            NLogUtil.Error(Config.GetWord("select_file_or_directory_output") + Config.GetWord("exclamation_mark"), nameof(FaceEnhancer));
            return false;
        }
        return true;
    }
    // 对输入图像进行仿射变换，并创建遮罩
    private void ApplyAffineTransformationAndCreateMask(Mat inputMat, List<Point2f> detectedLandmarks, ref Mat transformedImage, ref Mat mask)
    {
        Mat mat = new Mat();
        transformedImage = Common.warp_face_by_face_landmark_5(inputMat, mat, detectedLandmarks, predefinedLandmarks, new Size(imageWidth, imageHeight));
        // 创建静态框遮罩
        int[] cropSize = new int[2] { mat.Cols, mat.Rows };
        mask = Common.create_static_box_mask(cropSize, float_1, int_2);

        Mat[] array = Cv2.Split(mat);
        for (int i = 0; i < 3; i++)
        {
            array[i].ConvertTo(array[i], MatType.CV_32FC1, 2.0 / 255.0, -1.0);
        }
        Cv2.Merge(array, mat);
        Mat[] array2 = array;
        foreach (Mat mat2 in array2)
        {
            mat2.Dispose();
        }
        processedImageData = Common.ExtractMat2(mat);
        mat.Dispose();
    }
    // 将处理后的图像数据通过ONNX模型进行推理，并获取增强后的图像
    public Mat EnhanceFace(Mat inputMat, List<Point2f> detectedLandmarks)
    {
        // 初始化输出图像和遮罩
        Mat transformedImage = new Mat();
        Mat mask = new Mat();
        ApplyAffineTransformationAndCreateMask(inputMat, detectedLandmarks, ref transformedImage, ref mask);
        // 将提取的图像数据转换为ONNX模型所需的Tensor格式
        Tensor<float> inputTensor = new DenseTensor<float>(processedImageData, new int[4] { 1, 3, imageWidth, imageHeight });
        // 准备ONNX模型的输入
        List<NamedOnnxValue> modelInputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };
        DisposableNamedOnnxValue[] modelOutputs = onnxInferenceSession.Run(modelInputs).ToArray();
        float[] rawOutput = modelOutputs[0].AsTensor<float>().ToArray();
        // 对模型输出进行归一化处理

        int num = 262144;
        for (int i = 0; i < rawOutput.Length; i++)
        {
            rawOutput[i] = (rawOutput[i] + 1f) * 0.5f;
            if (rawOutput[i] < -1f)
            {
                rawOutput[i] = -1f;
            }
            if (rawOutput[i] > 1f)
            {
                rawOutput[i] = 1f;
            }
            rawOutput[i] *= 255f;
            if (rawOutput[i] < 0f)
            {
                rawOutput[i] = 0f;
            }
            if (rawOutput[i] > 255f)
            {
                rawOutput[i] = 255f;
            }
        }
        float[] array3 = new float[num];
        float[] array4 = new float[num];
        float[] array5 = new float[num];
        Array.Copy(rawOutput, array3, num);
        Array.Copy(rawOutput, num, array4, 0, num);
        Array.Copy(rawOutput, num * 2, array5, 0, num);
        Mat mat = new Mat(imageHeight, imageWidth, MatType.CV_32FC1, array3);
        Mat mat2 = new Mat(imageHeight, imageWidth, MatType.CV_32FC1, array4);
        Mat mat3 = new Mat(imageHeight, imageWidth, MatType.CV_32FC1, array5);
        Mat mat4 = new Mat();
        Cv2.Merge(new Mat[3] { mat3, mat2, mat }, mat4);
        mat4.ConvertTo(mat4, MatType.CV_8UC3);
        mask.GetArray<float>(out var data);
        int cols2 = mask.Cols;
        int rows2 = mask.Rows;
        MatType type = mask.Type();
        for (int j = 0; j < data.Length; j++)
        {
            if (data[j] < 0f)
            {
                data[j] = 0f;
            }
            if (data[j] > 1f)
            {
                data[j] = 1f;
            }
        }
        mask = new Mat(rows2, cols2, type, data);
        Mat paste_vision_frame = Common.paste_back(inputMat, mat4, mask, transformedImage);
        return Common.blend_frame(inputMat, paste_vision_frame);
    }
    //public Mat EnhanceFace(Face targetFace, Mat tempVisionFrame)
    //{
    //    string modelTemplate = GetOptions("model").Model.Template;
    //    Size modelSize = GetOptions("model").Model.Size;
    //    (Mat cropVisionFrame, double[,] affineMatrix) = WarpFaceByFaceLandmark5(tempVisionFrame, targetFace.Landmarks["5/68"], modelTemplate, modelSize);
    //    Mat boxMask = CreateStaticBoxMask(new Size(cropVisionFrame.Width, cropVisionFrame.Height), TempFileManager.Globals.FaceMaskBlur, new Scalar(0, 0, 0, 0));
    //    List<Mat> cropMaskList = new List<Mat> { boxMask };

    //    if (TempFileManager.Globals.FaceMaskTypes.Contains(FaceMaskType.occlusion))
    //    {
    //        Mat occlusionMask = CreateOcclusionMask(cropVisionFrame);
    //        cropMaskList.Add(occlusionMask);
    //    }
    //    cropVisionFrame = PrepareCropFrame(cropVisionFrame);
    //    cropVisionFrame = ApplyEnhance(cropVisionFrame);
    //    cropVisionFrame = NormalizeCropFrame(cropVisionFrame);
    //    Mat cropMask = cropMaskList.Min().Clamp(0, 1);
    //    Mat pasteVisionFrame = PasteBack(tempVisionFrame, cropVisionFrame, cropMask, affineMatrix);
    //    tempVisionFrame = BlendFrame(tempVisionFrame, pasteVisionFrame);
    //    return tempVisionFrame;
    //}
    //public Mat ApplyEnhance(Mat cropVisionFrame)
    //{
    //    InferenceSession frameProcessor = GetFrameProcessor();
    //    Dictionary<string, OrtValue> frameProcessorInputs = new Dictionary<string, OrtValue>();

    //    foreach (var input in frameProcessor.InputMetadata)
    //    {
    //        if (input.Key == "input")
    //        {
    //            frameProcessorInputs[input.Key] = cropVisionFrame.ToOrtValue();
    //        }
    //        else if (input.Key == "weight")
    //        {
    //            var weight = np.array(new double[] { 1 });
    //            frameProcessorInputs[input.Key] = weight.ToOrtValue();
    //        }
    //    }

    //    _threadSemaphore.Wait();
    //    try
    //    {
    //        var results = frameProcessor.Run(frameProcessorInputs.Values.ToList(), null);
    //        return results[0].GetTensor().ToMat();
    //    }
    //    finally
    //    {
    //        _threadSemaphore.Release();
    //    }
    //}
    //public Mat PrepareCropFrame(Mat cropVisionFrame)
    //{
    //    cropVisionFrame = cropVisionFrame.ConvertTo(MatType.CV_32F).Mul(1.0 / 255.0);
    //    cropVisionFrame = (cropVisionFrame - 0.5) / 0.5;
    //    cropVisionFrame.Reshape(1, 3, cropVisionFrame.Rows, cropVisionFrame.Cols);
    //    return cropVisionFrame;
    //}
    //public static Mat NormalizeCropFrame(Mat cropVisionFrame)
    //{
    //    cropVisionFrame = cropVisionFrame.Clip(-1, 1).Mul(0.5).Add(0.5);
    //    cropVisionFrame.Reshape(0, 3, cropVisionFrame.Rows, cropVisionFrame.Cols);
    //    cropVisionFrame = cropVisionFrame.ConvertTo(MatType.CV_8U).Mul(255).Round().ToMat();
    //    cropVisionFrame = cropVisionFrame.CvtColor(ColorConversionCodes.BGR2RGB);
    //    return cropVisionFrame;
    //}
    //public Mat BlendFrame(Mat tempVisionFrame, Mat pasteVisionFrame)
    //{
    //    double faceEnhancerBlend = 1 - (TempFileManager.Globals.FaceEnhancerBlend / 100.0);
    //    Cv2.AddWeighted(src1: InputArray.Create(tempVisionFrame), alpha: faceEnhancerBlend, src2: InputArray.Create(pasteVisionFrame), beta: 1 - faceEnhancerBlend, gamma: 0, dst: OutputArray.Create(tempVisionFrame));
    //    return tempVisionFrame;
    //}
    //public Mat GetReferenceFrame(Face sourceFace, Face targetFace, Mat tempVisionFrame)
    //{
    //    return EnhanceFace(targetFace, tempVisionFrame);
    //}
    //public Mat ProcessFrame(FaceEnhancerInputs inputs)
    //{
    //    var referenceFaces = inputs.ReferenceFaces;
    //    Mat targetVisionFrame = inputs.TargetVisionFrame;

    //    if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.Many)
    //    {
    //        List<Face> manyFaces = GetManyFaces(targetVisionFrame);
    //        if (manyFaces != null)
    //        {
    //            foreach (Face targetFace in manyFaces)
    //            {
    //                targetVisionFrame = EnhanceFace(targetFace, targetVisionFrame);
    //            }
    //        }
    //    }
    //    else if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.One)
    //    {
    //        Face targetFace = GetOneFace(targetVisionFrame);
    //        if (targetFace != null)
    //        {
    //            targetVisionFrame = EnhanceFace(targetFace, targetVisionFrame);
    //        }
    //    }
    //    else if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.Reference)
    //    {
    //        List<Face> similarFaces = FindSimilarFaces(referenceFaces, targetVisionFrame, TempFileManager.Globals.ReferenceFaceDistance);
    //        if (similarFaces != null)
    //        {
    //            foreach (Face similarFace in similarFaces)
    //            {
    //                targetVisionFrame = EnhanceFace(similarFace, targetVisionFrame);
    //            }
    //        }
    //    }
    //    return targetVisionFrame;
    //}
    //public void ProcessFrames(List<string> sourcePaths, List<QueuePayload> queuePayloads, UpdateProgress updateProgress)
    //{
    //    var referenceFaces = Vision.GetReferenceFaces() ?? new Dictionary<string, List<Face>>();

    //    foreach (QueuePayload queuePayload in ProcessManager.Manage(queuePayloads))
    //    {
    //        string targetVisionPath = queuePayload.FramePath;
    //        Mat targetVisionFrame = Vision.ReadImage(targetVisionPath);
    //        Mat outputVisionFrame = ProcessFrame(new FaceEnhancerInputs { ReferenceFaces = referenceFaces, TargetVisionFrame = targetVisionFrame });
    //        Vision.WriteImage(targetVisionPath, outputVisionFrame);
    //        updateProgress(1);
    //    }
    //}
    //public void ProcessImage(string sourcePath, string targetPath, string outputPath)
    //{
    //    var referenceFaces = Vision.GetReferenceFaces() ?? new Dictionary<string, List<Face>>();
    //    Mat targetVisionFrame = Vision.ReadStaticImage(targetPath);
    //    Mat outputVisionFrame = ProcessFrame(new FaceEnhancerInputs { ReferenceFaces = referenceFaces, TargetVisionFrame = targetVisionFrame });
    //    Vision.WriteImage(outputPath, outputVisionFrame);
    //}
}
