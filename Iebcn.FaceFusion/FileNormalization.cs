using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Iebcn.FaceFusion;
public class FileNormalization
{
    // 将输出路径标准化为文件名
    public static string NormalizeOutputPath(string targetPath, string outputPath)
    {
        if (targetPath != null && outputPath != null)
        {
            var targetName = Path.GetFileNameWithoutExtension(targetPath);
            var targetExtension = Path.GetExtension(targetPath);
            var outputHash = Sha1Hash(TempFileManager.Globals).Substring(0, 8);

            var outputName = $"{targetName}-{outputHash}";

            if (Directory.Exists(outputPath))
            {
                return Path.Combine(outputPath, outputName + targetExtension);
            }

            outputName = Path.GetFileNameWithoutExtension(outputPath);
            var outputExtension = Path.GetExtension(outputPath);
            var outputDirectoryPath = Path.GetDirectoryName(outputPath);
            if (Directory.Exists(outputDirectoryPath) && !string.IsNullOrEmpty(outputExtension))
            {
                return Path.Combine(outputDirectoryPath, $"{outputName}{targetExtension}");
            }
        }
        return null;
    }

    // 将填充标准化为Padding结构
    public static (int, int, int, int)? NormalizePadding(List<int> padding)
    {
        if (padding != null)
        {
            switch (padding.Count)
            {
                case 1:
                    return (padding[0], padding[0], padding[0], padding[0]);
                case 2:
                    return (padding[0], padding[1], padding[0], padding[1]);
                case 3:
                    return (padding[0], padding[1], padding[2], padding[1]);
                case 4:
                    return (padding[0], padding[1], padding[2], padding[3]);
                default:
                    return null;
            }
        }
        return null;
    }

    // 将帧率标准化为Fps结构
    public static double? NormalizeFps(double? fps)
    {
        if (fps.HasValue)
        {
            if (fps.Value < 1.0)
                return 1.0;
            if (fps.Value > 60.0)
                return 60.0;
            return fps.Value;
        }
        return null;
    }

    // 生成SHA1哈希值
    private static string Sha1Hash(object input)
    {
        using (SHA1Managed sha1 = new SHA1Managed())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject( input));
            byte[] hashBytes = sha1.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
