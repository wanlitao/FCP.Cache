namespace FCP.Cache.Memory
{
    public class MemoryCacheProvider : BaseCacheProvider<object>
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
