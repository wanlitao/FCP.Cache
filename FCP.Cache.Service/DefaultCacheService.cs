using System;
using System.Linq;
using System.Collections.Generic;

namespace FCP.Cache.Service
{
    public class DefaultCacheService : BaseCacheService
    {
        private readonly ICollection<IDistributedCacheProvider> _cacheProviderCollection;

        public DefaultCacheService(ICollection<IDistributedCacheProvider> cacheProviders)
        {
            if (cacheProviders == null)
                throw new ArgumentNullException(nameof(cacheProviders));

            _cacheProviderCollection = cacheProviders;
        }

        protected override IReadOnlyList<IDistributedCacheProvider> CacheProviders
        {
            get
            {
                return _cacheProviderCollection.ToList().AsReadOnly();
            }
        }
    }
}
