namespace Speech.Synthesis.Microsoft.Auth
{
    using Extensions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal class BearerTokenSource
    {
        private class TokenRefreshEntity
        {
            public string SubscriptionId;
            public Region TokenIssueRegion = Region.EastAsia;
            public string Token;
            public DateTimeOffset LastRefreshTime;
            public DateTimeOffset NextRefreshTime =>
                LastRefreshTime >= DateTimeOffset.MaxValue - TimeSpan.FromMinutes(TokenTimeOutMinutes)
                    ? DateTimeOffset.MaxValue
                    : LastRefreshTime + TimeSpan.FromMinutes(TokenTimeOutMinutes);
        }

        private static readonly Lazy<BearerTokenSource> LazyInstance = new Lazy<BearerTokenSource>(() => new BearerTokenSource());
        public static BearerTokenSource Instance = LazyInstance.Value;

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IDictionary<string, TokenRefreshEntity> _tokenRefreshEntities = new ConcurrentDictionary<string, TokenRefreshEntity>();
        private readonly IDictionary<string, ManualResetEventSlim> _firstRunEventSlims =
            new ConcurrentDictionary<string, ManualResetEventSlim>();

        public const int TokenTimeOutMinutes = 9;

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _refreshTask;

        public BearerTokenSource()
        {
            StartRefreshTask();
        }

        public string GetToken(string subscriptionId, Region tokenIssueRegion = Region.EastAsia)
        {
            Register(subscriptionId, tokenIssueRegion);
            if (_firstRunEventSlims.TryGetValue(subscriptionId, out ManualResetEventSlim eventSlim))
            {
                eventSlim.Wait();
            }

            return _tokenRefreshEntities.TryGetValue(subscriptionId, out TokenRefreshEntity refreshEntity)
                ? refreshEntity.Token
                : null;
        }

        public void Register(string subscriptionId, Region tokenIssueRegion = Region.EastAsia)
        {
            if (_tokenRefreshEntities.TryGetValue(subscriptionId, out TokenRefreshEntity entity))
            {
                entity.TokenIssueRegion = tokenIssueRegion;
            }
            else
            {
                entity = new TokenRefreshEntity
                {
                    SubscriptionId = subscriptionId,
                    TokenIssueRegion = tokenIssueRegion
                };

                _tokenRefreshEntities[subscriptionId] = entity;
                _firstRunEventSlims[subscriptionId] = new ManualResetEventSlim(false);
                Task.Run(async () =>
                {
                    await RefreshTokenAsync(entity);
                    _firstRunEventSlims.TryGetValue(subscriptionId, out ManualResetEventSlim eventSlim);
                    eventSlim?.Set();
                    _cts.Cancel();
                });
            }
        }

        public void UnRegister(string subscriptionId)
        {
            _tokenRefreshEntities.Remove(subscriptionId);
        }

        private void StartRefreshTask()
        {
            _refreshTask = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        TokenRefreshEntity firstToRefresh = _tokenRefreshEntities.Values
                            .OrderBy(entity => entity.NextRefreshTime).FirstOrDefault();
                        if (firstToRefresh == null || firstToRefresh.NextRefreshTime > DateTimeOffset.UtcNow)
                        {
                            TimeSpan sleepTime = firstToRefresh == null
                                ? TimeSpan.FromMilliseconds(-1)
                                : (firstToRefresh.NextRefreshTime - DateTimeOffset.UtcNow);
                            await Task.Delay(sleepTime, _cts.Token);
                        }
                        else
                        {
                            await RefreshTokenAsync(firstToRefresh);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        _cts = new CancellationTokenSource();
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        private async Task RefreshTokenAsync(TokenRefreshEntity refreshEntity)
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", refreshEntity.SubscriptionId);

            HttpResponseMessage result = await _httpClient.PostAsync(refreshEntity.TokenIssueRegion.GetIssueTokenUri(), null);
            if (result.IsSuccessStatusCode)
            {
                string token = await result.Content.ReadAsStringAsync();
                refreshEntity.Token = token;
                refreshEntity.LastRefreshTime = DateTimeOffset.UtcNow;
            }
            else
            {
                refreshEntity.LastRefreshTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(9 - 0.3);
                throw new Exception($"Get token fail with status code :[{result.StatusCode}], with message [{result.Content.ReadAsStringAsync()}]");
            }
        }
    }
}
