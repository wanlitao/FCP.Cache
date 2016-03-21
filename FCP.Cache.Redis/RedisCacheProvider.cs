namespace FCP.Cache.Redis
{
    public class RedisCacheProvider : BaseDistributedCacheProvider
    {
        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)。
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
