
using OpenCvSharp;

namespace Iebcn.FaceFusion;

public class FaceEnhancerInputs
{
    public Dictionary<string, List<Face>> ReferenceFaces { get; set; }
    public Mat TargetVisionFrame { get; set; }
}