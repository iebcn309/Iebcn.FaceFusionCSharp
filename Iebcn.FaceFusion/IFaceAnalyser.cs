using OpenCvSharp;

namespace Iebcn.FaceFusion;
public interface IFaceAnalyser
{
    FaceDetectResult Detect(Mat srcImage, string faceDetectorSize);
}
