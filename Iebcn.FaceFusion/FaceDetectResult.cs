namespace Iebcn.FaceFusion;

public class FaceDetectResult
{
    public FaceDetectResult() 
    {
        BoundingBox = new List<BoundingBox>();
        FaceLandmark5List=new List<FaceLandmarker>();
        ScoreList = new List<float>();
    }
    public List<BoundingBox> BoundingBox { get; set; }
    public List<FaceLandmarker> FaceLandmark5List { get; set; }
    public List<float> ScoreList { get; set; }

}
