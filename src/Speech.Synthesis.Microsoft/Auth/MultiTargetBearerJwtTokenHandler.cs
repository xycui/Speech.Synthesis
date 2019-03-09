namespace Speech.Synthesis.Microsoft.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MultiTargetBearerJwtTokenHandler : DelegatingHandler
    {
        public delegate string GetSubscriptionId(Uri id);

        private static readonly BearerTokenSource TokenSource = BearerTokenSource.Instance;
        private readonly GetSubscriptionId _getSubscriptionId;

        public MultiTargetBearerJwtTokenHandler(IDictionary<string, string> targetSubscriptionMapping)
        {
            _getSubscriptionId = id =>
                targetSubscriptionMapping.TryGetValue(id.ToString(), out string subId) ? subId : null;
        }

        public MultiTargetBearerJwtTokenHandler(GetSubscriptionId getSubscriptionId)
        {
            _getSubscriptionId = getSubscriptionId;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string subscriptionId = _getSubscriptionId(request.RequestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenSource.GetToken(subscriptionId));

            return base.SendAsync(request, cancellationToken);
        }
    }
}
