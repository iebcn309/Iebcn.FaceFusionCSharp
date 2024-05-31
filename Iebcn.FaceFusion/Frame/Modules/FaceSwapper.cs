using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Iebcn.FaceFusion.Frame.Modules;
public class FaceSwapper : Processor
{
    private float[] transformedData;

    private float[] modelMatrix;

    private List<Point2f> faceLandmarks;

    private float staticBoxMaskPadding = 0.3f;

    private int[] staticBoxMaskSize = new int[4];

    private float[] colorCorrectionFactor = new float[3];

    private float[] colorCorrectionOffset = new float[3] { 1f, 1f, 1f };

    public FaceSwapper()
    {
        _model = new ModelInfo { Type = "inswapper", Path = Path.Combine("model", "inswapper_128.onnx"), Url = "https://github.com/facefusion/facefusion-assets/releases/download/models/inswapper_128.onnx", Template = "arcface_128_v2", Size = new Size(128, 128), Mean = [0.0f, 0.0f, 0.0f], StandardDeviation = [1.0f, 1.0f, 1.0f] };
        imageHeight = 128;
        imageWidth = 128;
        faceLandmarks =
        [
            new Point2f(46.2946f, 51.6963f),
            new Point2f(81.5318f, 51.5014f),
            new Point2f(64.0252f, 71.7366f),
            new Point2f(49.5493f, 92.3655f),
            new Point2f(78.7299f, 92.2041f),
        ];
        modelMatrix = LoadModelMatrix(Path.Combine("model", "model_matrix.bin"));
        SetProcessor();

    }

