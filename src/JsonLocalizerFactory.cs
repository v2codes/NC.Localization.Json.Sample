using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

namespace NC.Localization.Json.Sample
{
    public class JsonLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cache;
        private readonly IFileProvider _fileProvider;

        public JsonLocalizerFactory(IDistributedCache cache, 
                                    IFileProvider fileProvider)
        {
            _cache = cache;
            _fileProvider = fileProvider;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonLocalizer(_cache, _fileProvider);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonLocalizer(_cache, _fileProvider);
        }
    }
}
