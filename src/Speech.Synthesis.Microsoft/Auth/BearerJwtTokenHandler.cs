namespace Speech.Synthesis.Microsoft.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;

    internal class BearerJwtTokenHandler : DelegatingHandler
    {
        private readonly Uri _fetchTokenUri;
        private readonly string _subscriptionKey;
        private readonly HttpClient _httpClient = new HttpClient();

        private static readonly ISet<HttpStatusCode> ErrorCodes =
            new HashSet<HttpStatusCode>(new[] { HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized });

        private volatile string _token;
        private readonly Timer _accessTokenRenewer;

        public BearerJwtTokenHandler(string subscriptionKey, Region tokenIssueRegion = Region.EastAsia)
        {
            _subscriptionKey = string.IsNullOrEmpty(subscriptionKey) ? throw new Exception("Subscription can not be null or empty.") : subscriptionKey;
            _fetchTokenUri = tokenIssueRegion.GetIssueTokenUri();
            _token = Task.Run(GetLatestToken).Result;
            _accessTokenRenewer = new Timer(OnTokenExpiredCallback,
                this,
                TimeSpan.FromMinutes(9),
                TimeSpan.FromMilliseconds(-1));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (ErrorCodes.Contains(response.StatusCode))
            {
                _token = await GetLatestToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        private async Task<string> GetLatestToken()
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            HttpResponseMessage result = await _httpClient.PostAsync(_fetchTokenUri, null);
            return await result.Content.ReadAsStringAsync();
        }

        private async void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                _token = await GetLatestToken();
            }
            finally
            {
                _accessTokenRenewer.Change(TimeSpan.FromMinutes(9), TimeSpan.FromMilliseconds(-1));
            }
        }
    }
}
