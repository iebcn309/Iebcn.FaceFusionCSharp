namespace Iebcn.FaceFusion;

public class Face
{
    public string bounding_box { get; set; }
    public string landmarks { get; set; }
    public Dictionary<string,string> Landmarks { get;  set; }
    public string scores { get; set; }
    public string embedding { get; set; }
    public string normed_embedding { get; set; }
    public string gender { get; set; }
    public string age { get; set; }

}