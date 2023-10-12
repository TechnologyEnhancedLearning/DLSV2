using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class InMemoryTicketStore : ITicketStore
    {
        private readonly ConcurrentDictionary<string, AuthenticationTicket> _cache;

        public InMemoryTicketStore(ConcurrentDictionary<string, AuthenticationTicket> cache)
        {
            _cache = cache;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var ticketUserId = ticket.Principal.Claims.Where(c => c.Type == "UserID")
                .FirstOrDefault()
                .Value;
            var matchingAuthTicket = _cache.Values.FirstOrDefault(
                t => t.Principal.Claims.FirstOrDefault(
                    c => c.Type == "UserID"
                    && c.Value == ticketUserId) != null);
            if (matchingAuthTicket != null)
            {
                var cacheKey = _cache.Where(
                    entry => entry.Value == matchingAuthTicket)
                    .Select(entry => entry.Key)
                    .FirstOrDefault();
                _cache.TryRemove(
                    cacheKey,
                    out _);
            }
            var key = Guid
                .NewGuid()
                .ToString();
            await RenewAsync(
                key,
                ticket);
            return key;
        }

        public Task RenewAsync(string key,
            AuthenticationTicket ticket)
        {
            _cache.AddOrUpdate(
                key,
                ticket,
                (_, _) => ticket);
            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            _cache.TryGetValue(
                key,
                out var ticket);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.TryRemove(
                key,
                out _);
            return Task.CompletedTask;
        }
    }
}
