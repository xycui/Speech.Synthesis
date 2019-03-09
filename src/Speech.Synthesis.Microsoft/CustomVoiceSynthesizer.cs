namespace Speech.Synthesis.Microsoft
{
    using Auth;
    using Core;
    using Core.Params;
    using Extensions;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomVoiceSynthesizer : DynamicPathSynthesizer<SynthesisParams, AudioEncode>
    {
        private readonly HttpClient _synthesisClient;

        public CustomVoiceSynthesizer(IAuthPathProvider<SynthesisParams> pathProvider) : base(pathProvider)
        {
            _synthesisClient = new HttpClient(new MultiTargetBearerJwtTokenHandler(pathProvider.GetSubscriptionId));
        }

        public CustomVoiceSynthesizer(IAuthPathProvider<SynthesisParams> pathProvider,
            ISsmlConverter<SynthesisParams> ssmlConverter) : base(pathProvider, ssmlConverter)
        {
            _synthesisClient = new HttpClient(new MultiTargetBearerJwtTokenHandler(pathProvider.GetSubscriptionId));
        }

        protected override async Task<Stream> InternalSynthesisAsync(string ssml, Uri targetEp, AudioEncode encode, CancellationToken token)
        {
            _ = targetEp == null ? throw new Exception("Endpoint is not available for the request.") : targetEp;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, targetEp)
            {
                Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml")
            };
            request.Headers.Add("Connection", "Keep-Alive");
            request.Headers.Add("X-Microsoft-OutputFormat", encode.GetEncodeName());
            request.Headers.Add("X-FD-ClientID", "RadioStationService");
            request.Headers.Add("X-FD-ImpressionGUID", Guid.NewGuid().ToString());

            HttpResponseMessage response = await _synthesisClient.SendAsync(request, token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception(
                    $"Synthesize fail with status code: {response.StatusCode.ToString()}, error message: [{errorMsg}]");
            }
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return stream;
        }
    }
}
