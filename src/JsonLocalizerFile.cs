
namespace NC.Localization.Json.Sample
{
    public class JsonLocalizerFile
    {
        /// <summary>
        /// Culture name; eg : en , en-us, zh-CN
        /// </summary>
        public string Culture { get; set; }

        public Dictionary<string, string> Texts { get; set; }

        public JsonLocalizerFile()
        {
            Texts = new Dictionary<string, string>();
        }

        public string GetValue(string key)
        {
            if (Texts.ContainsKey(key))
            {
                return Texts[key];
            }
            return string.Empty;
        }

        public Dictionary<string, string> GetAllValues()
        {
            return Texts;
        }
    }
}
