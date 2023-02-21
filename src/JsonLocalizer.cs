using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using NC.Localization.Json.Sample.Utils;
using System.Text.Json;
using System.Xml.Linq;

namespace NC.Localization.Json.Sample
{
    public class JsonLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly IFileProvider _fileProvider;


        public JsonLocalizer(IDistributedCache cache,
                             IFileProvider fileProvider)
        {
            _cache = cache;
            _fileProvider = fileProvider;
        }

        public LocalizedString this[string key]
        {
            get
            {
                var value = GetString(key);
                return new LocalizedString(key, value);
            }
        }

        public LocalizedString this[string key, params object[] arguments]
        {
            get
            {
                var localizedString = this[key];
                if (!localizedString.ResourceNotFound)
                {
                    return new LocalizedString(key, string.Format(localizedString.Value, arguments));
                }
                return localizedString;
            }
        }

        private string GetString(string key)
        {
            var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
            var cacheValue = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheValue))
                return cacheValue;

            var jsonFile = GetJsonLocalizerFile();
            var result = jsonFile?.GetValue(key);

            if (!string.IsNullOrEmpty(result))
                _cache.SetString(cacheKey, result);

            return result;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var jsonFile = GetJsonLocalizerFile();
            var result = jsonFile?.GetAllValues();
            return result.Select(p => new LocalizedString(p.Key, p.Value));
        }

        private JsonLocalizerFile? GetJsonLocalizerFile()
        {
            var filePath = $"/Resources.{Thread.CurrentThread.CurrentCulture.Name.ToLower()}.json";
            var fileInfo = _fileProvider.GetFileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"本地化json文件未找到，{filePath}");
            }

            using (var stream = fileInfo.CreateReadStream())
            {
                var jsonString = Utf8Helper.ReadStringFromStream(stream);
                try
                {
                    var jsonFile = JsonSerializer.Deserialize<JsonLocalizerFile>(jsonString, DeserializeOptions);
                    return jsonFile;
                }
                catch (JsonException ex)
                {
                    throw new JsonException($"本地化Json文件反序列化失败，{ex.Message}");
                }
            }
        }

        private static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

    }
}
