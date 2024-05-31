using OpenCvSharp;
using System.Runtime.InteropServices;

namespace Iebcn.FaceFusion;

public class Common
{
    public static int[] Details;
    public static float[] ExtractMat(Mat src)
    {
        Size size = src.Size();
        int num = src.Channels();
        float[] array = new float[size.Width * size.Height * num];
        GCHandle gCHandle = default(GCHandle);
        try
        {
            gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr intPtr = gCHandle.AddrOfPinnedObject();
            for (int i = 0; i < num; i++)
            {
                Mat mat = new Mat(src.Height, src.Width, MatType.CV_32FC1, intPtr + i * size.Width * size.Height * 4);
                Cv2.ExtractChannel(src, mat, i);
                mat.Dispose();
            }
        }
        finally
        {
            gCHandle.Free();
        }
        return array;
    }
    public static float[] ExtractMat2(Mat src)
    {
        Size size = src.Size();
        int num = src.Channels();
        float[] array = new float[size.Width * size.Height * num];
        GCHandle gCHandle = default(GCHandle);
        try
        {
            gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr intPtr = gCHandle.AddrOfPinnedObject();
            for (int num2 = num - 1; num2 >= 0; num2--)
            {
                Mat mat = new Mat(src.Height, src.Width, MatType.CV_32FC1, intPtr + (num - 1 - num2) * size.Width * size.Height * 4);
                Cv2.ExtractChannel(src, mat, num2);
                mat.Dispose();
            }
        }
        finally
        {
            gCHandle.Free();
        }
        return array;
    }

    public static float GetIoU(BoundingBox box1, BoundingBox box2)
    {
        float num = Math.Max(box1.XMin, box2.XMin);
        float num2 = Math.Max(box1.YMin, box2.YMin);
        float num3 = Math.Min(box1.XMax, box2.XMax);
        float num4 = Math.Min(box1.YMax, box2.YMax);
        float num5 = Math.Max(0f, num3 - num);
        float num6 = Math.Max(0f, num4 - num2);
        float num7 = num5 * num6;
        if (num7 == 0f)
        {
            return 0f;
        }
        float num8 = (box1.XMax - box1.XMin) * (box1.YMax - box1.YMin) + (box2.XMax - box2.XMin) * (box2.YMax - box2.YMin) - num7;
        return num7 / num8;
    }

    public static List<int> nms(List<BoundingBox> boxes, List<float> confidences, float nms_thresh)
    {
        confidences.OrderByDescending((float x) => x).ToList();
        int num = confidences.Count();
        bool[] array = new bool[num];
        for (int i = 0; i < num; i++)
        {
            if (array[i])
            {
                continue;
            }
            for (int j = i + 1; j < num; j++)
            {
                if (!array[j])
                {
                    float ioU = GetIoU(boxes[i], boxes[j]);
                    if (ioU > nms_thresh)
                    {
                        array[j] = true;
                    }
                }
            }
        }
        List<int> list = new List<int>();
        for (int k = 0; k < array.Length; k++)
        {
            if (!array[k])
            {
                list.Add(k);
            }
        }
        return list;
    }

    public static Mat warp_face_by_face_landmark_5(Mat temp_vision_frame, Mat crop_img, List<Point2f> face_landmark_5, List<Point2f> normed_template, Size crop_size)
    {
        Mat mat = new Mat(face_landmark_5.Count, 1, MatType.CV_32FC2, face_landmark_5.ToArray());
        Mat mat2 = new Mat(normed_template.Count, 1, MatType.CV_32FC2, normed_template.ToArray());
        Mat mat3 = new Mat();
        Mat mat4 = Cv2.EstimateAffinePartial2D(mat, mat2, mat3, RobustEstimationAlgorithms.RANSAC, 100.0);
        Cv2.WarpAffine(temp_vision_frame, crop_img, mat4, crop_size, InterpolationFlags.Area, BorderTypes.Replicate);
        return mat4;
    }

