using OpenCvSharp;

namespace Iebcn.FaceFusion;
public interface IProcessor
{
    void PostProcess();
    void Process(Mat image);
}
