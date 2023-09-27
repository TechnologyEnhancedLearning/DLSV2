namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class MockHttpContextSession : ISession
    {
        private readonly Dictionary<string, byte[]> store = new Dictionary<string, byte[]>();

        public void Clear()
        {
            store.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            store.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            store.Add(key, value);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return store.TryGetValue(key, out value);
        }

        public string Id => "1";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => store.Keys;
    }
}
