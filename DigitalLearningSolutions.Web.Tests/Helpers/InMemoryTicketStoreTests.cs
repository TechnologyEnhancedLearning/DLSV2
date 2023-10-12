namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authentication;
    using NUnit.Framework;

    public class InMemoryTicketStoreTests
    {
        [Test]
        public async Task StoreAsync_no_matching_ticket()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            var store = new InMemoryTicketStore(cache);
            var claims = new List<Claim>()
            {
                new Claim(
                    "UserID",
                    "12345"),
            };
            var authenticationTicket = this.CreateAuthenticationTicket(claims);

            // When
            var key = await store.StoreAsync(authenticationTicket);

            // Then
            key
                .Should()
                .NotBeNull();
            key
                .Should()
                .BeOfType<string>();
            cache
                .Should()
                .HaveCount(1);
        }

        [Test]
        public async Task StoreAsync_matching_ticket()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            this.AddTicketToCache(
                cache,
                "12345");
            var store = new InMemoryTicketStore(cache);
            var claims = new List<Claim>()
            {
                new Claim(
                    "UserID",
                    "12345"),
            };
            var authenticationTicket = this.CreateAuthenticationTicket(claims);

            // When
            var key = await store.StoreAsync(authenticationTicket);

            // Then
            key
                .Should()
                .NotBeNull();
            key
                .Should()
                .BeOfType<string>();
            cache
                .Should()
                .HaveCount(1);
        }

        [Test]
        public async Task StoreAsync_existing_entry_no_match()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            this.AddTicketToCache(
                cache,
                "67890");

            var store = new InMemoryTicketStore(cache);
            var claims = new List<Claim>()
            {
                new Claim(
                    "UserID",
                    "12345"),
            };
            var authenticationTicket = this.CreateAuthenticationTicket(claims);

            // When
            var key = await store.StoreAsync(authenticationTicket);

            // Then
            key
                .Should()
                .NotBeNull();
            key
                .Should()
                .BeOfType<string>();
            cache
                .Should()
                .HaveCount(2);
        }

        [Test]
        public async Task RenewAsync_AddToEmptyCache()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            var store = new InMemoryTicketStore(cache);
            var claims = new List<Claim>()
            {
                new Claim(
                    "UserID",
                    "12345"),
            };
            var authenticationTicket = this.CreateAuthenticationTicket(claims);
            var key = Guid
                .NewGuid()
                .ToString();

            // When
            await store.RenewAsync(key, authenticationTicket);

            // Then
            cache
                .Should()
                .HaveCount(1);
        }

        [Test]
        public async Task RenewAsync_AddToNonEmptyCache()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            this.AddTicketToCache(
                cache,
                "67890");
            var store = new InMemoryTicketStore(cache);
            var claims = new List<Claim>()
            {
                new Claim(
                    "UserID",
                    "12345"),
            };
            var authenticationTicket = this.CreateAuthenticationTicket(claims);
            var key = Guid
                .NewGuid()
                .ToString();

            // When
            await store.RenewAsync(key, authenticationTicket);

            // Then
            cache
                .Should()
                .HaveCount(2);
        }


        [Test]
        public async Task RetrieveAsync_NoMatch()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            var store = new InMemoryTicketStore(cache);
            var key = Guid
                .NewGuid()
                .ToString();

            // When
            var authenticationTicket = await store.RetrieveAsync(key);

            // Then
            authenticationTicket
               .Should()
               .BeNull();
            cache
                .Should()
                .HaveCount(0);
        }

        [Test]
        public async Task RetrieveAsync_Match()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            var keyGuid = this.AddTicketToCache(
                cache,
                "67890");
            var store = new InMemoryTicketStore(cache);

            // When
            var authenticationTicket = await store.RetrieveAsync(keyGuid);

            // Then
            authenticationTicket
               .Should()
               .NotBeNull();
            cache
                .Should()
                .HaveCount(1);
        }

        [Test]
        public async Task RemoveAsync_KeyExists()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            var keyGuid = this.AddTicketToCache(
                cache,
                "12345");
            var store = new InMemoryTicketStore(cache);

            // When
            await store.RemoveAsync(keyGuid);

            // Then
            cache
                .Should()
                .HaveCount(0);
        }

        [Test]
        public async Task RemoveAsync_KeyDoesNotExists()
        {
            // Given
            var cache = new ConcurrentDictionary<string, AuthenticationTicket>();
            this.AddTicketToCache(
                cache,
                "12345");
            var store = new InMemoryTicketStore(cache);
            var keyGuid = Guid
                .NewGuid()
                .ToString();

            // When
            await store.RemoveAsync(keyGuid);

            // Then
            cache
                .Should()
                .HaveCount(1);
        }

        private AuthenticationTicket CreateAuthenticationTicket(IEnumerable<Claim> claims)
        {
            var claimsPrincipal = this.CreateClaimsPrincipal(claims);
            var authenticationTicket = new AuthenticationTicket(
                claimsPrincipal,
               "Identity.Application");
            return authenticationTicket;
        }

        private ClaimsPrincipal CreateClaimsPrincipal(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }

        private string AddTicketToCache(
            ConcurrentDictionary<string, AuthenticationTicket> cache,
            string value)
        {
            var claims = new List<Claim>()
            {
                new Claim(
                    "UserID",
                    value),
            };
            var existingAuthenticationTicket = this.CreateAuthenticationTicket(claims);
            var keyGuid = Guid
                .NewGuid()
                .ToString();
            cache.AddOrUpdate(keyGuid,
                existingAuthenticationTicket,
                (_, _) => existingAuthenticationTicket);
            return keyGuid;
        }
    }
}