    private float[] LoadModelMatrix(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                float[] array = new float[fileStream.Length / 4L];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = binaryReader.ReadSingle();
                }
                return array;
            }
        }
    }
    // 对输入图像进行预处理，包括仿射变换和颜色校正
    private Mat PreprocessImage(Mat inputMat, List<Point2f> detectedLandmarks, List<float> weights, ref Mat transformedImage, ref Mat mask)
    {
        Mat warpMat = new Mat();
        transformedImage = Common.warp_face_by_face_landmark_5(inputMat, warpMat, detectedLandmarks, faceLandmarks, new Size(imageWidth, imageHeight));
        int[] cropSize = new int[2] { warpMat.Cols, warpMat.Rows };
        mask = Common.create_static_box_mask(cropSize, staticBoxMaskPadding, staticBoxMaskSize);
        Mat[] channels = Cv2.Split(warpMat);
        for (int i = 0; i < channels.Length; i++)
        {
            channels[i].ConvertTo(channels[i], MatType.CV_32FC1, 1.0 / (255.0 * colorCorrectionOffset[i]), (0f - colorCorrectionFactor[i]) / colorCorrectionOffset[i]);
        }
        Cv2.Merge(channels, warpMat);

        processedImageData = Common.ExtractMat2(warpMat);
        warpMat.Dispose();
        foreach (Mat channel in channels)
        {
            channel.Dispose();
        }
        // 计算变换后的数据

        float num = 0f;
        for (int k = 0; k < 512; k++)
        {
            num = (float)((double)num + Math.Pow(weights[k], 2.0));
        }
        num = (float)Math.Sqrt(num);
        transformedData = new float[512];
        for (int l = 0; l < 512; l++)
        {
            float num2 = 0f;
            for (int m = 0; m < 512; m++)
            {
                num2 += weights[m] * modelMatrix[m * 512 + l];
            }
            transformedData[l] = num2 / num;
        }
        return mask;
    }

    public Mat Detect(Mat inputMat, List<float> weights, List<Point2f> detectedLandmarks)
    {
        Mat transformedImage = new Mat();
        Mat mask = new Mat();
        PreprocessImage(inputMat, detectedLandmarks, weights, ref transformedImage, ref mask);
        Tensor<float> inputTensor = new DenseTensor<float>(processedImageData, new int[4] { 1, 3, imageHeight, imageWidth });

        Tensor<float> transformedTensor = new DenseTensor<float>(transformedData, new int[2] { 1, 512 });
        List<NamedOnnxValue> inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor("target", inputTensor),
        NamedOnnxValue.CreateFromTensor("source", transformedTensor)
    };
        DisposableNamedOnnxValue[] modelOutputs = onnxInferenceSession.Run(inputs).ToArray();
        float[] outputData = modelOutputs[0].AsTensor<float>().ToArray();
        int num = 16384;
        for (int i = 0; i < outputData.Length; i++)
        {
            outputData[i] *= 255f;
            if (outputData[i] < 0f)
            {
                outputData[i] = 0f;
            }
            if (outputData[i] > 255f)
            {
                outputData[i] = 255f;
            }
        }
        float[] array3 = new float[num];
        float[] array4 = new float[num];
        float[] array5 = new float[num];
        Array.Copy(outputData, array3, num);
        Array.Copy(outputData, num, array4, 0, num);
        Array.Copy(outputData, num * 2, array5, 0, num);
        Mat mat = new Mat(128, 128, MatType.CV_32FC1, array3);
        Mat mat2 = new Mat(128, 128, MatType.CV_32FC1, array4);
        Mat mat3 = new Mat(128, 128, MatType.CV_32FC1, array5);
        Mat mat4 = new Mat();
        Cv2.Merge(new Mat[3] { mat3, mat2, mat }, mat4);
        mask.GetArray<float>(out var data);
        int cols = mask.Cols;
        int rows = mask.Rows;
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
        mask = new Mat(rows, cols, type, data);
        return Common.paste_back(inputMat, mat4, mask, transformedImage);
    }
    //public static ndarray ModelInitializer { get; private set; } = null;
    //public ndarray GetModelInitializer()
    //{
    //    lock (_threadLock)
    //    {
    //        while (ProcessManager.IsChecking())
    //        {
    //            Thread.Sleep(500);
    //        }
    //        if (ModelInitializer is null)
    //        {
    //            string modelPath = GetOptions("model").Model.Path;
    //            var model = new InferenceSession(modelPath);
    //            ModelInitializer = np.ndarray(new shape(model.OverridableInitializerMetadata.Last()));
    //        }
    //        return ModelInitializer;
    //    }
    //}

    //public static void ClearModelInitializer()
    //{
    //    ModelInitializer = null;
    //}
    // public static void RegisterArgs(ArgumentParser program)
    //{
    //    string faceSwapperModelFallback;
    //    if (Environment.OSVersion.Platform == PlatformID.Unix)
    //    {
    //        faceSwapperModelFallback = "inswapper_128";
    //    }
    //    else
    //    {
    //        faceSwapperModelFallback = "inswapper_128_fp16";
    //    }
    //    program.AddArgument("--face-swapper-model", help: Wordings.Get("help.face_swapper_model"), default: Config.GetStringValue("frame_processors.face_swapper_model", faceSwapperModelFallback), choices: FrameProcessorsChoices.FaceSwapperModels);
    //   }
    //   public static void ApplyArgs(ArgumentParserResult args)
    //   {
    //       FrameProcessorsGlobals.FaceSwapperModel = args.GetValueForOption("--face-swapper-model");
    //       switch (FrameProcessorsGlobals.FaceSwapperModel)
    //       {
    //           case "blendswap_256":
    //               TempFileManager.Globals.FaceRecognizerModel = "arcface_blendswap";
    //               break;
    //           case "inswapper_128":
    //           case "inswapper_128_fp16":
    //               TempFileManager.Globals.FaceRecognizerModel = "arcface_inswapper";
    //               break;
    //           case "simswap_256":
    //           case "simswap_512_unofficial":
    //               TempFileManager.Globals.FaceRecognizerModel = "arcface_simswap";
    //               break;
    //           case "uniface_256":
    //               TempFileManager.Globals.FaceRecognizerModel = "arcface_uniface";
    //               break;
    //       }
    //   }

    //public static bool PreProcess(ProcessMode mode)
    //{
    //    if (!TempFileManager.HasImage(TempFileManager.Globals.SourcePaths))
    //    {
    //        //Logger.Error(Wordings.Get("select_image_source") + Wordings.Get("exclamation_mark"), GlobalVariables.Name);
    //        return false;
    //    }
    //    List<string> sourceImagePaths = TempFileManager.FilterImagePaths(TempFileManager.Globals.SourcePaths);
    //    List<Mat> sourceFrames = Vision.ReadStaticImages(sourceImagePaths);
    //    foreach (Mat sourceFrame in sourceFrames)
    //    {
    //        if (!GetOneFace(sourceFrame))
    //        {
    //            //Logger.Error(Wordings.Get("no_source_face_detected") + Wordings.Get("exclamation_mark"), GlobalVariables.Name);
    //            return false;
    //        }
    //    }
    //    if (mode == ProcessMode.Output || mode == ProcessMode.Preview && (!TempFileManager.IsImage(TempFileManager.Globals.TargetPath) && !TempFileManager.IsVideo(TempFileManager.Globals.TargetPath)))
    //    {
    //        //Logger.Error(Wordings.Get("select_image_or_video_target") + Wordings.Get("exclamation_mark"), GlobalVariables.Name);
    //        return false;
    //    }
    //    if (mode == ProcessMode.Output && FileNormalization.NormalizeOutputPath(TempFileManager.Globals.TargetPath, TempFileManager.Globals.OutputPath) != null)
    //    {
    //        //Logger.Error(Wordings.Get("select_file_or_directory_output") + Wordings.Get("exclamation_mark"), GlobalVariables.Name);
    //        return false;
    //    }
    //    return true;
    //}

    //public static void PostProcess()
    //{
    //    //ReadStaticImage.ClearCache();
    //    if (TempFileManager.Globals.VideoMemoryStrategy == VideoMemoryStrategy.Strict || TempFileManager.Globals.VideoMemoryStrategy == VideoMemoryStrategy.Moderate)
    //    {
    //        ClearModelInitializer();
    //        ClearFrameProcessor();
    //    }
    //    if (TempFileManager.Globals.VideoMemoryStrategy == VideoMemoryStrategy.Strict)
    //    {
    //        ClearFaceAnalyser();
    //        ClearContentAnalyser();
    //        ClearFaceOccluder();
    //        ClearFaceParser();
    //    }
    //}
    //public Mat SwapFace(Face sourceFace, Face targetFace, Mat tempVisionFrame)
    //{
    //    string modelTemplate = GetOptions("model").Model.Template;
    //    Size modelSize = GetOptions("model").Model.Size;
    //    (Mat cropVisionFrame, Matrix3x2 affineMatrix) = WarpFaceByFaceLandmark5(tempVisionFrame, targetFace.Landmarks["5/68"], modelTemplate, modelSize);
    //    List<Mat> cropMaskList = new List<Mat>();

    //    if (TempFileManager.Globals.FaceMaskTypes.Contains(FaceMaskType.box))
    //    {
    //        Mat boxMask = CreateStaticBoxMask(new SizeF(tempVisionFrame.shape[0], tempVisionFrame.shape[1]), TempFileManager.Globals.FaceMaskBlur, TempFileManager.Globals.FaceMaskPadding);
    //        cropMaskList.Add(boxMask);
    //    }
    //    if (TempFileManager.Globals.FaceMaskTypes.Contains(FaceMaskType.occlusion))
    //    {
    //        Mat occlusionMask = CreateOcclusionMask(cropVisionFrame);
    //        cropMaskList.Add(occlusionMask);
    //    }
    //    cropVisionFrame = PrepareCropFrame(cropVisionFrame);
    //    cropVisionFrame = ApplySwap(sourceFace, cropVisionFrame);
    //    cropVisionFrame = NormalizeCropFrame(cropVisionFrame);
    //    if (TempFileManager.Globals.FaceMaskTypes.Contains(FaceMaskType.region))
    //    {
    //        Mat regionMask = CreateRegionMask(cropVisionFrame, TempFileManager.Globals.FaceMaskRegions);
    //        cropMaskList.Add(regionMask);
    //    }
    //    Mat cropMask = CropMaskList.MinimumReduce().Clip(0, 1);
    //    tempVisionFrame = PasteBack(tempVisionFrame, cropVisionFrame, cropMask, affineMatrix);
    //    return tempVisionFrame;
    //}

    //public Mat ApplySwap(Face sourceFace, Mat cropVisionFrame)
    //{
    //    InferenceSession frameProcessor = GetFrameProcessor();
    //    string modelType = GetOptions("model").Model.Type;
    //    List<NamedOnnxValue> frameProcessorInputs = new List<NamedOnnxValue>();
    //    //foreach (var frameProcessorInput in frameProcessor.InputMetadata)
    //    //{
    //    //    if (frameProcessorInput.Key == "source")
    //    //    {
    //    //        if (modelType == "blendswap" || modelType == "uniface")
    //    //        {
    //    //             var val= PrepareSourceFrame(sourceFace);
    //    //             frameProcessorInputs.Add(new NamedOnnxValue(frameProcessorInput.Key, val));
    //    //        }
    //    //        else
    //    //        {
    //    //            frameProcessorInputs[frameProcessorInput.Key] = PrepareSourceEmbedding(sourceFace);
    //    //        }
    //    //    }
    //    //    if (frameProcessorInput.Key == "target")
    //    //    {
    //    //        frameProcessorInputs[frameProcessorInput.Key] = cropVisionFrame;
    //    //    }
    //    //}
    //    cropVisionFrame = (Mat)frameProcessor.Run(frameProcessorInputs);
    //    return cropVisionFrame;
    //}

    //public Mat PrepareSourceFrame(Face sourceFace)
    //{
    //    string modelType = GetOptions("model").Model.Type;
    //    Mat sourceVisionFrame = Vision.ReadStaticImage(TempFileManager.Globals.SourcePaths[0]);
    //    if (modelType == "blendswap")
    //    {
    //        (sourceVisionFrame, _) = WarpFaceByFaceLandmark5(sourceVisionFrame, sourceFace.Landmarks["5/68"], "arcface_112_v2", new Size(112, 112));
    //    }
    //    if (modelType == "uniface")
    //    {
    //        (sourceVisionFrame, _) = WarpFaceByFaceLandmark5(sourceVisionFrame, sourceFace.Landmarks["5/68"], "ffhq_512", new Size(256, 256));
    //    }
    //    sourceVisionFrame = sourceVisionFrame[:, :, ::- 1] / 255.0;
    //    sourceVisionFrame = sourceVisionFrame.Transpose(2, 0, 1);
    //    sourceVisionFrame = sourceVisionFrame.ExpandDims(0).astype(np.Float32);
    //    return sourceVisionFrame;
    //}

    //public Mat PrepareSourceEmbedding(Face sourceFace)
    //{
    //    string modelType = GetOptions("model").Type;
    //    if (modelType == "inswapper")
    //    {
    //        NDarray modelInitializer = GetModelInitializer();
    //        Embedding sourceEmbedding = sourceFace.Embedding.Reshape(new Shape(1, -1));
    //        sourceEmbedding = sourceEmbedding.Dot(modelInitializer) / sourceEmbedding.Norm();
    //        return sourceEmbedding;
    //    }
    //    else
    //    {
    //        return sourceFace.NormedEmbedding.Reshape(new Shape(1, -1));
    //    }
    //}

    //public Mat PrepareCropFrame(Mat cropVisionFrame)
    //{
    //    float[] modelMean = GetOptions("model").Model.Mean;
    //    float[] modelStandardDeviation = GetOptions("model").Model.StandardDeviation;
    //    cropVisionFrame = cropVisionFrame[:, :, ::- 1] / 255.0;
    //    cropVisionFrame = (cropVisionFrame - modelMean) / modelStandardDeviation;
    //    cropVisionFrame = cropVisionFrame.Transpose(2, 0, 1);
    //    cropVisionFrame = cropVisionFrame.ExpandDims(0).astype(np.Float32);
    //    return cropVisionFrame;
    //}

    //public static Mat NormalizeCropFrame(Mat cropVisionFrame)
    //{
    //    cropVisionFrame = cropVisionFrame.Transpose(1, 2, 0);
    //    cropVisionFrame = (cropVisionFrame * 255.0).Round();
    //    cropVisionFrame = cropVisionFrame[:, :, ::- 1];
    //    return cropVisionFrame;
    //}

    //public static Mat GetReferenceFrame(Face sourceFace, Face targetFace, Mat tempVisionFrame)
    //{
    //    return SwapFace(sourceFace, targetFace, tempVisionFrame);
    //}

    //public Mat ProcessFrame(FaceSwapperInputs inputs)
    //{
    //    List<Face> referenceFaces = inputs.Get("reference_faces") as List<Face>;
    //    Face sourceFace = inputs.Get("source_face") as Face;
    //    Mat targetVisionFrame = inputs.Get("target_vision_frame") as Mat;

    //    if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.Many)
    //    {
    //        List<Face> manyFaces = GetManyFaces(targetVisionFrame);
    //        if (manyFaces.Any())
    //        {
    //            foreach (Face targetFace in manyFaces)
    //            {
    //                targetVisionFrame = SwapFace(sourceFace, targetFace, targetVisionFrame);
    //            }
    //        }
    //    }
    //    else if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.One)
    //    {
    //        Face targetFace = GetOneFace(targetVisionFrame);
    //        if (targetFace != null)
    //        {
    //            targetVisionFrame = SwapFace(sourceFace, targetFace, targetVisionFrame);
    //        }
    //    }
    //    else if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.Reference)
    //    {
    //        List<Face> similarFaces = FindSimilarFaces(referenceFaces, targetVisionFrame, TempFileManager.Globals.ReferenceFaceDistance);
    //        if (similarFaces.Any())
    //        {
    //            foreach (Face similarFace in similarFaces)
    //            {
    //                targetVisionFrame = SwapFace(sourceFace, similarFace, targetVisionFrame);
    //            }
    //        }
    //    }
    //    return targetVisionFrame;
    //}

    //public async Task ProcessFramesAsync(List<string> sourcePaths, List<QueuePayload> queuePayloads, UpdateProgress updateProgress)
    //{
    //    List<Face> referenceFaces = GetReferenceFaces() ?? new List<Face>();
    //    List<Mat> sourceFrames = await ReadStaticImagesAsync(sourcePaths);
    //    Face sourceFace = GetAverageFace(sourceFrames);

    //    foreach (QueuePayload queuePayload in ProcessManager.Manage(queuePayloads))
    //    {
    //        string targetVisionPath = queuePayload["frame_path"];
    //        Mat targetVisionFrame = await ReadImageAsync(targetVisionPath);
    //        Mat outputVisionFrame = ProcessFrame(
    //        {
    //            "reference_faces": referenceFaces,
    //            "source_face": sourceFace,
    //            "target_vision_frame": targetVisionFrame
    //        });
    //        await WriteImageAsync(targetVisionPath, outputVisionFrame);
    //        updateProgress(1);
    //    }
    //}
    //public void ProcessFrames(List<string> sourcePaths, List<QueuePayload> queuePayloads, Action<int> updateProgress)
    //{
    //    List<Face> referenceFaces = new List<Face>();

    //    if (TempFileManager.Globals.FaceSelectorMode == FaceSelectorMode.Reference)
    //    {
    //        referenceFaces = GetReferenceFaces();
    //    }
    //    List<Mat> sourceFrames = Vision.ReadStaticImages(sourcePaths);
    //    Face sourceFace = (sourceFrames);

    //    foreach (QueuePayload queuePayload in process_manager.manage(queuePayloads))
    //    {
    //        string targetVisionPath = queuePayload["frame_path"];
    //        Mat targetVisionFrame = read_image(targetVisionPath);
    //        Mat outputVisionFrame = process_frame(
    //         {
    //        reference_faces: referenceFaces,
    //             source_face: sourceFace,
    //             target_vision_frame: targetVisionFrame
    //         });
    //        write_image(targetVisionPath, outputVisionFrame);
    //        updateProgress(1);
    //    }
    //}

    //public void ProcessImage(List<string> sourcePaths, string targetPath, string outputPath)
    //{
    //    List<Face> referenceFaces = facefusion.globals.face_selector_mode.Contains("reference") ? get_reference_faces() : null;
    //    List<Mat> sourceFrames = read_static_images(sourcePaths);
    //    Face sourceFace = get_average_face(sourceFrames);
    //    Mat targetVisionFrame = read_static_image(targetPath);
    //    Mat outputVisionFrame = process_frame(
    //     {
    //    reference_faces: referenceFaces,
    //         source_face: sourceFace,
    //         target_vision_frame: targetVisionFrame
    //     });
    //    write_image(outputPath, outputVisionFrame);
    //}

    //public void ProcessVideo(List<string> sourcePaths, List<string> tempFramePaths)
    //{
    //    frame_processors.multi_process_frames(sourcePaths, tempFramePaths, ProcessFrames);
    //}

}
