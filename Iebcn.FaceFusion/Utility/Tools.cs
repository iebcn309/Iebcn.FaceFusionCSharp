
namespace Iebcn.FaceFusion.Utility
{
	public class Tools
	{
		// 根据 gptsovits config.params['gptsovits_role'] 返回以参考音频为key的dict
		public static Dictionary<string, Dictionary<string, string>> GetGPTSoVITSRole()
		{
			var roleList = new Dictionary<string, Dictionary<string, string>>();
			var gptsovitsRole = Config.Settings["gptsovits_role"].ToString()?.Trim();
			if (string.IsNullOrEmpty(gptsovitsRole))
				return null;

			var lines = gptsovitsRole.Split("\n", StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				var parts = line.Trim().Split('#');
				if (parts.Length != 3)
					continue;

				var roleName = parts[0].Trim();
				var referWavPath = parts[0].Trim();
				var promptText = parts[1].Trim();
				var promptLanguage = parts[2].Trim();

				var roleData = new Dictionary<string, string>
				{
					{ "refer_wav_path", referWavPath },
					{ "prompt_text", promptText },
					{ "prompt_language", promptLanguage }
				};

				roleList.Add(roleName, roleData);
			}

			return roleList;
		}
		public static void PygameAudio(string filename)
		{
			throw new NotImplementedException();
		}

		public static void RemoveSilenceFromEnd(string filename)
		{
			throw new NotImplementedException();
		}

		public static void SetProcess(string v)
		{
			throw new NotImplementedException();
		}

		public static void WavToMp3(dynamic dynamic, string filename)
		{
			throw new NotImplementedException();
		}
	}
}