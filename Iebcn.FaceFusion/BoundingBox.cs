namespace Iebcn.FaceFusion;

public class BoundingBox
{
    public float XMin {  get; set; }
    public float YMin { get; set; }
    public float XMax { get; set; }
    public float YMax { get; set; }

    public BoundingBox(float xmin, float ymin, float xmax, float ymax)
    {
        this.XMin = xmin;
        this.YMin = ymin;
        this.XMax = xmax;
        this.YMax = ymax;
    }
}
