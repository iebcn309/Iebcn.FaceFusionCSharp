using Iebcn.FaceFusion.Frame.Modules;
using Iebcn.FaceFusion.Utility;
using OpenCvSharp;

namespace Iebcn.FaceFusion;
public class VideoProcessor
{
    private YoloFace _yolov8Face=new YoloFace();
    private FaceLandmarker _faceLandmarker=new FaceLandmarker();
    private FaceRecognizerArcface faceRecognizerArcface=new FaceRecognizerArcface();
    private FaceSwapper faceSwapper=new FaceSwapper();
    private FaceEnhancer faceEnhancer = new FaceEnhancer();

    public VideoProcessor()
    {
    }

    /// <summary>
    /// 处理视频文件
    /// </summary>
    /// <param name="startTime">处理开始时间（秒）</param>
    public void ProcessVideo(double startTime)
    {
        // 规范化输出路径
        var normedOutputPath = FileNormalization.NormalizeOutputPath(TempFileManager.Globals.TargetPath, TempFileManager.Globals.OutputPath);
        ContentAnalyser contentAnalyser = new ContentAnalyser();
        // 判断视频是否包含敏感内容，若包含则直接返回
        //if (contentAnalyser.AnalyseVideo(TempFileManager.Globals.TargetPath, TempFileManager.Globals.TrimFrameStart, TempFileManager.Globals.TrimFrameEnd))
        //{
        //    return;
        //}

        // 清理临时文件
        NLogUtil.Debug(Config.GetWord("clearing_temp"), nameof(VideoProcessor).ToUpper());
        TempFileManager.ClearTemp(TempFileManager.Globals.TargetPath);

        // 创建临时文件夹
        NLogUtil.Debug(Config.GetWord("creating_temp"), nameof(VideoProcessor).ToUpper());
        TempFileManager.CreateTemp(TempFileManager.Globals.TargetPath);


        using var capture = new VideoCapture(TempFileManager.Globals.TargetPath);
        if (!capture.IsOpened())
            return;
        int sleepTime = (int)Math.Round(1000 / capture.Fps);
        Mat matSource = Cv2.ImRead(TempFileManager.Globals.SourcePaths[0]);
        //处理源图像
        var result = _yolov8Face.Detect(matSource);
        List<Point2f> faceLandmarksSource = _faceLandmarker.RunModelAndGetLandmarks(matSource, result.BoundingBox[0]);
        List<float> mbeddingSource = faceRecognizerArcface.CalcEmbedding(matSource, faceLandmarksSource);
        List<float> face1 = null;

        var frame = new Mat();
        using var writer = new VideoWriter("output_video.mp4", -1, capture.Fps, new Size((int)capture.Get(VideoCaptureProperties.FrameWidth), (int)capture.Get(VideoCaptureProperties.FrameHeight)));
        List<IProcessor> processorList = new List<IProcessor>();
        using var window = new Window("capture");
        var firstDetect = true;
        int i = 0;
        while (true)
        {
            capture.Read(frame); // same as cvQueryFrame
            if (frame.Empty())
                break;
            // 处理视频帧
            result = _yolov8Face.Detect(frame);
            BoundingBox faceBoundingBox = null;
            if (result.BoundingBox.Count > 0)
            {
                foreach (var box in result.BoundingBox)
                {
                    faceBoundingBox = box;
                    List<Point2f> faceLandmarks = _faceLandmarker.RunModelAndGetLandmarks(frame, faceBoundingBox);
                    List<float> mbedding = faceRecognizerArcface.CalcEmbedding(frame, faceLandmarks);

                    if (face1 == null)
                    {

                        face1 = mbedding;
                    }
                    else
                    {
          
                        var reference_face_distance =Common.CosineSimilarity(face1.ToArray(), mbedding.ToArray());

                        Console.WriteLine($"相似度：{reference_face_distance}");
                        if (reference_face_distance < TempFileManager.Globals.ReferenceFaceDistance)
                        {
                            Mat mat_ = faceSwapper.Detect(frame, mbeddingSource , faceLandmarks);
                            frame = faceEnhancer.EnhanceFace(mat_, faceLandmarks);
                            if (firstDetect)
                            {
                                ProcessManager.Start();
                            }
                            frame.SaveImage(i.ToString() + ".png");
                            i++;
                            firstDetect = false;
                        }

              
                    }
                }    
                Console.WriteLine(string.Join(",", result.ScoreList));
            }


            writer.Write(frame);
            //window.ShowImage(frame);
            //if (faceBoundingBox != null)
            //{
            //    Cv2.WaitKey(1);
            //}
            //else
            //{
            //    Cv2.WaitKey(sleepTime);
            //}
        }
        // 合并视频
        //NLogUtil.Info(Config.GetWord("merging_video").Format(TempFileManager.Globals.OutputVideoResolution,TempFileManager.Globals.OutputVideoFps), nameof(VideoProcessor).ToUpper());

        //if (!MergeVideo(TempFileManager.Globals.TargetPath, TempFileManager.Globals.OutputVideoResolution, TempFileManager.Globals.OutputVideoFps))
        //{
        //    if (IsProcessStopping())
        //    {
        //        return;
        //    }
        //    NLogUtil.Error(Config.GetWord("merging_video_failed"), nameof(VideoProcessor).ToUpper());
        //    return;
        //}
        //else
        //{
        //    NLogUtil.Debug(Config.GetWord("merging_video_succeed"), nameof(VideoProcessor).ToUpper());
        //}
        // 处理音频
        //if (TempFileManager.Globals.SkipAudio)
        //{
        //    NLogUtil.Info(Config.GetWord("skipping_audio"), nameof(VideoProcessor).ToUpper());
        //    TempFileManager.MoveTemp(TempFileManager.Globals.TargetPath, normedOutputPath);
        //}
        //else
        //{
        //    var lipSyncerIncluded =  TempFileManager.Globals.FrameProcessors.Contains("lip_syncer");

        //    if (lipSyncerIncluded)
        //    {
        //        var sourceAudioPath = TempFileManager.FilterAudioPaths(TempFileManager.Globals.SourcePaths).First();
        //        if (sourceAudioPath != null && ReplaceAudio(TempFileManager.Globals.TargetPath, sourceAudioPath, normedOutputPath))
        //        {
        //            NLogUtil.Debug(Config.GetWord("restoring_audio_succeed"), nameof(VideoProcessor).ToUpper());
        //        }
        //        else
        //        {
        //            if (IsProcessStopping())
        //            {
        //                return;
        //            }

        //            NLogUtil.Info(Config.GetWord("restoring_audio_skipped"), nameof(VideoProcessor).ToUpper());
        //            TempFileManager.MoveTemp(TempFileManager.Globals.TargetPath, normedOutputPath);
        //        }
        //    }
        //    else
        //    {
        //        if (RestoreAudio(TempFileManager.Globals.TargetPath, normedOutputPath, TempFileManager.Globals.OutputVideoFps))
        //        {
        //            NLogUtil.Debug(Config.GetWord("restoring_audio_succeed"), nameof(VideoProcessor).ToUpper());
        //        }
        //        else
        //        {
        //            if (IsProcessStopping())
        //            {
        //                return;
        //            }

        //            NLogUtil.Info(Config.GetWord("restoring_audio_skipped"), nameof(VideoProcessor).ToUpper());
        //            TempFileManager.MoveTemp(TempFileManager.Globals.TargetPath, normedOutputPath);
        //        }
        //    }
        //}

        // 清理临时文件
        //NLogUtil.Debug(Config.GetWord("clearing_temp"), nameof(VideoProcessor).ToUpper());
        //TempFileManager.ClearTemp(TempFileManager.Globals.TargetPath);

        //// 验证输出视频文件
        //if (normedOutputPath.IsVideo())
        //{
        //    var seconds = $"{(TimeSpan.FromSeconds(DateTime.Now.Second - startTime)).TotalSeconds:F2}";
        //    NLogUtil.Info(Config.GetWord("ProcessingVideoSucceed").Format( seconds), nameof(VideoProcessor).ToUpper());
        //}
        //else
        //{
        //    NLogUtil.Error(Config.GetWord("ProcessingVideoFailed"), nameof(VideoProcessor).ToUpper());
        //}
        Console.WriteLine(ProcessManager.Stopwatch.ElapsedMilliseconds);
        ProcessManager.End();
    }

    private bool ReplaceAudio(string targetPath, string sourceAudioPath, string normedOutputPath)
    {
        throw new NotImplementedException();
    }

    private bool RestoreAudio(string targetPath, string normedOutputPath, float outputVideoFps)
    {
        throw new NotImplementedException();
    }

    private bool IsProcessStopping()
    {
        if (ProcessManager.IsStopping())
        {
            ProcessManager.End();
            NLogUtil.Info(Config.GetWord("processing_stopped"), nameof(VideoProcessor));
        }
        return ProcessManager.IsPending();
    }
}