    public static Mat create_static_box_mask(int[] crop_size, float face_mask_blur, int[] face_mask_padding)
    {
        float num = (int)((double)crop_size[0] * 0.5 * (double)face_mask_blur);
        int val = Math.Max((int)(num / 2f), 1);
        Mat mat = Mat.Ones(crop_size[0], crop_size[1], MatType.CV_32FC1);
        int height = Math.Max(val, crop_size[1] * face_mask_padding[0] / 100);
        mat[new Rect(0, 0, crop_size[1], height)].SetTo(0.0);
        height = crop_size[0] - Math.Max(val, crop_size[1] * face_mask_padding[2] / 100);
        mat[new Rect(0, height, crop_size[1], crop_size[0] - height)].SetTo(0.0);
        height = Math.Max(val, crop_size[0] * face_mask_padding[3] / 100);
        mat[new Rect(0, 0, height, crop_size[0])].SetTo(0.0);
        height = crop_size[1] - Math.Max(val, crop_size[0] * face_mask_padding[1] / 100);
        mat[new Rect(height, 0, crop_size[1] - height, crop_size[0])].SetTo(0.0);
        if (num > 0f)
        {
            Cv2.GaussianBlur(mat, mat, new Size(0, 0), (double)num * 0.25);
        }
        return mat;
    }

    public static Mat paste_back(Mat temp_vision_frame, Mat crop_vision_frame, Mat crop_mask, Mat affine_matrix)
    {
        Mat mat = new Mat();
        Cv2.InvertAffineTransform(affine_matrix, mat);
        Mat mat2 = new Mat();
        Size dsize = new Size(temp_vision_frame.Cols, temp_vision_frame.Rows);
        Cv2.WarpAffine(crop_mask, mat2, mat, dsize);
        mat2.GetArray<float>(out var data);
        _ = mat2.Cols;
        _ = mat2.Rows;
        mat2.Type();
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] < 0f)
            {
                data[i] = 0f;
            }
            if (data[i] > 1f)
            {
                data[i] = 1f;
            }
        }
        Mat mat3 = new Mat();
        Cv2.WarpAffine(crop_vision_frame, mat3, mat, dsize, InterpolationFlags.Linear, BorderTypes.Replicate);
        Mat[] array = mat3.Split();
        Mat[] array2 = temp_vision_frame.Split();
        for (int j = 0; j < 3; j++)
        {
            array[j].ConvertTo(array[j], MatType.CV_32FC1);
            array2[j].ConvertTo(array2[j], MatType.CV_32FC1);
        }
        Mat[] mv = new Mat[3]
        {
            mat2.Mul(array[0]) + array2[0].Mul(1.0 - mat2),
            mat2.Mul(array[1]) + array2[1].Mul(1.0 - mat2),
            mat2.Mul(array[2]) + array2[2].Mul(1.0 - mat2)
        };
        Mat mat4 = new Mat();
        Cv2.Merge(mv, mat4);
        mat4.ConvertTo(mat4, MatType.CV_8UC3);
        return mat4;
    }

    public static Mat blend_frame(Mat temp_vision_frame, Mat paste_vision_frame, int FACE_ENHANCER_BLEND = 80)
    {
        float num = 1f - (float)FACE_ENHANCER_BLEND / 100f;
        Mat mat = new Mat();
        Cv2.AddWeighted(temp_vision_frame, num, paste_vision_frame, 1f - num, 0.0, mat);
        return mat;
    }
    public static double ComputeCosineSimilarity(List<Point2f> point2fListA, List<Point2f> point2fListB)
    {
        if (point2fListA.Count != point2fListB.Count)
        {
            throw new ArgumentException("Both vectors must have the same length.");
        }
        var vectorA = point2fListA.SelectMany(point => new[] { point.X, point.Y }).ToArray();
        var vectorB = point2fListB.SelectMany(point => new[] { point.X, point.Y }).ToArray();
        double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();

        double normA = Math.Sqrt(vectorA.Select(x => Math.Pow(x, 2)).Sum());
        //double[] normedEmbeddingA = vectorA.Select(x => x / normA).ToArray();
        double normB = Math.Sqrt(vectorB.Select(x => Math.Pow(x, 2)).Sum());
        //double[] normedEmbeddingB = vectorB.Select(x => x / normB).ToArray();
        return dotProduct / (normA * normB);

        //return ComputeCosineSimilarity(normedEmbeddingA, normedEmbeddingB);
    }
    public static double CosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
        {
            throw new ArgumentException("Both vectors must have the same length.");
        }
        var dotProduct = vector1.Zip(vector2, (a, b) => a * b).Sum();
        var norm1 = Math.Sqrt(vector1.Sum(v => v * v));
        var norm2 = Math.Sqrt(vector2.Sum(v => v * v));

        return 1 - dotProduct / (norm1 * norm2);
    }
}
