namespace Iebcn.FaceFusionCSharp.Components
{
    public class Metadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string License { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        // 创建元数据实例
        public static Metadata Instance => new Metadata(
            "Iebcn.FaceFusion",
            "下一代面部交换和增强工具C#版",
            "2.6.0",
            "MIT",
            "guang yan",
            "https://facefusion.iebcn.com"
        );
        public Metadata(string name, string description, string version, string license, string author, string url)
        {
            Name = name;
            Description = description;
            Version = version;
            License = license;
            Author = author;
            Url = url;
        }
    }
}
